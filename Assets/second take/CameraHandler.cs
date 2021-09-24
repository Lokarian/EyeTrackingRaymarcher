using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera _cam;
    public MonitorHandler _monitor;
    public ComputeShader shader;
    private static readonly int _textureOutId = Shader.PropertyToID("textureOut");
    private int _kernelIndex;
    private RenderTexture _texture;
    public int monitorIndex;

    void Start()
    {
        _cam = gameObject.GetComponent<Camera>();
        _kernelIndex = shader.FindKernel("CSMain");
        createRenderTexture();
        Display.displays[monitorIndex].Activate();
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


    private void OnPostRender()
    {
        shader.Dispatch(_kernelIndex, (int)Math.Ceiling(_texture.width/8.0), (int)Math.Ceiling(_texture.height/8.0), 1);
        
        Graphics.DrawTexture;
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
        shader.SetTexture(_kernelIndex, _textureOutId, _texture );
    }
    
}