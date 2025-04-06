using UnityEngine;
using BehaviorTree;
public class Jungle_Monster_Task_Idle : Node
{
    public override NodeState Evaluate()
    {
        // Lógica de Idle
        Debug.Log("Idle");
        nodeState = NodeState.SUCCESS;
        return nodeState;
    }
}
