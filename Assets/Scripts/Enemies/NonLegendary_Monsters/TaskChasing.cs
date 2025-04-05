using UnityEngine;
using BehaviorTree;

public class TaskChasing : Node
{
    private Transform _transform;
    private Transform _playerTransform;
    public float _chaseSpeed;
    private float _attackRange; // Distancia mínima para detenerse cerca del jugador
    private bool _isChasing;
    private Winged_Monster _wingedMonster;

    public TaskChasing(Winged_Monster wingedMonster, Transform transform, Transform playerTransform, float chaseSpeed, float attackRange)
    {
        _wingedMonster = wingedMonster;
        _transform = transform;
        _playerTransform = playerTransform;
        _chaseSpeed = chaseSpeed;
        _attackRange = attackRange;
        _isChasing = true;
    }

    public override NodeState Evaluate()
    {
        if (_playerTransform == null || !_wingedMonster.isDamagedByPlayer)
        {
            nodeState = NodeState.FAILURE;
            return nodeState;
        }

        float distance = Vector3.Distance(_transform.position, _playerTransform.position);
        if (_isChasing && distance > _attackRange) // Distancia mínima para detenerse cerca del jugador
        {
            Vector3 direction = (_playerTransform.position - _transform.position).normalized;
            _transform.position += direction * _chaseSpeed * Time.deltaTime;
            nodeState = NodeState.RUNNING;
        }
        else
        {
            _isChasing = false;
            nodeState = NodeState.SUCCESS;
        }

        return nodeState;
    }
}
