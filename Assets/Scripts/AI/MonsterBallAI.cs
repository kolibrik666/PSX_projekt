using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBallAI : MonoBehaviour
{
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] Transform _player;
    [SerializeField] Transform _initialPos;

    float _wanderRadius = 18f;
    float _wanderTimer = 5f;
    int _wonderMaxCount = 3;
    int _wonderCount = 0;

    float _reach = 10f;
    float _timer;
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer > _reach)
        {
            Wander();
        }
        else
        {
            Reset();
            _agent.destination = _player.position;
        }
    }
    void Wander()
    {
        _timer += Time.unscaledDeltaTime;

        if (_timer >= _wanderTimer && _wonderCount <= _wonderMaxCount)
        {
            Vector3 newPos = RandomNavSphere(transform.position, _wanderRadius, -1);
            _agent.SetDestination(newPos);
            _timer = 0f;
            _wonderCount++;
        }
        else if(_wonderCount > _wonderMaxCount)
        {
            _agent.SetDestination(_initialPos.position);
            if (Vector3.Distance(transform.position, _initialPos.position) < 2)
            {
                Reset();
            }
        }
    }
    private void Reset()
    {
        _wonderCount = 0;
        _timer = 0f;
    }
    Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, distance, layermask);

        return navHit.position;
    }
}
