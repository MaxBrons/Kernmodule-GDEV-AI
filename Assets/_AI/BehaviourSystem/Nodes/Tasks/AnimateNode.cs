using UnityEngine;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public static class AnimationNames
    {
        public const string IDLE = "Idle";
        public const string CROUCH_IDLE = "Crouch Idle";
        public const string WALK = "Walk";
        public const string CROUCH_WALK = "Crouch Walk";
        public const string RUN = "Run";
        public const string THROW = "Throw";
    }

    public sealed class AnimateNode : BaseNode
    {
        private Animator _animator;
        private string _animationName;
        private float _transitionTime;
        private bool _forcePlay;

        public AnimateNode(Animator animator, string animationToPlay, float transitionTime, bool forcePlay = false)
        {
            _animator = animator;
            _animationName = animationToPlay;
            _transitionTime = transitionTime;
            _forcePlay = forcePlay;
        }

        protected override Status OnUpdate()
        {
            var inTransition = _animator.IsInTransition(0);
            var sameName = _animator.GetCurrentAnimatorStateInfo(0).IsName(_animationName);

            if (!inTransition && !sameName) {
                _animator.CrossFade(_animationName, _transitionTime, 0);
                return Status.Running;
            }

            if (_forcePlay) {
                _animator.Play(_animationName, -1, 0.0f);
            }

            return Status.Success;
        }
    }
}