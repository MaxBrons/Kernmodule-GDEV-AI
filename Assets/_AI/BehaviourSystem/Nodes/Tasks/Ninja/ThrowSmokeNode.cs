using UnityEngine;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class ThrowSmokeNode : BaseNode
    {
        private GameObject _smokePrefab;
        private Transform _positionToSpawn;

        public ThrowSmokeNode(GameObject smokePrefab, Transform positionToSpawn)
        {
            _smokePrefab = smokePrefab;
            _positionToSpawn = positionToSpawn;
        }

        protected override Status OnUpdate()
        {
            GameObject.Instantiate(_smokePrefab, _positionToSpawn.position, Quaternion.identity);
            return Status.Success;
        }
    }
}