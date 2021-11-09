using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DistanceTreeNodeModel : MonoBehaviour
{
    protected List<DistanceTreeNodeModel> children = new List<DistanceTreeNodeModel>();
    protected DistanceTreeNodeType NodeType;

    public void AddChild(DistanceTreeNodeModel child)
    {
        children.Add(child);
    }

    public abstract DistanceTreeLinearModel? GetLinearModel();

    public void ConstructLinearTree(List<DistanceTreeLinearModel> list)
    {
        foreach (var child in children)
        {
            child.ConstructLinearTree(list);
        }

        var model = GetLinearModel();
        if (model.HasValue)
        {
            list.Add(model.Value);
        }
    }
}

public enum DistanceTreeNodeType
{
    _OR = 0,
    _AND = 1,
    _NOT = 2,
    _SPHERE = 3,
    _TORUS = 4,
}