using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Assertions;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Vector3 = UnityEngine.Vector3;

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
    [Range(0.0f, 10.0f)] public float CircleSize = 2;
    [Range(0.0001f, 0.1f)] public float MarchEpsilon = 0.001f;
    [Range(1, 1000)] public int MaxIterations = 50;

    //shader ids
    private static readonly int _textureOutId = Shader.PropertyToID("textureOut");
    private static readonly int _viewFrustrumId = Shader.PropertyToID("viewFrustrum");
    private static readonly int _rightEyePositionId = Shader.PropertyToID("rightEyePosition");
    private static readonly int _leftEyePositionId = Shader.PropertyToID("leftEyePosition");
    private static readonly int _resolutionId = Shader.PropertyToID("resolution");
    private static readonly int _circleSizeId = Shader.PropertyToID("circleSize");
    private static readonly int _marchEpsilonId = Shader.PropertyToID("marchEpsilon");
    private static readonly int _maxIterationsId = Shader.PropertyToID("maxIterations");

    private int _sideBySideKernelIndex;
    private int _leftEyeKernelIndex;
    private int _testKernelIndex;

    void Start()
    {
        _cam = gameObject.GetComponent<Camera>();
        _testKernelIndex = shader.FindKernel("CSMain");
        _sideBySideKernelIndex = shader.FindKernel("SideBySide");
        _leftEyeKernelIndex = shader.FindKernel("LeftEyeOnly");
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
        switch (stereoType)
        {
            case StereoType.NO3D:
            case StereoType.LEFT_ONLY:
                shader.Dispatch(_leftEyeKernelIndex, (int) Math.Ceiling(_texture.width / 8.0),
                    (int) Math.Ceiling(_texture.height / 8.0), 1);
                break;
            case StereoType.SIDE_BY_SIDE:
                shader.Dispatch(_sideBySideKernelIndex, (int) Math.Ceiling(_texture.width / 8.0),
                    (int) Math.Ceiling(_texture.height / 8.0), 1);
                break;
        }

        Graphics.Blit(_texture, destination);
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
            case StereoType.LEFT_ONLY:
                shader.SetTexture(_leftEyeKernelIndex, _textureOutId, _texture);
                shader.SetInts(_resolutionId, _texture.width, _texture.height);
                break;
            case StereoType.SIDE_BY_SIDE:
                shader.SetTexture(_sideBySideKernelIndex, _textureOutId, _texture);
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
    RIGHT_ONLY
}