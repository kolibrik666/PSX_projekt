using System;
using UnityEngine;

public class SetSpotlight : Interactable
{
    [SerializeField] SpotlightController _spotlightController;
    [SerializeField] GameObject _targetPrefab;
    [SerializeField] GameObject _light;
    [SerializeField] Transform[] spawnPoints;

    bool _controlSpotlight = false;
    Vector3 _savedRandomDirection;
    public Vector3 SavedRandomDirection => _savedRandomDirection;

    public static event Action OnChangeControl;

    private void OnEnable()
    {
        _spotlightController.enabled = false;
        SpawnTarget();
    }

    private void SpawnTarget()
    {
        Transform randomSpawnPoint = spawnPoints[RandomNumGen.Random(0, spawnPoints.Length)];
        var target = Instantiate(_targetPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
        Vector3 targetDirection = target.transform.position - transform.position;
        _savedRandomDirection = targetDirection.normalized;
    }
    private void ChangeControl()
    {
        if (_controlSpotlight)
        {
            _light.SetActive(false);
            _spotlightController.enabled = false;
            _controlSpotlight = false;
        }
        else
        {
            _light.SetActive(true);
            _spotlightController.enabled = true;
            _controlSpotlight = true;
        }
    }
    public override void OnFocus()
    {
        //animacia a zobrazenie staète tlaèidlo
    }

    public override void OnInteract()
    {
        print("INTERACTED WITH " + gameObject.name);
        OnChangeControl?.Invoke();
        ChangeControl(); 
    }

    public override void OnLoseFocus()
    {
        //throw new NotImplementedException();
    }
}
