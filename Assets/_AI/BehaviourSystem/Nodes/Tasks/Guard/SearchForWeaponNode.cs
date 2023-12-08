using UnityEngine;
using UnityEngine.AI;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class SearchForWeaponNode : BaseNode
    {
        private Transform _hand;
        private Weapon _weapon;
        private bool _equiped = false;
        private MoveToPositionNode _moveToPositionNode;

        public SearchForWeaponNode(NavMeshAgent agent, float speed, float stoppingDistance, Transform hand)
        {
            _hand = hand;
            _moveToPositionNode = new(agent, speed, stoppingDistance, "Weapon Position");
        }

        public override void SetBlackboard(Blackboard blackboard)
        {
            base.SetBlackboard(blackboard);
            _moveToPositionNode.SetBlackboard(blackboard);
        }

        public override void Abort()
        {
            base.Abort();
            _moveToPositionNode.Abort();
        }

        protected override void OnEnter()
        {
            _weapon = _weapon != null ? _weapon : Object.FindObjectOfType<Weapon>();

            if (_weapon)
                _equiped = _weapon.transform.parent == _hand;

            _blackboard.Set("Weapon", _weapon);
            _blackboard.Set("Weapon Position", _weapon.transform.position);
        }

        protected override Status OnUpdate()
        {
            if (!_equiped) {
                if (_weapon == null)
                    return Status.Failed;

                var result = _moveToPositionNode.Execute();

                if (result != Status.Success)
                    return Status.Running;

                _weapon.transform.SetPositionAndRotation(_hand.position, _hand.rotation);
                _weapon.transform.parent = _hand;
            }

            return Status.Success;
        }
    }
}