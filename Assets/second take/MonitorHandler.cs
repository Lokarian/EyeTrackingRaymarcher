using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonitorHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private readonly Transform[] _corners= new Transform[4];
    public int monitorIndex
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            _corners[i]=gameObject.transform.GetChild(i);
        }
        
        
    }

    public Vector3[] getCornerPositions()
    {
        return _corners.Select(a => a.position).ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
