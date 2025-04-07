using UnityEngine;
using System.Collections.Generic;
using BehaviorTree;
public class TaskIntimidate : Node
{
    private Transform _transform;
    private Transform _playerTransform;
    public float detectionRadius = 10f;
    public float detectionAngle = 45f;

    public TaskIntimidate(Transform transform, Transform playerTransform)
    {
        _transform = transform;
        _playerTransform = playerTransform;
    }

    public override NodeState Evaluate()
    {
        if (IsPlayerInCone())
        {
            // Lógica cuando el jugador es detectado
            nodeState = NodeState.SUCCESS;
        }
        else
        {
            nodeState = NodeState.FAILURE;
        }
        return nodeState;
    }

    private bool IsPlayerInCone()
    {
        Vector3 directionToPlayer = (_playerTransform.position - _transform.position).normalized;
        float angleToPlayer = Vector3.Angle(_transform.forward, directionToPlayer);

        if (angleToPlayer < detectionAngle / 2 && Vector3.Distance(_transform.position, _playerTransform.position) < detectionRadius)
        {
            Debug.Log("detecta player");
            return true;
        }
        Debug.Log("Deja de detectar player");
        return false;
    }
}
