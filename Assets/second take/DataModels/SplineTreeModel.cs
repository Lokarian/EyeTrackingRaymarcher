using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineTreeModel : DistanceTreeNodeModel
{
    public Transform A;
    public Transform B;
    public Transform C;
    [Range(0.01f,2f)]
    public float Thickness=0.02f;
    // Start is called before the first frame update
    void Start()
    {
        NodeType = DistanceTreeNodeType._SPLINE;
        DesiredChildCount = 0;
    }


    public override DistanceTreeLinearModel GetLinearModel()
    {
        return new DistanceTreeLinearModel()
        {
            translation = transform.worldToLocalMatrix,
            color = _color,
            nodeType = (int)NodeType,
            a=new Vector4(A.transform.position.x,A.transform.position.y,B.transform.position.x,B.transform.position.y),
            b = new Vector4((float)Math.Pow(transform.worldToLocalMatrix.determinant, (double) 1 / 3),C.transform.position.x,C.transform.position.y,Thickness)
        };
    }
}
