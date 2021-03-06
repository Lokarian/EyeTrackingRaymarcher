using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MonitorHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private readonly Transform[] _corners= new Transform[4];
    private GameObject _plane;
    public CameraHandler cameraHandler;
    public Boolean ResizeForTexture;
    void Start()
    {
        _plane = transform.GetChild(1).gameObject;
        for (int i = 0; i < 4; i++)
        {
            _corners[i]=_plane.transform.GetChild(i);
        }
        
        
    }

    void Update()
    {
        if (ResizeForTexture)
        {
            ResizeForTexture = false;
            var scale = _plane.transform.localScale;
            var res = cameraHandler.getResolution();
            _plane.transform.localScale = new Vector3(scale.z/res[1]*res[0], scale.y, scale.z);
        }
    }
    private void OnPostRender()
    {
        
    }
    
    public Vector3[] GetCornerPositions()
    {
        return _corners.Select(a => a.position).ToArray();
    }
    public Vector3 GetCorner(int index)
    {
        return _corners[index].position;
    }
    public Vector3 WidthVector()
    {
        return _corners[1].position - _corners[0].position;
    }
    public Vector3 HeightVector()
    {
        return _corners[0].position - _corners[2].position;
    }

    public MonitorConfig getConfig()
    {
        var conf=new MonitorConfig();
        conf.topLeft = _corners[0].position;
        conf.topRight = _corners[1].position;
        conf.bottomLeft = _corners[3].position;
        conf.stereoType = cameraHandler.stereoType;
        conf.monitorIndex = cameraHandler.monitorIndex;
        return conf;
    }
    
}
