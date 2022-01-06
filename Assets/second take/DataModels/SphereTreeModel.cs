using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTreeModel : DistanceTreeNodeModel
{
     
    // Start is called before the first frame update
    void Start()
    {
        NodeType = DistanceTreeNodeType._SPHERE;
        DesiredChildCount = 0;
    }


    public override DistanceTreeLinearModel GetLinearModel()
    {
        return new DistanceTreeLinearModel()
        {
            translation = transform.worldToLocalMatrix,
            color = _color,
            nodeType = (int)NodeType,
            a=new Vector3(0.5f,0,0),
            b = new Vector3((float)Math.Pow(transform.worldToLocalMatrix.determinant, (double) 1 / 3),0.0f,0.0f)
        };
    }
}
