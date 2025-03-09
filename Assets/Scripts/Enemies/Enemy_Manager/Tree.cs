using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel.Design.Serialization;

namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node _root = null;

        protected void Start()
        {
            _root = CreateTree();
        }
        private void Update()
        {
            if (_root != null) 
                _root.Evaluate();
        }

        protected abstract Node CreateTree();

    }
}

