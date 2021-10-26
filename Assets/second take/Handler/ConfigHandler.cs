using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UVRPN.Core;

public class ConfigHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private int MonitorCount = 1;
    public Transform MonitorPrefab;
    public Transform TobiiPrefab;
    public Transform HeadPrefab;
    private List<Transform> _monitors = new List<Transform>();
    private TobiiHandler _tobiiHandler;
    private VRPNHandler _vrpnHandler;
    private Transform _head;
    void Start()
    {
        SerializeConfig conf = null;
        try
        {
            string jsonFile = System.IO.File.ReadAllText(Application.persistentDataPath + "/setupconfig.json");
            conf = JsonUtility.FromJson<SerializeConfig>(jsonFile);
        }
        catch (UnityException e)
        {
            throw new Exception("Could not read config. " + e.ToString());
        }

        if (conf == null)
        {
            throw new Exception("Could not read config");
        }

        _head = Instantiate(HeadPrefab,new Vector3(0f,0f,-5f),Quaternion.identity);
        if (conf.eyeTracking != null && conf.eyeTracking.usesVRPN)
        {
            _head.GetChild(0).localPosition = conf.eyeTracking.leftEyeOffSet;
            _head.GetChild(1).localPosition = conf.eyeTracking.rightEyeOffSet;
            _head.GetComponent<VRPN_Tracker>().enabled = true;
        }
        
        MonitorCount = Display.displays.Length;
        //test config integrity
        if (conf.monitors.Any(a => conf.monitors.Any(b => a != b && a.monitorIndex == b.monitorIndex)))
        {
            throw new Exception("Two Monitors with same display index are defined in config");
        }

        foreach (var monitor in conf.monitors)
        {
            if (monitor.monitorIndex >= MonitorCount&&!Application.isEditor)
            {
                Debug.Log("Not enough Monitors for index "+monitor.monitorIndex);
                continue;
            }
            
            createMonitorFromConfig(monitor);
            if (conf.eyeTracking!=null&&conf.eyeTracking.usesTobii && conf.eyeTracking.tobiiMonitorIndex == monitor.monitorIndex)
            {
                var monitorObj = _monitors.Last();
                var anchor = monitorObj.Find("TobiiAnchor");
                var tobiiObj = Instantiate(TobiiPrefab,anchor.position,anchor.rotation, anchor);
                _tobiiHandler=tobiiObj.GetComponent<TobiiHandler>();
                _tobiiHandler._eyes = new List<int>() {0, 1}.Select(a => _head.GetChild(a)).ToArray();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void createMonitorFromConfig(MonitorConfig conf)
    {
        var height = (conf.bottomLeft - conf.topLeft);
        var width = (conf.topRight - conf.topLeft);
        var center = conf.topLeft + height / 2 + width / 2;
        var normal = Vector3.Cross(height, width);
        var scale = new Vector3(width.magnitude, 1,height.magnitude);
        var monitor = Instantiate(MonitorPrefab, center, Quaternion.LookRotation(normal, -height));
        var plane = monitor.GetChild(1);
        plane.localScale = scale ;//plane is 10x10 not 1x1
        _monitors.Add(monitor);
        var cam = monitor.GetChild(0).GetComponent<CameraHandler>();
        cam.monitorIndex = conf.monitorIndex;
        cam.stereoType = conf.stereoType;
        cam.eyes=new List<int>() {0, 1}.Select(a => _head.GetChild(a)).ToArray();
    }
    

    public void SaveConfig()
    {
        Debug.Log("pressed");
        SerializeConfig conf = new SerializeConfig();
        var list = new List<MonitorConfig>();
        foreach (var a in _monitors)
        {
            list.Add(a.GetComponent<MonitorHandler>().getConfig());
        }

        conf.monitors = list.ToArray();
        var eyetracking = new EyeTracking();
        var tobiiIndex = SaveTobii();
        if (tobiiIndex != -1)
        {
            eyetracking.usesTobii = true;
            eyetracking.tobiiMonitorIndex = tobiiIndex;
        }
        conf.eyeTracking = eyetracking;
        
        string json = JsonUtility.ToJson(conf);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/setupconfig.json", json);
        Debug.Log(Application.persistentDataPath);
    }

    public int SaveTobii()
    {
        var tobii = (TobiiHandler) GameObject.FindObjectOfType (typeof(TobiiHandler));
        if (tobii == null)
        {
            return -1;
        }

        return tobii.GetComponentInParent<MonitorHandler>().GetComponentInChildren<CameraHandler>().monitorIndex;
    }
}

[CustomEditor(typeof(ConfigHandler))]
// ^ This is the script we are making a custom editor for.
public class YourScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //This draws the default screen.  You don't need this if you want
        //to start from scratch, but I use this when I'm just adding a button or
        //some small addition and don't feel like recreating the whole inspector.
        if (GUILayout.Button("Save Config"))
        {
            (target as ConfigHandler).SaveConfig();
        }
    }
}