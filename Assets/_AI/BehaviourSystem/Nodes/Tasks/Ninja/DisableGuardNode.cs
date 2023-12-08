using UnityEngine;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class DisableGuardNode : BaseNode
    {
        private Guard _guard;
        private float _seconds;
        private float _timer = 0.0f;

        public DisableGuardNode(Guard guard, float seconds)
        {
            _guard = guard;
            _seconds = seconds;
        }

        protected override void OnEnter()
        {
            _guard.enabled = false;
            _timer = 0.0f;
        }

        protected override void OnExit()
        {
            _guard.enabled = true;
        }

        protected override Status OnUpdate()
        {
            if (_timer >= _seconds)
                return Status.Success;

            _timer += Time.deltaTime;

            return Status.Running;
        }
    }
}