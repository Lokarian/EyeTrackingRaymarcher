using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonitorHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private readonly Transform[] _corners= new Transform[4];
    private GameObject _plane;
    public CameraHandler cameraHandler;
    void Start()
    {
        _plane = transform.GetChild(1).gameObject;
        for (int i = 0; i < 4; i++)
        {
            _corners[i]=_plane.transform.GetChild(i);
        }
        
        
    }

    private void OnPostRender()
    {
        
    }
    
    public Vector3[] getCornerPositions()
    {
        return _corners.Select(a => a.position).ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
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
