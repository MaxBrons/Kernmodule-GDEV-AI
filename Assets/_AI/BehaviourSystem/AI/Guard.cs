using KMGDEVAI.BehaviourSystem.Nodes;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Guard : MonoBehaviour
{
    [SerializeField] private float _viewDistance = 5.0f;
    [SerializeField] private float _fov = 40.0f;
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private float _runningMultiplier = 2.0f;
    [SerializeField] private float _keepDistance = 1.0f;
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private Transform _viewTransform;
    [SerializeField] private Transform _hand;
    [SerializeField] private StateHeaderUI _stateHeaderUI;

    private NavMeshAgent _agent;
    private Animator _animator;
    private BaseNode _behaviourTree;
    private Blackboard _blackboard;
    private Player _player;

    private const string WEAPON = "Weapon";

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _player = FindObjectOfType<Player>();
        _blackboard = new Blackboard();

        _behaviourTree =
            new SequenceNode(
                new GateNode(
                    new SequenceNode(
                        new LookForPlayerNode(_viewTransform, _player, _fov, _viewDistance),
                        new AnimateNode(_animator, AnimationNames.RUN, 0.1f),
                        new UpdateStateHeaderNode(_stateHeaderUI, "Grabbing Weapon", Color.yellow, Color.black),
                        new SearchForWeaponNode(_agent, _speed * _runningMultiplier, _keepDistance, _hand),
                        new UpdateStateHeaderNode(_stateHeaderUI, "Attacking", Color.yellow, Color.black),
                        new SequenceNode(
                            new DoWhileNode(
                                new SequenceNode(
                                    new DoWhileNode(
                                        new MoveToPositionNode(_agent, _speed * _runningMultiplier, _keepDistance, _player.transform),
                                        () => {
                                            var length = _agent.transform.position - _player.transform.position;

                                            return length.sqrMagnitude > _agent.stoppingDistance * _agent.stoppingDistance;
                                        }
                                        ),
                                    new AttackPlayerNode(_player.transform, WEAPON),
                                    new AnimateNode(_animator, AnimationNames.THROW, 0.1f, true),
                                    new WaitNode(_animator.GetCurrentAnimatorStateInfo(0).length)
                                    ),
                                () => (_agent.transform.position - _player.transform.position).sqrMagnitude < _viewDistance * _viewDistance
                                ),
                            new AnimateNode(_animator, AnimationNames.IDLE, 0.1f)
                            )
                ),
                new SequenceNode(
                    new UpdateStateHeaderNode(_stateHeaderUI, "Patrolling", Color.yellow, Color.black),
                    new GetNextWaypointNode(_waypoints),
                    new AnimateNode(_animator, AnimationNames.WALK, 0.1f),
                    new MoveToPositionNode(_agent, _speed, _keepDistance, "Current Waypoint"),
                    new AnimateNode(_animator, AnimationNames.IDLE, 0.1f),
                    new WaitNode(0.5f)
                    )
                )
                );

        _behaviourTree.SetBlackboard(_blackboard);

        _stateHeaderUI.SetHeader("", Color.yellow, Color.black);
    }

    private void Update()
    {
        var result = _behaviourTree?.Execute();
        //print(result);
    }

    private void OnDisable()
    {
        _behaviourTree?.Abort();
        _stateHeaderUI.SetHeader("Stunned", Color.yellow, Color.black);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(_viewTransform.position, _viewTransform.rotation, new Vector3(16.0f / 9.0f, 1.0f, 1.0f));
        Gizmos.DrawFrustum(Vector3.zero, _fov, _viewDistance, 0.5f, 16.0f / 9.0f);
    }

}