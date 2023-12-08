
using System.Linq;
using UnityEngine;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class GetClosestWaypointNode : BaseNode
    {
        private Transform _self;
        private Transform[] _waypoints;

        public GetClosestWaypointNode(Transform self, Transform[] waypoints)
        {
            _self = self;
            _waypoints = waypoints;
        }

        protected override void OnEnter()
        {
            Transform closest = _waypoints[0];

            foreach (Transform t in _waypoints) {
                var distA = (_self.position - t.position);
                var distB = (_self.position - closest.position);

                if (distA.sqrMagnitude < distB.sqrMagnitude) {
                    closest = t;
                }
            }
           
            _blackboard.Set("Closest Waypoint", closest.position);
        }

        protected override Status OnUpdate()
        {
            if (_waypoints.Length <= 0)
                return Status.Failed;

            return Status.Success;
        }
    }
}