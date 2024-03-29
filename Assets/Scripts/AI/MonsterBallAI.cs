using UnityEngine;
using UnityEngine.AI;
using Zenject;
using static Extensions;

public class MonsterBallAI : GenericAI
{
    [Inject] AudioManager _audioManager;
    [SerializeField] Sound _soundHeartBeat;

    float _wanderRadius = 18f;
    float _wanderTime = 5f;
    float _reach = 14f;
    float _timer;
    float _heartBeatTimer = 0;
    float _interval = 0.3f;

    int _wonderMaxCount = 3;
    int _wonderCount = 0;

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        NavMeshPath path = new NavMeshPath();

        if (distanceToPlayer < _killReach && _player.IsAlive) _survivalManager.CheckLife(true); 
        if (distanceToPlayer > _reach || !_player.IsAlive) _state = AIState.Wander;
        else _state = AIState.Follow;

        float distanceRatio = Mathf.Clamp01(distanceToPlayer / _reach);
        if (distanceToPlayer < _reach / 1.5 && _player.IsAlive)
        {
            _heartBeatTimer += Time.unscaledDeltaTime;
            if (_heartBeatTimer >= (distanceRatio < _interval ? _interval : distanceRatio))
            {
                _audioManager.PlayOneShot(_soundHeartBeat);
                _heartBeatTimer = 0;
            }
        }

        _agent.CalculatePath(_player.transform.position, path);
        if (path.status == NavMeshPathStatus.PathComplete && _state == AIState.Follow) _survivalManager.SetChaseState(true);
        else
        {
            _survivalManager.SetChaseState(false);
            if(path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid) _state = AIState.Wander;
        }

        switch (_state)
        {
            case AIState.Wander:
                Wander();
                break;
            case AIState.Follow:                
                Reset();
                _agent.destination = _player.transform.position;
                break;
        }
    }
    void Wander()
    {
        _timer += Time.unscaledDeltaTime;

        if (_timer >= _wanderTime && _wonderCount <= _wonderMaxCount)
        {
            Vector3 newPos = RandomNavSphere(transform.position, _wanderRadius, -1);
            _agent.SetDestination(newPos);
            _timer = 0f;
            _wonderCount++;
        }
        else if(_wonderCount > _wonderMaxCount)
        {
            _agent.SetDestination(_initialPos.position);
            if (Vector3.Distance(transform.position, _initialPos.position) < 2) Reset(); 
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
