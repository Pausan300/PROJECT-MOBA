using BehaviorTree;

public class Jungle_Monster_Check_IsBackingHome : Node
{
    private Jungle_Monster_Basic_Behaviour _jungleMonster;

    public Jungle_Monster_Check_IsBackingHome(Jungle_Monster_Basic_Behaviour jungleMonster)
    {
        _jungleMonster = jungleMonster;
    }

    public override NodeState Evaluate()
    {
        if (_jungleMonster.isBackingHome)
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
