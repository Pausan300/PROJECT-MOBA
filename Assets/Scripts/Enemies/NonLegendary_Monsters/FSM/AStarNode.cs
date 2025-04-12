using UnityEngine;

public class AstarNode
{
    public Vector3 position;
    public float gCost;
    public float hCost;
    public AstarNode parent;

    public float FCost => gCost + hCost;

    public AstarNode(Vector3 position)
    {
        this.position = position;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        AstarNode other = (AstarNode)obj;
        return position == other.position;
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }
}