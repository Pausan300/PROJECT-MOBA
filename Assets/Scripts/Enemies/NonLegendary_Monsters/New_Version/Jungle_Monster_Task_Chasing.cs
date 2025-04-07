using UnityEngine;
using BehaviorTree;

public class Jungle_Monster_Task_Chasing : Node
{
    private Transform _transform;
    private Transform _playerTransform;
    public float _chaseSpeed;
    private float _attackRange; // Distancia mínima para detenerse cerca del jugador

    public Jungle_Monster_Task_Chasing(Transform transform, Transform playerTransform, float chaseSpeed, float attackRange)
    {
        _transform = transform;
        _playerTransform = playerTransform;
        _chaseSpeed = chaseSpeed;
        _attackRange = attackRange;
    }

    public override NodeState Evaluate()
    {
        if (_playerTransform == null)
        {
            nodeState = NodeState.FAILURE;
            return nodeState;
        }

        float distance = Vector3.Distance(_transform.position, _playerTransform.position);
        if (distance > _attackRange) // Distancia mínima para detenerse cerca del jugador
        {
            Vector3 direction = (_playerTransform.position - _transform.position).normalized;
            _transform.position += direction * _chaseSpeed * Time.deltaTime;
            nodeState = NodeState.RUNNING;
        }
        else
        {
            nodeState = NodeState.SUCCESS;
        }

        return nodeState;
    }
}
