using BehaviorTree;
using UnityEngine;

public class TaskIsDamage : Node
{
    private Winged_Monster _wingedMonster;

    public TaskIsDamage(Winged_Monster wingedMonster)
    {
        _wingedMonster = wingedMonster;
    }

    public override NodeState Evaluate()
    {
        if (_wingedMonster.isDamagedByPlayer)
        {
            nodeState = NodeState.SUCCESS;
        }
        else
        {
            nodeState = NodeState.FAILURE;
        }
        return nodeState;
    }
}
