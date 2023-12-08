namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class RepeatNode : DecoratorNode
    {
        private int _amount = 0;
        private int _currentLoop = 0;

        public RepeatNode(int amount, BaseNode node) : base(node)
        {
            _amount = amount;
        }

        public override void Abort()
        {
            _amount = 0;
            _node.Abort();
            base.Abort();
        }

        protected override Status OnUpdate()
        {
            if (_currentLoop >= _amount) {
                return Status.Success;
            }

            _currentLoop++;
            _node.Execute();

            return _currentLoop < _amount ? Status.Running : Status.Success;
        }

        protected override void OnExit()
        {
            _amount = 0;
        }
    }
}
