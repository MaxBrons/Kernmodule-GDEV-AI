using System;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class ActionNode : BaseNode
    {
        private event Func<Status> _action;

        public ActionNode(Func<Status> action)
        {
            _action = action;
        }

        protected override Status OnUpdate()
        {
            return _action?.Invoke() ?? Status.Failed;
        }
    }
}