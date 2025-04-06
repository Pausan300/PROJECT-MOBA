
using UnityEngine;
using BehaviorTree;

/*public class CheckHomeDistance : Node
{
    private Winged_Monster_Main_Sequence _wingedMonster;
    private Transform _wingedMonsterPosition;
    private float _sphereAreaHomeDistance;
    private Transform _homeTransform;
    private float _counter;
    private float _counterMax = 2f;

    public CheckHomeDistance(Winged_Monster_Main_Sequence wingedMonster, Transform wingedMonsterPosition, float sphereAreaHomeDistance, Transform homeTransform)
    {
        _wingedMonster = wingedMonster;
        _wingedMonsterPosition = wingedMonsterPosition;
        _sphereAreaHomeDistance = sphereAreaHomeDistance;
        _homeTransform = homeTransform;
        _counter = _counterMax;
    }

    public override NodeState Evaluate()
    {
        float distance = Vector3.Distance(_wingedMonsterPosition.position, _homeTransform.position);
        if (distance > _sphereAreaHomeDistance)
        {
            _counter -= Time.deltaTime;
            if (_counter <= 0)
            {
                _wingedMonster.isDamagedByPlayer = false;
                nodeState = NodeState.SUCCESS;
                return nodeState;
            }
            nodeState = NodeState.RUNNING;
        }
        else
        {
            _counter = _counterMax; // Reset counter if within range
            nodeState = NodeState.FAILURE;
        }
        return nodeState;
    }
}*/