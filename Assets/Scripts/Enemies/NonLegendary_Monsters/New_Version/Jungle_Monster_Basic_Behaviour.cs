using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;

public class Jungle_Monster_Basic_Behaviour : BehaviorTree.Tree
{
    public Transform target;
    public Transform ownTransform;
    public float chaseSpeed = 5f;
    public float attackRange = 0.5f;
    public bool isDamagedByPlayer = false;

    protected override Node CreateTree()
    {
        Jungle_Monster_Task_Idle idle = new Jungle_Monster_Task_Idle();
        Jungle_Monster_Task_Chasing chase = new Jungle_Monster_Task_Chasing(ownTransform, target, chaseSpeed, attackRange);
        Jungle_Monster_Attack attack = new Jungle_Monster_Attack(ownTransform, target, attackRange);
        Jungle_Monster_CheckIfItDamage_By_Player checkIfDamaged = new Jungle_Monster_CheckIfItDamage_By_Player(this);

        Node root = new Selector(new List<Node>
        {
             new Sequence(new List<Node> { checkIfDamaged, new Selector(new List<Node> { new Sequence(new List<Node> { chase, attack }), chase }) }),
            idle
        });
        return root;
    }

    private void OnDrawGizmos()
    {
        if (ownTransform == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ownTransform.position, 10f); // Usa el mismo radio que en Task_Chasing

        Vector3 leftBoundary = Quaternion.Euler(0, -45f / 2, 0) * ownTransform.forward * 10f; // Usa el mismo ángulo y radio que en Task_Chasing
        Vector3 rightBoundary = Quaternion.Euler(0, 45f / 2, 0) * ownTransform.forward * 10f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(ownTransform.position, ownTransform.position + leftBoundary);
        Gizmos.DrawLine(ownTransform.position, ownTransform.position + rightBoundary);
    }
}