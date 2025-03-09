using UnityEngine;
using BehaviorTree;

public class Winged_Monster : BehaviorTree.Tree
{
    public UnityEngine.Transform target;
    public UnityEngine.Transform ownTransform;

    protected override Node CreateTree()
    {
        Node root = new TaskIntimidate(ownTransform, target);
        return root;
    }

    private void OnDrawGizmos()
    {
        if (ownTransform == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ownTransform.position, 10f); // Usa el mismo radio que en TaskIntimidate

        Vector3 leftBoundary = Quaternion.Euler(0, -45f / 2, 0) * ownTransform.forward * 10f; // Usa el mismo ángulo y radio que en TaskIntimidate
        Vector3 rightBoundary = Quaternion.Euler(0, 45f / 2, 0) * ownTransform.forward * 10f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(ownTransform.position, ownTransform.position + leftBoundary);
        Gizmos.DrawLine(ownTransform.position, ownTransform.position + rightBoundary);
    }
}
