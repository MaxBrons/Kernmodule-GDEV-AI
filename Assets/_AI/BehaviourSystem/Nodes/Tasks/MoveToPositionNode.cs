using UnityEngine;
using UnityEngine.AI;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class MoveToPositionNode : BaseNode
    {
        private NavMeshAgent _agent;
        private float _speed;
        private float _stoppingDistance;
        private string _positionBBID;
        private Vector3 _targetPosition;
        private Transform _target;

        public MoveToPositionNode(NavMeshAgent agent, float speed, float stoppingDistance, Transform target)
            : this(agent, speed, stoppingDistance, "")
        {
            _target = target;
        }

        public MoveToPositionNode(NavMeshAgent agent, float speed, float stoppingDistance, string positionBBID)
        {
            _agent = agent;
            _speed = speed;
            _stoppingDistance = stoppingDistance;
            _positionBBID = positionBBID;
        }

        protected override void OnEnter()
        {
            _agent.speed = _speed;
            _agent.stoppingDistance = _stoppingDistance;
            _targetPosition = _blackboard.Get<Vector3>(_positionBBID);
        }

        protected override Status OnUpdate()
        {
            _targetPosition = _target != null ? _target.position : _targetPosition;

            if (!NearlyEquals(_agent.pathEndPosition, _targetPosition, 0.1f))
                _agent.SetDestination(_targetPosition);

            if (Vector3.Distance(_agent.transform.position, _targetPosition) <= _stoppingDistance)
                return Status.Success;

            return Status.Running;
        }

        private bool NearlyEquals(Vector3 a, Vector3 b, float difference)
        {
            return (a - b).sqrMagnitude <= difference * difference;
        }
    }
}