
using UnityEngine;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class GetNextWaypointNode : BaseNode
    {
        private int _currentIndex;
        private Transform[] _waypoints;

        public GetNextWaypointNode(Transform[] waypoints)
        {
            _currentIndex = -1;
            _waypoints = waypoints;
        }

        protected override void OnEnter()
        {
            _currentIndex = (_currentIndex + 1) % _waypoints.Length;
            _blackboard.Set("Current Waypoint", _waypoints[_currentIndex].position);
        }

        protected override Status OnUpdate()
        {
            return Status.Success;
        }
    }
}