
using UnityEngine;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class WaitNode : BaseNode
    {
        private float _timeToWait;
        private float _currentTime;

        public WaitNode(float timeToWait)
        {
            _timeToWait = timeToWait;
        }

        protected override void OnEnter()
        {
            _currentTime = 0.0f;
        }

        protected override Status OnUpdate()
        {
            _currentTime += Time.deltaTime;

            if (_currentTime < _timeToWait)
                return Status.Running;

            return Status.Success;
        }
    }
}