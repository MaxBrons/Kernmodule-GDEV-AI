using KMGDEVAI.BehaviourSystem.Nodes;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ally : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private float _runningMultiplier = 2.0f;
    [SerializeField] private float _keepDistance = 1.0f;
    [SerializeField] private GameObject _smoke;
    [SerializeField] private Transform[] _hidingSpots;
    [SerializeField] private StateHeaderUI _stateHeaderUI;

    private NavMeshAgent _agent;
    private Animator _animator;
    private BaseNode _behaviourTree;
    private Blackboard _blackboard;
    private Transform _player;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _player = FindObjectOfType<Player>().transform;
        _blackboard = new Blackboard();

        _behaviourTree =
            new GateNode
            (
                new SequenceNode
                (
                    new OnPlayerDamagedNode(_player.GetComponent<Health>()),
                    new GetClosestWaypointNode(transform, _hidingSpots),
                    new AnimateNode(_animator, AnimationNames.RUN, 0.1f),
                    new UpdateStateHeaderNode(_stateHeaderUI, "Moving to cover", Color.yellow, Color.black),
                    new MoveToPositionNode(_agent, _speed * 1.5f, _keepDistance, "Closest Waypoint"),
                    new AnimateNode(_animator, AnimationNames.THROW, 0.0f),
                    new UpdateStateHeaderNode(_stateHeaderUI, "Throwing smoke", Color.yellow, Color.black),
                    new WaitNode(1.0f),
                    new ThrowSmokeNode(_smoke, FindObjectOfType<Guard>().transform),
                    new DisableGuardNode(FindObjectOfType<Guard>(), 3.0f),
                    new AnimateNode(_animator, AnimationNames.CROUCH_IDLE, 0.1f)
                ),
                new ParalellNode
                (
                    new UpdateStateHeaderNode(_stateHeaderUI, "Following player", Color.yellow, Color.black),
                    new BranchNode
                    (
                        new AnimateNode(_animator, AnimationNames.CROUCH_IDLE, 0.1f),
                        new AnimateNode(_animator, AnimationNames.CROUCH_WALK, 0.1f),
                        () => (_agent.transform.position - _player.position).sqrMagnitude <= _keepDistance * _keepDistance
                    ),
                    new BranchNode
                    (
                        new MoveToPositionNode(_agent, _speed, _keepDistance, _player),
                        null,
                        () => (_agent.transform.position - _player.position).sqrMagnitude > _keepDistance * _keepDistance
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
}