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
        color= Color.green;
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
