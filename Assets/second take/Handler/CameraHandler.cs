using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Assertions;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class CameraHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera _cam;
    public MonitorHandler _monitor;
    public ComputeShader shader;
    private RenderTexture _texture;
    public int monitorIndex;
    public StereoType stereoType;
    public Transform[] eyes;
    public Texture2D skybox;
    public bool TakeScreenshot;
    [Range(0.0f, 1000.0f)] public float CircleSize = 2;
    [Range(0.0001f, 0.1f)] public float MarchEpsilon = 0.001f;
    [Range(1, 10000)] public int MaxIterations = 500;
    [Range(0, 5)] public int MaxReflections = 0;
    [Range(0.0f, 1.0f)] public float Shininess = 0.5f;
    [Range(0.0f, 2.0f)]public float AmbientFactor = 0.1f;
    [Range(0.0f, 2.0f)]public float ShadowStartDistance = 0.1f;
    [Range(0.0f, 100.0f)]public float SoftShadowFactor = 8;
    public Color LightColor = Color.white;
    public Transform Light;
    public UdpHandler udpHandler;
    [Range(-1.0f, 1.0f)] public float X = -1.0f;
    [Range(-1.0f, 1.0f)] public float Y = -1.0f;
    [Range(-1.0f, 1.0f)] public float Z = -1.0f;
    [Range(0.0f, 4.0f)] public float Resolution = 1.0f;
    
    //shader ids
    private static readonly int _textureOutId = Shader.PropertyToID("textureOut");
    private static readonly int _viewFrustrumId = Shader.PropertyToID("viewFrustrum");
    private static readonly int _rightEyePositionId = Shader.PropertyToID("rightEyePosition");
    private static readonly int _leftEyePositionId = Shader.PropertyToID("leftEyePosition");
    private static readonly int _resolutionId = Shader.PropertyToID("resolution");
    private static readonly int _circleSizeId = Shader.PropertyToID("sphereSize");
    private static readonly int _marchEpsilonId = Shader.PropertyToID("marchEpsilon");
    private static readonly int _maxIterationsId = Shader.PropertyToID("maxIterations");
    private static readonly int _shadowStartDistanceId = Shader.PropertyToID("shadowStartDistance");
    private static readonly int _softShadowFactorId = Shader.PropertyToID("softShadowFactor");
    private static readonly int _SkyboxId = Shader.PropertyToID("_Skybox");
    
    private static readonly int _lightPositionId = Shader.PropertyToID("lightPosition");
    private static readonly int _lightColorId = Shader.PropertyToID("lightColor");
    private static readonly int _ambientFactorId = Shader.PropertyToID("ambientFactor");
    private static readonly int _shininessId = Shader.PropertyToID("shininess");
    private static readonly int _maxReflectionsId = Shader.PropertyToID("maxReflections");

    private int _sideBySideKernelIndex;
    private int _antiAliasingKernelIndex;
    private int _leftEyeKernelIndex;
    private int _no3dKernelIndex;
    private int _testKernelIndex;
    
    void Start()
    {
        _cam = gameObject.GetComponent<Camera>();
        _testKernelIndex = shader.FindKernel("CSMain");
        _sideBySideKernelIndex = shader.FindKernel("SideBySide");
        _antiAliasingKernelIndex = shader.FindKernel("AntiAliasing");
        _leftEyeKernelIndex = shader.FindKernel("LeftEyeOnly");
        _no3dKernelIndex = shader.FindKernel("No3d");
        createRenderTexture();
        if (!Application.isEditor)
        {
            if (monitorIndex != 0)
            {
                Display.displays[monitorIndex].Activate();
                _cam.targetDisplay = monitorIndex;
            }
            else
            {
                //Camera.main = _cam;
                Camera.main.tag = "Untagged";
                _cam.tag = "MainCamera";
                _cam.targetDisplay = monitorIndex;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_texture.width != _cam.pixelWidth || _texture.height != _cam.pixelHeight)
        {
            Debug.Log("Window size changed, create new Render Texture");
            createRenderTexture();
        }

        if (TakeScreenshot)
        {
            
        }
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Render(destination);
    }
    private void Render(RenderTexture destination)
    {
        updateRenderTexture();
        
        shader.SetMatrix(_viewFrustrumId, cameraMatrix());
        shader.SetFloats(_leftEyePositionId, VectorToArray(eyes[0].position));
        shader.SetFloats(_rightEyePositionId, VectorToArray(eyes[1].position));
        shader.SetFloat(_circleSizeId, CircleSize);
        shader.SetFloat(_marchEpsilonId, MarchEpsilon);
        shader.SetInt(_maxIterationsId, MaxIterations);
        shader.SetInt(_maxReflectionsId, MaxReflections);
        shader.SetInts(_resolutionId, new int[]{(int)Math.Ceiling(_texture.width*Resolution),(int)Math.Ceiling(_texture.height*Resolution)});
        shader.SetFloat(_ambientFactorId, AmbientFactor);
        shader.SetFloat(_shininessId, Shininess);
        shader.SetFloat(_softShadowFactorId, SoftShadowFactor);
        shader.SetFloat(_shadowStartDistanceId, ShadowStartDistance);
        shader.SetFloats(_lightColorId,VectorToArray((Vector4)LightColor));
        shader.SetFloats(_lightPositionId,VectorToArray(Light.position));
        udpHandler.udpPackage.marchEpsilon = MarchEpsilon;
        udpHandler.udpPackage.maxIterations = MaxIterations;
        udpHandler.udpPackage.ambientFactor = AmbientFactor;
        udpHandler.udpPackage.shininess = Shininess;
        udpHandler.udpPackage.maxReflections = MaxReflections;
        udpHandler.udpPackage.shadowStartDistance = ShadowStartDistance;
        udpHandler.udpPackage.softShadowFactor = SoftShadowFactor;
        udpHandler.udpPackage.lightColor = (Vector4)LightColor;
        udpHandler.udpPackage.lightPosition = Light.position;
        switch (stereoType)
        {
            case StereoType.NO3D:
                shader.Dispatch(_no3dKernelIndex, (int) Math.Ceiling(_texture.width*Resolution / 8.0),
                    (int) Math.Ceiling(_texture.height*Resolution / 4.0), 1);
                break;
            case StereoType.LEFT_ONLY:
                shader.Dispatch(_leftEyeKernelIndex, (int) Math.Ceiling(_texture.width*Resolution / 8.0),
                    (int) Math.Ceiling(_texture.height*Resolution / 4.0), 1);
                break;
            case StereoType.SIDE_BY_SIDE:
                shader.Dispatch(_sideBySideKernelIndex, (int) Math.Ceiling(_texture.width*Resolution / 8.0),
                    (int) Math.Ceiling(_texture.height*Resolution / 4.0), 1);
                break;
            case StereoType.ANTIALIASING:
                shader.Dispatch(_antiAliasingKernelIndex, (int) Math.Ceiling(_texture.width*Resolution / 8.0),
                    (int) Math.Ceiling(_texture.height*Resolution / 4.0), 1);
                break;
        }
        Graphics.Blit(_texture, destination,new Vector2(Resolution,Resolution),new Vector2(0,0));
    }

    private void createRenderTexture()
    {
        if (_texture != null)
        {
            _texture.Release();
        }

        Debug.Log("create Render Texture of size " + _cam.pixelWidth + "," + _cam.pixelHeight);
        _texture = new RenderTexture(_cam.pixelWidth, _cam.pixelHeight, 16);
        _texture.enableRandomWrite = true;
        _texture.Create();
        updateRenderTexture();
    }

    private void updateRenderTexture()
    {
        switch (stereoType)
        {
            case StereoType.NO3D:
                shader.SetTexture(_no3dKernelIndex, _textureOutId, _texture);
                shader.SetTexture(_no3dKernelIndex,_SkyboxId, skybox);
                shader.SetInts(_resolutionId, _texture.width, _texture.height);
                break;
            case StereoType.LEFT_ONLY:
                shader.SetTexture(_leftEyeKernelIndex, _textureOutId, _texture);
                shader.SetTexture(_leftEyeKernelIndex,_SkyboxId, skybox);
                shader.SetInts(_resolutionId, _texture.width, _texture.height);
                break;
            case StereoType.SIDE_BY_SIDE:
                shader.SetTexture(_sideBySideKernelIndex, _textureOutId, _texture);
                shader.SetTexture(_sideBySideKernelIndex,_SkyboxId, skybox);
                shader.SetInts(_resolutionId, _texture.width, _texture.height);
                break;
            case StereoType.ANTIALIASING:
                shader.SetTexture(_antiAliasingKernelIndex, _textureOutId, _texture);
                shader.SetTexture(_antiAliasingKernelIndex,_SkyboxId, skybox);
                shader.SetInts(_resolutionId, _texture.width, _texture.height);
                break;
        }
    }

    private Matrix4x4 cameraMatrix()
    {
        Vector3 eyeBL = _monitor.GetCorner(2) - eyes[0].position;
        Vector3 width = _monitor.WidthVector();
        Vector3 height = _monitor.HeightVector();
        Vector3 rightEyeLeftEye = eyes[0].position - eyes[1].position;
        var camFrust = new Matrix4x4();
        camFrust.m00 = width.x;
        camFrust.m10 = width.y;
        camFrust.m20 = width.z;
        camFrust.m01 = height.x;
        camFrust.m11 = height.y;
        camFrust.m21 = height.z;
        camFrust.m02 = eyeBL.x;
        camFrust.m12 = eyeBL.y;
        camFrust.m22 = eyeBL.z;
        camFrust.m03 = rightEyeLeftEye.x;
        camFrust.m13 = rightEyeLeftEye.y;
        camFrust.m23 = rightEyeLeftEye.z;
        //Debug.Log(camFrust.ToString());
        return camFrust;
    }

    public int[] getResolution()
    {
        return new int[] { _texture.width,_texture.height};
    }
    private float[] VectorToArray(Vector3 vecIn)
    {
        return new float[3] {vecIn.x, vecIn.y, vecIn.z};
    }
}

public enum StereoType
{
    NO3D,
    SIDE_BY_SIDE,
    TOP_BOTTOM,
    LEFT_ONLY,
    RIGHT_ONLY,
    ANTIALIASING
}