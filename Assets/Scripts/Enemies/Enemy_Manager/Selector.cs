using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace BehaviorTree
{
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        nodeState = NodeState.SUCCESS;
                        return nodeState;
                    case NodeState.RUNNING:
                        nodeState = NodeState.RUNNING;
                        return nodeState;
                    default:
                        continue;
                }
            }
            nodeState = NodeState.FAILURE;
            return nodeState;
        }
    }

}

