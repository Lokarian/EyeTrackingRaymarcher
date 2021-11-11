using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DistanceTreeNodeModel : MonoBehaviour
{
    protected List<DistanceTreeNodeModel> children = new List<DistanceTreeNodeModel>();
    public DistanceTreeNodeType NodeType;
    protected int DesiredChildCount;
    private Transform parent;

    private void Update()
    {
        var currentParent = transform.parent;
        if (currentParent != parent)
        {
            if (parent != null)
            {
                var nodeParent = (DistanceTreeNodeModel)parent.GetComponent(typeof(DistanceTreeNodeModel));
                if (nodeParent != null)
                {
                    nodeParent.UpdateChildren();
                }
            }
            if (currentParent != null)
            {
                var nodeParent = (DistanceTreeNodeModel)currentParent.GetComponent(typeof(DistanceTreeNodeModel));
                if (nodeParent != null)
                {
                    nodeParent.UpdateChildren();
                }
            }
        }
    }

    private void UpdateChildren()
    {
        children.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var nodeChild = (DistanceTreeNodeModel)child.GetComponent(typeof(DistanceTreeNodeModel));
            if (nodeChild != null)
            {
                children.Add(nodeChild);
            }
        }
    }

    public abstract DistanceTreeLinearModel GetLinearModel();

    public bool IsValid()
    {
        return children.Count == DesiredChildCount;
    }

    public bool LinearizeTree(List<DistanceTreeNodeModel> list)
    {
        if (!IsValid())
        {
            return false;
        }
        foreach (var child in children)
        {
            if (!child.LinearizeTree(list))
            {
                return false;
            }
        }
        list.Add(this);
        return true;
    }
}

public enum DistanceTreeNodeType
{
    _OR = 0,
    _AND = 1,
    _NOT = 2,
    _SPHERE = 3,
    _TORUS = 4,
    _SMOOTHINTERSECTS = 5,
    _SMOOTHUNION = 6,
    _SMOOTHDIFFERENCE = 7,
    _CUBE = 8,
}