using UnityEngine;
using UnityEngine.AI;
using static Extensions;

public class MonsterBallAI : GenericAI
{
    float _wanderRadius = 18f;
    float _wanderTimer = 5f;
    float _reach = 14f;
    float _timer;

    int _wonderMaxCount = 3;
    int _wonderCount = 0;

    protected void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        if (distanceToPlayer < _killReach && _player.IsAlive)
        {
            _survivalManager.CheckLife(true);
        }
        if (distanceToPlayer > _reach || !_player.IsAlive)
        {
            Wander();
        }
        else
        {
            Reset();
            _agent.destination = _player.transform.position;
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
    protected override void UpdateReach(bool b)
    {
        if (b) _reach = _reach - 6;
        else _reach = _reach * 2;
    }
    void Reset()
    {
        _wonderCount = 0;
        _timer = 0f;
    }
    Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = GenerateRandomVectorInsideUnitSphere() * distance;
        randomDirection += origin;
        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, distance, layermask);
        return navHit.position;
    }

}
