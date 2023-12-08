namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class InvertNode : DecoratorNode
    {
        public InvertNode(BaseNode node) : base(node)
        {
        }

        protected override Status OnUpdate()
        {
            var result = _node.Execute();

            if (result == Status.Success)
                return Status.Failed;

            if (result == Status.Failed)
                return Status.Success;

            return result;
        }
    }
}