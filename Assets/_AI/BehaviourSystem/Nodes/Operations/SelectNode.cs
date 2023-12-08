namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class SelectNode : CompositeNode
    {
        public SelectNode(params BaseNode[] nodes) : base(nodes)
        {
        }

        protected override Status OnUpdate()
        {
            foreach (var node in _nodes) {
                var result = node.Execute();

                if (result != Status.Success)
                    continue;
                return Status.Success;
            }
            return Status.Failed;
        }
    }
}