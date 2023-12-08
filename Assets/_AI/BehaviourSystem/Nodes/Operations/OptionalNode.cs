
namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class OptionalNode : DecoratorNode
    {
        public OptionalNode(BaseNode node) : base(node)
        {
        }

        protected override Status OnUpdate()
        {
            _node.Execute();
            return Status.Success;
        }
    }
}