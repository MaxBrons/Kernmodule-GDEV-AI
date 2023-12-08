using System;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class BranchNode : BaseNode
    {
        private BaseNode _trueNode;
        private BaseNode _falseNode;
        private Func<bool> _predicate;

        public BranchNode(BaseNode trueNode, BaseNode falseNode, Func<bool> predicate)
        {
            _trueNode = trueNode;
            _falseNode = falseNode;
            _predicate = predicate;
        }

        public override void SetBlackboard(Blackboard blackboard)
        {
            base.SetBlackboard(blackboard);
            _trueNode?.SetBlackboard(blackboard);
            _falseNode?.SetBlackboard(blackboard);
        }

        protected override Status OnUpdate()
        {
            try {
                if (_predicate())
                    return _trueNode?.Execute() ?? Status.Failed;
            }
            catch {
                return Status.Failed;
            }

            return _falseNode?.Execute() ?? Status.Failed;
        }
    }
}