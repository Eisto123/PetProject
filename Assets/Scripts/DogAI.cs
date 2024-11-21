using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogAI : MonoBehaviour
{
    [SerializeField] private Transform target;

    private NavMeshAgent _agent;

    private float _recalcPathTime = 0.5f;
    private float _recalcPathTimer = 0.0f;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        // _agent.SetDestination(target.position);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed");
            _agent.SetDestination(target.position);
        }

        _recalcPathTimer += Time.deltaTime;
        if (_recalcPathTimer >= _recalcPathTime)
        {
            _recalcPathTimer = 0.0f;
            _agent.SetDestination(target.position);
        }
    }
}
