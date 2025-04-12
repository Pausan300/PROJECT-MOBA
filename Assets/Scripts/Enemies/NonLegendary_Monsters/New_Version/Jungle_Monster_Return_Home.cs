using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;

public class Jungle_Monster_Return_Home : Node
{
    private Jungle_Monster_Basic_Behaviour _jungleMonster;
    private Transform _transform;
    private Transform _homeTransform;
    private float _returnSpeed = 5f;
    private AStarPathfinding _pathfinding;
    private List<Vector3> _path;
    private int _currentPathIndex;
    private LayerMask _obstacleMask;

    public Jungle_Monster_Return_Home(Jungle_Monster_Basic_Behaviour jungleMonster, Transform transform, Transform homeTransform, LayerMask obstacleMask, AStarPathfinding pathfinding)
    {
        _jungleMonster = jungleMonster;
        _transform = transform;
        _homeTransform = homeTransform;
        _pathfinding = pathfinding;
        _obstacleMask = obstacleMask;
    }

    public override NodeState Evaluate()
    {
        if (_path == null || _path.Count == 0)
        {
            _path = _pathfinding.FindPath(_transform.position, _homeTransform.position, 0.5f, _obstacleMask);
            _currentPathIndex = 0;
            Debug.Log("Path calculated for returning home");
        }

        if (_path != null && _path.Count > 0)
        {
            Vector3 targetPosition = _path[_currentPathIndex];
            float distance = Vector3.Distance(_transform.position, targetPosition);
            if (distance > 0.1f)
            {
                Vector3 direction = (targetPosition - _transform.position).normalized;
                _transform.position += direction * _returnSpeed * Time.deltaTime;
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

        float homeDistance = Vector3.Distance(_transform.position, _homeTransform.position);
        if (homeDistance <= 0.1f)
        {
            _jungleMonster.isDamagedByPlayer = false; // Resetear la booleana
            _jungleMonster.isBackingHome = false; // Resetear la booleana
            nodeState = NodeState.SUCCESS;
        }

        return nodeState;
    }
}