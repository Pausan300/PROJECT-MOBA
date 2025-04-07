using BehaviorTree;
using UnityEngine;

public class Jungle_Monster_Attack : Node
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Transform _transform;
    private Transform _playerTransform;
    private float _attackRange;

    public Jungle_Monster_Attack(Transform transform, Transform playerTransform, float attackRange)
    {
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
}
