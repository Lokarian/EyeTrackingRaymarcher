using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UNDOSPACETRANSFORMTreeModel : DistanceTreeNodeModel
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override DistanceTreeLinearModel GetLinearModel()
    {
        return new DistanceTreeLinearModel()
        {
            nodeType = (int)DistanceTreeNodeType._UNDOTRANSFORM,
        };
    }
}
