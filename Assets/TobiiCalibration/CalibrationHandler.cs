using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationHandler : MonoBehaviour
{
    public Transform[] Eyes;

    public Transform markerPrefab;
    public Transform TobiiAnchor;
    public Vector3 CalibrationPoint;
    private List<Vector3[]> snapshots=new List<Vector3[]>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            takeSnapshot();
            if (snapshots.Count>1)
            {
                Debug.Log($"Lengths: {(snapshots[snapshots.Count-1][0]-snapshots[snapshots.Count-2][0]).magnitude}, {(snapshots[snapshots.Count-1][1]-snapshots[snapshots.Count-2][1]).magnitude}");
            }
        }

        if (Input.GetKeyDown("t"))
        {
            this.transform.position = CalibrationPoint - Eyes[1].position;
        }
    }
   
    void takeSnapshot()
    {
        snapshots.Add(new Vector3[]{Eyes[0].position,Eyes[1].position});
        if (markerPrefab != null)
        {
            Instantiate(markerPrefab, Eyes[0].position, Eyes[0].rotation,TobiiAnchor);
            Instantiate(markerPrefab, Eyes[1].position, Eyes[1].rotation,TobiiAnchor);
            
        }
        Debug.Log((Eyes[0].position-Eyes[1].position).magnitude);
    }
    
}
