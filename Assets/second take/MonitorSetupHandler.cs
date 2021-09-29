using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonitorSetupHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private int MonitorCount = 1;
    public Transform MonitorPrefab;
    private List<Transform> _monitors = new List<Transform>();

    void Start()
    {
        SerializeConfig conf=null;
        try
        {

            string jsonFile = System.IO.File.ReadAllText(Application.persistentDataPath + "/setupconfig.json");
            conf = JsonUtility.FromJson<SerializeConfig>(jsonFile);
        }
        catch (UnityException e)
        {
            throw new Exception("Could not read config. "+e.ToString());
        }

        if (conf == null)
        {
            throw new Exception("Could not read config");
        }

        MonitorCount = Display.displays.Length;
        //test config integrity
        if (conf.monitors.Any(a => conf.monitors.Any(b => a != b && a.monitorIndex == b.monitorIndex)))
        {
            throw new Exception("Two Monitors with same display index are defined in config");
        }
        
        foreach (var monitor in conf.monitors)
        {
            createMonitorFromConfig(monitor);
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
        var normal = Vector3.Cross( height,width);
        var scale = new Vector3(width.magnitude, height.magnitude, 1);
        var monitor = Instantiate(MonitorPrefab,center,Quaternion.LookRotation(normal, -height));
        monitor.localScale = scale/2;
        _monitors.Add(monitor);
        var cam=monitor.GetChild(0).GetComponent<CameraHandler>();
        cam.monitorIndex = conf.monitorIndex;
        cam.stereoType = conf.stereoType;
    }
    public void SaveMonitorConfig()
    {
        Debug.Log("pressed");
        SerializeConfig conf = new SerializeConfig();
        var list = new List<MonitorConfig>();
        foreach (var a in _monitors)
        {
            list.Add(a.GetComponent<MonitorHandler>().getConfig());
        }
        conf.monitors = list.ToArray();
        string json = JsonUtility.ToJson(conf);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/setupconfig.json", json);
        Debug.Log(Application.persistentDataPath);
    }
}