using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUBETreeModel : DistanceTreeNodeModel
{
    void Start()
    {
        NodeType = DistanceTreeNodeType._CUBE;
        DesiredChildCount = 0;
        
    }
    public override DistanceTreeLinearModel GetLinearModel()
    {
        return new DistanceTreeLinearModel()
        {
            translation = transform.worldToLocalMatrix,
            color = Vector3.right,
            nodeType = (int)NodeType,
        };
    }

}
