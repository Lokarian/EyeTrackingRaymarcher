using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUBETreeModel : DistanceTreeNodeModel
{
    void Start()
    {
        NodeType = DistanceTreeNodeType._CUBE;
        DesiredChildCount = 0;
        color= Color.blue;
    }
    public override DistanceTreeLinearModel GetLinearModel()
    {
        return new DistanceTreeLinearModel()
        {
            translation = transform.worldToLocalMatrix,
            color = _color,
            nodeType = (int)NodeType,
            b = new Vector3(0.0f,transform.worldToLocalMatrix.determinant,3.14159265359f)
        };
    }

}
