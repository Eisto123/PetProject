using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DogAI : MonoBehaviour
{
    [Header("Pet AI")]
    [SerializeField] private Transform _playerRef;

    // Component References
    private NavMeshAgent _agent;

    // Path Recalculation
    private float _recalcPathTime = 0.5f;
    private float _recalcPathTimer = 0.0f;

    // State
    private float _stateTimer = 0.0f;
    private Behaviour _currentBehaviour = Behaviour.Idle;

    private Transform _target;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch (_currentBehaviour)
        {
            case Behaviour.Idle:
                Idle();
                break;
            case Behaviour.ReadyToPlay:
                ReadyToPlay();
                break;
            case Behaviour.Playing:
                // Playing();
                break;
        }
    }

    private void Idle()
    {
        HandleRandomPositionCollecting();

        // TIMER TO MOVE TO IDLE POINT
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            State_Idle.WaitBeforeMovingTimer += Time.deltaTime;
            if (State_Idle.WaitBeforeMovingTimer >= State_Idle.WaitBeforeMovingMinTime)
            {
                State_Idle.WaitBeforeMovingTimer = 0.0f;
                State_Idle.UpdateCurrentWaitBeforeMovingTime();
                Vector3 randomPoint;
                try
                {
                    randomPoint = State_Idle.RandomPoints.Dequeue();
                }
                catch (System.InvalidOperationException)
                {
                    Debug.Log("No idle points in queue");
                    randomPoint = Vector3.zero;
                }

                if (randomPoint == Vector3.zero) Debug.LogError("Moved to a zero point");
                _agent.SetDestination(randomPoint);
            }

        }

    }

    private void HandleRandomPositionCollecting()
    {
        // TIMER TO CALCULATE RANDOM POINTS AND ADD THEM TO QUEUE
        if (State_Idle.RandomPoints.Count < 5)
        {
            State_Idle.RecalcRandomPointTimer += Time.deltaTime;
            if (State_Idle.RecalcRandomPointTimer >= State_Idle.RecalcRandomPointTime)
            {
                Vector3 randomPoint = CalculateRandomPoint(transform.position, State_Idle.IdleRadiusCheck);
                if (randomPoint != Vector3.zero)
                    State_Idle.RandomPoints.Enqueue(randomPoint);
            }
        }
    }

    private Vector3 CalculateRandomPoint(Vector3 center, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                return hit.position;
        }
        return Vector3.zero;
    }

    private void ReadyToPlay()
    {
        _target = _playerRef;
        if (Vector3.Distance(transform.position, _target.position) <= _agent.stoppingDistance)
        {

        }
        else
        {
            State_ReadyToPlay.RecalcPathTimer += Time.deltaTime;
            if (State_ReadyToPlay.RecalcPathTimer >= _recalcPathTime)
            {
                State_ReadyToPlay.RecalcPathTimer = 0.0f;
                _agent.SetDestination(_target.position);
            }
        }
    }

    #region DEBUG

    public void DebugTest()
    {
        _currentBehaviour = Behaviour.ReadyToPlay;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, State_Idle.IdleRadiusCheck);
    }

#endif

    #endregion
}
