using System;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class DoWhileNode : DecoratorNode
    {
        private Func<bool> _predicate;

        public DoWhileNode(BaseNode node, Func<bool> predicate) : base(node)
        {
            _predicate = predicate;
        }

        protected override Status OnUpdate()
        {
            try {
                if (_predicate()) {
                    var result = _node.Execute();
                    result = result == Status.Success ? Status.Running : result;

                    return result;
                }
            }
            catch {
                return Status.Failed;
            }

            return Status.Success;
        }
    }
}