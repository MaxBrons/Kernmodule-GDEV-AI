namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class ParalellNode : CompositeNode
    {
        public ParalellNode(params BaseNode[] nodes) : base(nodes)
        {
        }

        protected override Status OnUpdate()
        {
            foreach (var node in _nodes) {
                node.Execute();
            }

            return Status.Success;
        }
    }
}
