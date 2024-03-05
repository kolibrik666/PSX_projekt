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
        Transform randomSpawnPoint = spawnPoints[RandomNumGen.Range(0, spawnPoints.Length)];
        var target = Instantiate(_targetPrefab, _targetPrefab.transform.position, _targetPrefab.transform.rotation);
        target.transform.SetParent(randomSpawnPoint.transform, false);

        Vector3 targetDirection = target.transform.position - transform.position;
        _savedRandomDirection = targetDirection.normalized;
    }
    public void ChangeControl(bool puzzleDone = false)
    {
        if (puzzleDone)
        {
            OnChangeControl?.Invoke();
            _spotlightController.enabled = false;
            return;
        }

        OnChangeControl?.Invoke();
        _controlSpotlight = !_controlSpotlight;
        _light.SetActive(_controlSpotlight);
        _spotlightController.enabled = _controlSpotlight;
    }
    public override void OnFocus()
    {
        //animacia a zobrazenie staète tlaèidlo
    }

    public override void OnInteract()
    {   
        if(!_spotlightController.PuzzleCompleted) ChangeControl(); 
    }

    public override void OnLoseFocus()
    {
        //throw new NotImplementedException();
    }
}
