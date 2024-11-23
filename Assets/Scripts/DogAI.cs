using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DogAI : MonoBehaviour
{
    [Header("Dog AI")]
    [SerializeField] private Transform target;

    // Component References
    private NavMeshAgent _agent;

    // Path Recalculation
    private float _recalcPathTime = 0.5f;
    private float _recalcPathTimer = 0.0f;

    // State
    private float _stateTimer = 0.0f;
    private Behaviour _currentBehaviour = Behaviour.Idle;


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
            case Behaviour.Playing:
                // Playing();
                break;
            case Behaviour.FindPlayer:
                // FindPlayer();
                break;
        }
    }

    private void Idle()
    {
        HandleRandomPositionCollecting();

        // TIMER TO MOVE TO IDLE POINT
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _stateTimer += Time.deltaTime;
            if (_stateTimer >= State_Idle.WaitIdleMinTime)
            {

                _stateTimer = 0.0f;
                State_Idle.UpdateCurrentIdleWaitTime();
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

    private void ChaseTarget()
    {
        _recalcPathTimer += Time.deltaTime;
        if (_recalcPathTimer >= _recalcPathTime)
        {
            _recalcPathTimer = 0.0f;
            _agent.SetDestination(target.position);
        }
    }

    private void HandleRandomPositionCollecting()
    {
        // TIMER TO CALCULATE RANDOM POINTS AND ADD THEM TO QUEUE
        if (State_Idle.RandomPoints.Count < 5)
        {
            State_Idle.RecalcIdleTimer += Time.deltaTime;
            if (State_Idle.RecalcIdleTimer >= State_Idle.RecalcIdleTime)
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

    #region DEBUG

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, State_Idle.IdleRadiusCheck);
    }

#endif

    #endregion
}
