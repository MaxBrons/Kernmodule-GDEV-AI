using Unity.VisualScripting;
using UnityEngine;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class LookForPlayerNode : BaseNode
    {
        private Player _player;
        private Transform _self;
        private float _fovAngle;
        private float _maxDistance;

        public LookForPlayerNode(Transform self, Player player, float fov, float maxDistance)
        {
            _player = player;
            _self = self;
            _fovAngle = fov;
            _maxDistance = maxDistance;
        }

        protected override Status OnUpdate()
        {
            if (_player.IsUnityNull())
                return Status.Failed;

            Vector3 toTarget = _player.transform.position - _self.transform.position;

            if (Vector3.Angle(_self.transform.forward, toTarget) > _fovAngle)
                return Status.Failed;

            if (!Physics.Raycast(_self.position, toTarget, out RaycastHit hit, _maxDistance))
                return Status.Failed;

            if (hit.transform != _player.transform)
                return Status.Failed;

            if (hit.transform == _player.transform && hit.distance <= _maxDistance)
                return Status.Success;

            return Status.Running;
        }
    }
}