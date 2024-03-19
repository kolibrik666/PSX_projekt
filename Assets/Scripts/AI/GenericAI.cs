using System;
using UnityEngine;
using UnityEngine.AI;

public class GenericAI : MonoBehaviour
{
    [SerializeField] protected NavMeshAgent _agent;
    [SerializeField] protected FirstPersonController _player;
    [SerializeField] protected Transform _initialPos;
    [SerializeField] protected SurvivalManager _survivalManager;

    protected float _killReach = 1.7f;
    protected AIState _state;
    private void OnEnable()
    {
        FirstPersonController.IsCrouching += UpdateReach;
    }
    private void OnDisable()
    {
        FirstPersonController.IsCrouching -= UpdateReach;
    }
    protected virtual void UpdateReach(bool b){ }
}
