using System;
using UnityEngine;
using Zenject;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] GameObject _navMeshVisualizer;

    private KeyCode _altKey = KeyCode.Tab;
    private bool _meshVisualize;
    [Inject] GameStartData _gameStartData;
    [Inject] GameRunData _gameRunData;

    public static event Action OnValueChange;
    public void Setup()
    {
        _meshVisualize = false;
    }
    private void OnDestroy()
    {
        
    }
    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.V) && Input.GetKeyDown(_altKey))
        {
            _navMeshVisualizer.SetActive(_meshVisualize);
            _meshVisualize = !_meshVisualize;
        }
        if (Input.GetKey(KeyCode.G) && Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            _gameRunData.Sanity -= 10;
            OnValueChange?.Invoke();
        }
        if (Input.GetKey(KeyCode.G) && Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            _gameRunData.Sanity += 10;
            OnValueChange?.Invoke();

        }
        if (Input.GetKey(KeyCode.H) && Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            _gameRunData.Saturation -= 10;
            OnValueChange?.Invoke();

        }
        if (Input.GetKey(KeyCode.H) && Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            _gameRunData.Saturation += 10;
            OnValueChange?.Invoke();
        }

    }

}
