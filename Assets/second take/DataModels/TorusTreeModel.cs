using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorusTreeModel : DistanceTreeNodeModel
{
    public float radius1=0.5f;
    public float radius2=0.5f;
    void Start()
    {
        NodeType = DistanceTreeNodeType._TORUS;
        DesiredChildCount = 0;
    }
    public override DistanceTreeLinearModel GetLinearModel()
    {
        return new DistanceTreeLinearModel()
        {
            a = new Vector3(radius1, radius2, 0.0f),
            translation = transform.worldToLocalMatrix,
            color = _color,
            nodeType = (int)NodeType,
            b = new Vector3(0.0f,transform.worldToLocalMatrix.determinant,3.14159265359f)
        };
    }

}
