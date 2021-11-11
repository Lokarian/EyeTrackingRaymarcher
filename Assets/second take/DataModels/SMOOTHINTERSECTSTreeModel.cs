using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMOOTHINTERSECTSTreeModel : DistanceTreeNodeModel
{
    // Start is called before the first frame update
    [Range(0.001f,2.0f)]
    public float smoothness = 0.05f;
    void Start()
    {
        NodeType = DistanceTreeNodeType._SMOOTHINTERSECTS;
        DesiredChildCount = 2;
    }

    private void OnEnable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).GetComponent<DistanceTreeNodeModel>();
            if (child != null)
            {
                children.Add(child);
            }
        }
    }


    public override DistanceTreeLinearModel GetLinearModel()
    {
        return new DistanceTreeLinearModel()
        {
            nodeType = (int)NodeType,
            a = new Vector3(smoothness,0.0f,0.0f)
        };
    }
}
