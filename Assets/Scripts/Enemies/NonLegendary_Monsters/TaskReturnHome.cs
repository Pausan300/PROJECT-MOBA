using UnityEngine;
using BehaviorTree;

public class TaskReturnHome : Node
{
    private Transform _transform;
    private Transform _homeTransform;
    private float _returnSpeed = 5f;

    public TaskReturnHome(Transform transform, Transform homeTransform)
    {
        _transform = transform;
        _homeTransform = homeTransform;
    }

    public override NodeState Evaluate()
    {
        float distance = Vector3.Distance(_transform.position, _homeTransform.position);
        if (distance > 0.1f) // Distancia mínima para detenerse cerca de la casa
        {
            Vector3 direction = (_homeTransform.position - _transform.position).normalized;
            _transform.position += direction * _returnSpeed * Time.deltaTime;
            nodeState = NodeState.RUNNING;
        }
        else
        {
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
