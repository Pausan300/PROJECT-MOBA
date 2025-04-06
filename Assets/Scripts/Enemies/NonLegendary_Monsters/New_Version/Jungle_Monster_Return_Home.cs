using UnityEngine;
using BehaviorTree;
public class Jungle_Monster_Return_Home : Node
{
    private Jungle_Monster_Basic_Behaviour _jungleMonster;
    private Transform _transform;
    private Transform _homeTransform;
    private float _returnSpeed = 5f;

    public Jungle_Monster_Return_Home(Jungle_Monster_Basic_Behaviour jungleMonster, Transform transform, Transform homeTransform)
    {
        _jungleMonster = jungleMonster;
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
            _jungleMonster.isDamagedByPlayer = false; // Resetear la booleana
            _jungleMonster.isBackingHome = false; // Resetear la booleana
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
