using UnityEngine;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class AttackPlayerNode : BaseNode
    {
        private Transform _target;
        private IDamagable _targetHealth;
        private string _weaponName;
        private Weapon _weapon;

        public AttackPlayerNode(Transform target, string weaponName)
        {
            _target = target;
            _weaponName = weaponName;
        }

        protected override void OnEnter()
        {
            _weapon = _blackboard.Get<Weapon>(_weaponName);
            _targetHealth = _target.GetComponent<IDamagable>();
        }

        protected override void OnExit()
        {
            _weapon = null;
            _targetHealth = null;
        }

        protected override Status OnUpdate()
        {
            if (_weapon == null)
                return Status.Failed;

            if (_targetHealth != null) {
                _targetHealth.Damage(_weapon.Damage);

                return Status.Success;
            }

            return Status.Failed;
        }
    }
}