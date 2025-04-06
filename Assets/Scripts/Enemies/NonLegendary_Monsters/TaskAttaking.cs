using UnityEngine;
using BehaviorTree;

/*public class TaskAttacking : Node
{
    private Winged_Monster_Main_Sequence _wingedMonster;
    private Transform _transform;
    private Transform _playerTransform;
    private float _attackRange;

    public TaskAttacking(Winged_Monster_Main_Sequence wingedMonster, Transform transform, Transform playerTransform, float attackRange)
    {
        _wingedMonster = wingedMonster;
        _transform = transform;
        _playerTransform = playerTransform;
        _attackRange = attackRange;
    }

    public override NodeState Evaluate()
    {
        float distance = Vector3.Distance(_transform.position, _playerTransform.position);
        if (distance <= _attackRange)
        {
            // Lógica de ataque
            Debug.Log("Atacando al jugador");
            nodeState = NodeState.SUCCESS;
        }
        else
        {
            nodeState = NodeState.FAILURE;
        }
        return nodeState;
    }
}*/