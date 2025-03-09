using UnityEngine;
using System.Collections.Generic;   

namespace BehaviorTree
{
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildRunning = false;
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        nodeState = NodeState.FAILURE;
                        return nodeState;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildRunning = true;
                        continue;
                    default:
                        nodeState = NodeState.SUCCESS;
                        return nodeState;
                }
            }
            nodeState = anyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return nodeState;
        }
    }
}

public class Sequence
{
    
}
