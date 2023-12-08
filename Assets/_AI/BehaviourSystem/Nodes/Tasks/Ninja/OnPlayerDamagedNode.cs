using System.Diagnostics;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class OnPlayerDamagedNode : BaseNode
    {
        private Health _playerHealth;
        private float _hp;

        public OnPlayerDamagedNode(Health playerHealth)
        {
            _playerHealth = playerHealth;
        }

        protected override void OnExit()
        {
            _hp = _playerHealth.HealthPoints;
        }

        protected override Status OnUpdate()
        {
            return _hp > _playerHealth.HealthPoints ? Status.Success : Status.Failed;
        }
    }
}