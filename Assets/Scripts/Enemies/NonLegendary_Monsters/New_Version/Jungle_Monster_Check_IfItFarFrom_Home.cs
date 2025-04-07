using UnityEngine;
using BehaviorTree;

public class Jungle_Monster_Check_IfItFarFrom_Home : Node
{
    private Jungle_Monster_Basic_Behaviour _jungleMonster;
    private Transform _wingedMonsterPosition;
    private float _sphereAreaHomeDistance;
    private Transform _homeTransform;
    private float _counter;
    private float _counterMax = 2f;

    public Jungle_Monster_Check_IfItFarFrom_Home(Jungle_Monster_Basic_Behaviour jungleMonster, Transform wingedMonsterPosition, float sphereAreaHomeDistance, Transform homeTransform)
    {
        _jungleMonster = jungleMonster;
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
           /* _counter -= Time.deltaTime;
            if (_counter <= 0)
            {*/
                _jungleMonster.isDamagedByPlayer = false;
                _jungleMonster.isBackingHome = true;
                _jungleMonster.target = _homeTransform;
                nodeState = NodeState.SUCCESS;
                return nodeState;
            //}
            nodeState = NodeState.RUNNING;
        }
        else
        {
           // _counter = _counterMax; // Reset counter if within range
            nodeState = NodeState.FAILURE;
        }
        return nodeState;
    }
}
