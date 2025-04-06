using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;

/*public class Winged_Monster : BehaviorTree.Tree
{
    public Transform target;
    public Transform ownTransform;
    public Transform homeTransform;
    public float chaseSpeed = 5f;
    public float attackRange = 0.5f;
    public float sphereAreaHomeDistance = 10f;
    public bool isDamagedByPlayer = false;

    protected override Node CreateTree()
    {
        TaskIntimidate intimidate = new TaskIntimidate(ownTransform, target);
        TaskChasing chase = new TaskChasing(this, ownTransform, target, chaseSpeed, attackRange);
        TaskIsDamage checkIfDamaged = new TaskIsDamage(this);
        CheckHomeDistance checkHomeDistance = new CheckHomeDistance(this, ownTransform, sphereAreaHomeDistance, homeTransform);

        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node> { checkIfDamaged, chase }),
            new Sequence(new List<Node> { checkHomeDistance, new TaskReturnHome(ownTransform, homeTransform) }),
            intimidate
        });
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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(homeTransform.position, sphereAreaHomeDistance); // Dibuja el área de la casa
    }
}*/