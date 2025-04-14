using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;

public class Jungle_Monster_Task_Chasing : Node
{
    private Transform _transform;
    private Transform _playerTransform;
    public float _chaseSpeed;
    private float _attackRange; // Distancia mínima para detenerse cerca del jugador
    private AStarPathfinding _pathfinding;
    private List<Vector3> _path;
    private int _currentPathIndex;
    private LayerMask _obstacleMask;

    public Jungle_Monster_Task_Chasing(Transform transform, Transform playerTransform, float chaseSpeed, float attackRange, LayerMask obstacleMask, AStarPathfinding pathfinding)
    {
        _transform = transform;
        _playerTransform = playerTransform;
        _chaseSpeed = chaseSpeed;
        _attackRange = attackRange;
        _pathfinding = pathfinding;
        _obstacleMask = obstacleMask;
    }

    public override NodeState Evaluate()
    {
        if (_playerTransform == null)
        {
            nodeState = NodeState.FAILURE;
            return nodeState;
        }

        if (_path == null || _path.Count == 0)
        {
            _path = _pathfinding.FindPath(_transform.position, _playerTransform.position, 0.5f, _obstacleMask);
            _currentPathIndex = 0;
            Debug.Log("Path calculated for chasing");
        }

        if (_path != null && _path.Count > 0)
        {
            Vector3 targetPosition = _path[_currentPathIndex];
            float distance = Vector3.Distance(_transform.position, targetPosition);
            if (distance > 0.1f)
            {
                Vector3 direction = (targetPosition - _transform.position).normalized;
                _transform.position += direction * _chaseSpeed * Time.deltaTime;
                nodeState = NodeState.RUNNING;
            }
            else
            {
                _currentPathIndex++;
                if (_currentPathIndex >= _path.Count)
                {
                    _path = null;
                }
            }
        }

        float playerDistance = Vector3.Distance(_transform.position, _playerTransform.position);
        if (playerDistance <= _attackRange)
        {
            nodeState = NodeState.SUCCESS;
        }

        return nodeState;
    }
}