using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorusTreeModel : DistanceTreeNodeModel
{
    public float radius=0.5f;
    void Start()
    {
        NodeType = DistanceTreeNodeType._TORUS;
        DesiredChildCount = 0;
        
    }
    public override DistanceTreeLinearModel GetLinearModel()
    {
        return new DistanceTreeLinearModel()
        {
            a = new Vector3(1, radius, 0.0f),
            translation = transform.worldToLocalMatrix,
            color = Vector3.right,
            nodeType = (int)NodeType,
        };
    }

}
