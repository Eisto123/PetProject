using UnityEngine;
using UnityEngine.AI;

public class PetAI : MonoBehaviour
{
    [Header("Pet AI")]
    [SerializeField] private Transform _playerRef;

    [Header("Behaviour")]
    [SerializeField] private float _stoppingDistanceToPlayer = 0.7f;
    [SerializeField] private Transform _pickupPoint;
    [SerializeField] private LayerMask _pickupLayer;

    private Pickup _pickedUpObject;

    // Component References
    private NavMeshAgent _agent;

    // State
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
            case Behaviour.GoPickup:
                GoPickup();
                break;
            case Behaviour.ReturnPickup:
                ReturnPickup();
                break;
        }
    }

    #region State Changes

    public void OnBallPickedUpByPlayer()
    {
        _currentBehaviour = Behaviour.ReadyToPlay;
    }

    public void OnBallThrown(Transform pickupTarget)
    {
        _currentBehaviour = Behaviour.GoPickup;
        _agent.updateRotation = false;
        _target = pickupTarget;
        State_GoPickup.RecalcToTargetTimer = State_GoPickup.RecalcToTargetTime;
    }

    private void OnTargetPickedUp()
    {
        _target = _playerRef;
        _agent.updateRotation = true;
        _currentBehaviour = Behaviour.ReturnPickup;
    }

    private void OnPickupReturned()
    {
        _currentBehaviour = Behaviour.Idle;
    }

    #endregion

    #region State - IDLE

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
                Vector3 randomPoint = CalculateRandomPoint(transform.position, State_Idle.IdleMaxRadiusCheck, State_Idle.IdleMinRadiusCheck);
                if (randomPoint != Vector3.zero)
                    State_Idle.RandomPoints.Enqueue(randomPoint);
            }
        }
    }

    private Vector3 CalculateRandomPoint(Vector3 center, float radius, float minRadius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * radius;
            if (Vector3.Distance(center, randomPoint) < minRadius)
                continue;
            randomPoint = center + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                return hit.position;
        }
        return Vector3.zero;
    }

    #endregion

    #region State - READY TO PLAY

    private void ReadyToPlay()
    {
        _target = _playerRef;
        _agent.stoppingDistance = _stoppingDistanceToPlayer;
        if (Vector3.Distance(transform.position, _target.position) <= _agent.stoppingDistance)
        {
            // WAIT
        }
        else
        {
            State_ReadyToPlay.RecalcPathTimer += Time.deltaTime;
            if (State_ReadyToPlay.RecalcPathTimer >= State_ReadyToPlay.RecalcPathTime)
            {
                State_ReadyToPlay.RecalcPathTimer = 0.0f;
                _agent.SetDestination(_target.position);
            }
        }
    }

    #endregion

    #region State - GO PICKUP

    private void GoPickup()
    {
        _agent.stoppingDistance = State_GoPickup.stoppingDistanceToPickup;
        HandleRotation();
        if (Vector3.Distance(transform.position, _target.position) <= _agent.stoppingDistance)
        {
            ScanForPickup();
        }
        else
        {
            State_GoPickup.RecalcToTargetTimer += Time.deltaTime;
            if (State_GoPickup.RecalcToTargetTimer >= State_GoPickup.RecalcToTargetTime)
            {
                State_GoPickup.RecalcToTargetTimer = 0.0f;
                _agent.SetDestination(_target.position);
            }
        }
    }

    private void ScanForPickup()
    {
        if (Physics.SphereCast(transform.position, State_GoPickup.PickupRadius, transform.forward, out RaycastHit hit, State_GoPickup.PickupRange, _pickupLayer))
        {
            if (hit.transform.TryGetComponent(out Pickup pickup))
            {
                State_GoPickup.WaitBeforePickupTimer += Time.deltaTime;
                if (State_GoPickup.WaitBeforePickupTimer >= State_GoPickup.WaitBeforePickupTime)
                {
                    Pickup(pickup);
                    State_GoPickup.WaitBeforePickupTimer = 0f;
                }
            }
            else
            {
                Debug.Log("No pickup component found!!!");
                State_GoPickup.WaitBeforePickupTimer = 0f;
            }
        }
        else
        {
            Debug.Log("No colliders found!!!");
        }
    }

    private void Pickup(Pickup pickup)
    {
        _pickedUpObject = pickup;
        pickup.OnPickedup();
        pickup.transform.SetParent(_pickupPoint);
        pickup.transform.position = _pickupPoint.position;

        OnTargetPickedUp();
    }

    private void HandleRotation()
    {
        Vector3 nextPos = (_target.position - transform.position).normalized;
        nextPos.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(nextPos);
        targetRotation.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _agent.angularSpeed * Time.deltaTime);
    }

    #endregion

    #region State - RETURN PICKUP

    private void ReturnPickup()
    {
        _agent.stoppingDistance = _stoppingDistanceToPlayer;
        Vector3 _targetPosFloor = new(_target.position.x, transform.position.y, _target.position.z);
        if (Vector3.Distance(transform.position, _targetPosFloor) <= _agent.stoppingDistance)
        {
            _pickedUpObject.transform.SetParent(null);
            _pickedUpObject.OnDropped();
            _pickedUpObject = null;
            OnPickupReturned();
        }
        else
        {
            State_ReturnPickup.RecalcToPlayerTimer += Time.deltaTime;
            if (State_ReturnPickup.RecalcToPlayerTimer >= State_ReturnPickup.RecalcToPlayerTime)
            {
                State_ReturnPickup.RecalcToPlayerTimer = 0.0f;
                _agent.SetDestination(_target.position);
            }
        }
    }

    #endregion

    #region DEBUG

    public void DBG_ChangeStateTo(int newBehaviour)
    {
        _currentBehaviour = (Behaviour)newBehaviour;
    }

#if UNITY_EDITOR

    [Header("DEBUG")]
    [SerializeField] private bool _showIdleRadius = false;
    [SerializeField] private bool _showPickupRange = false;

    private void OnDrawGizmosSelected()
    {
        if (_showIdleRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, State_Idle.IdleMaxRadiusCheck);
        }

        if (_showPickupRange)
        {
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
            Gizmos.color = transparentRed;
            Gizmos.DrawSphere(transform.position + transform.forward * State_GoPickup.PickupRange, State_GoPickup.PickupRadius);
            Gizmos.DrawSphere(transform.position, State_GoPickup.PickupRadius);
        }
    }

#endif

    #endregion
}
