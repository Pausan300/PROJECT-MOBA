using UnityEngine;
using BehaviorTree;

public class Jungle_Monster_CheckIfItDamage_By_Player : Node
{
    private Jungle_Monster_Basic_Behaviour _jungleMonster;

    public Jungle_Monster_CheckIfItDamage_By_Player(Jungle_Monster_Basic_Behaviour jungleMonster)
    {
        _jungleMonster = jungleMonster;
    }

    public override NodeState Evaluate()
    {
        if (_jungleMonster.isDamagedByPlayer)
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
