using System;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class GateNode : BaseNode
    {
        private BaseNode _conditionalNode;
        private BaseNode _otherNode;
        private Func<bool> _predicate;

        public GateNode(BaseNode conditional, Func<bool> predicate)
        {
            _conditionalNode = conditional;
            _otherNode = null;
            _predicate = predicate;
        }

        public GateNode(BaseNode conditional, BaseNode other)
        {
            _conditionalNode = conditional;
            _otherNode = other;
        }

        public override void SetBlackboard(Blackboard blackboard)
        {
            base.SetBlackboard(blackboard);
            _conditionalNode?.SetBlackboard(blackboard);
            _otherNode?.SetBlackboard(blackboard);
        }

        protected override Status OnUpdate()
        {
            try {
                if (_predicate != null && _predicate()) {
                    return _conditionalNode?.Execute() ?? Status.Failed;
                }
            }
            catch {
                return Status.Failed;
            }

            var result = _conditionalNode?.Execute() ?? Status.Failed;

            if (result == Status.Failed)
                return _otherNode?.Execute() ?? Status.Failed;

            return result;
        }
    }
}