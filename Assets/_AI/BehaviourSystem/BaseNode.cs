using System.Collections.Generic;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public abstract class BaseNode
    {
        public enum Status
        {
            None,
            Running,
            Failed,
            Success
        }

        protected Blackboard _blackboard = null;

        private bool _started = false;

        public Status Execute()
        {
            if (!_started) {
                OnEnter();
                _started = true;
            }

            var result = OnUpdate();

            if (result != Status.Running) {
                OnExit();
                _started = false;
            }
            return result;
        }

        public virtual void SetBlackboard(Blackboard blackboard)
        {
            _blackboard = blackboard;
        }

        public virtual void Abort()
        {
            OnExit();
            _started = false;
        }

        protected virtual void OnEnter() { }
        protected abstract Status OnUpdate();
        protected virtual void OnExit() { }
    }

    public abstract class CompositeNode : BaseNode
    {
        protected BaseNode[] _nodes;

        public CompositeNode(params BaseNode[] nodes)
        {
            _nodes = nodes;
        }

        public override void SetBlackboard(Blackboard blackboard)
        {
            base.SetBlackboard(blackboard);
            foreach (var node in _nodes) {
                node.SetBlackboard(blackboard);
            }
        }

        public override void Abort()
        {
            base.Abort();
            foreach (var node in _nodes) {
                node.Abort();
            }
        }
    }

    public abstract class DecoratorNode : BaseNode
    {
        protected BaseNode _node;

        public DecoratorNode(BaseNode node)
        {
            _node = node;
        }

        public override void SetBlackboard(Blackboard blackboard)
        {
            base.SetBlackboard(blackboard);
            _node.SetBlackboard(blackboard);
        }

        public override void Abort()
        {
            base.Abort();
            _node.Abort();
        }
    }
}
