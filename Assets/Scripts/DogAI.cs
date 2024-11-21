using System.Collections;
using System.Collections.Generic;
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
        // TIMER TO RECALCULATE IDLE POINT
        State_Idle.RecalcIdleTimer += Time.deltaTime;
        if (State_Idle.RecalcIdleTimer >= State_Idle.RecalcIdleTime)
        {
            Vector3 randomPoint = CalculateRandomPoint();
            if (randomPoint != Vector3.zero)
                State_Idle.RandomPoints.Enqueue(CalculateRandomPoint());
        }

        // TIMER TO MOVE TO IDLE POINT
        _stateTimer += Time.deltaTime;
        if (_stateTimer >= State_Idle.MoveIdleTime)
        {
            _stateTimer = 0.0f;
            Vector3 randomPoint;
            try
            {
                randomPoint = State_Idle.RandomPoints.Dequeue();
            }
            catch (System.InvalidOperationException)
            {
                randomPoint = Vector3.zero;
            }

            // DEBUG!!!!
            if (randomPoint == Vector3.zero) Debug.LogError("Moved to a zero point");

            _agent.SetDestination(randomPoint);

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

    private Vector3 CalculateRandomPoint()
    {
        Vector3 randomPoint;
        GetRandomPoint(transform.position, 10.0f, out randomPoint);
        return randomPoint;
    }

    private bool GetRandomPoint(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}
