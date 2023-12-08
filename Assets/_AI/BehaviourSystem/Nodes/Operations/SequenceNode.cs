namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class SequenceNode : CompositeNode
    {
        private int _currentIndex = 0;

        public SequenceNode(params BaseNode[] nodes) : base(nodes)
        {
        }

        protected override void OnEnter()
        {
            _currentIndex = 0;
        }

        protected override void OnExit()
        {
            _currentIndex = 0;
        }

        protected override Status OnUpdate()
        {
            for (; _currentIndex < _nodes.Length; _currentIndex++) {
                Status result = _nodes[_currentIndex].Execute();

                if (result != Status.Success)
                    return result;
            }
            return Status.Success;
        }
    }
}
