using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    //protected int _challengesToComplete = 0;
    //protected int _challengesCompleted = 0;
    protected List<GameObject> _spawnablePuzzleList = new();
    bool _isExitUnlocked = false;

    public event Action OnPuzzleCompleted;
    private void OnEnable()
    {
        KeyExit.OnKeyPickedUp += UnlockExit;
    }

    private void OnDisable()
    {
        KeyExit.OnKeyPickedUp -= UnlockExit;
    }

    private void UnlockExit()
    {
        _isExitUnlocked = true;
    }

    public void Setup(List<GameObject> list)
    {
        _spawnablePuzzleList = list;
        foreach(var puzzle in _spawnablePuzzleList)
        {
            //puzzle.GetComponent<>
        }
    }
    /*
    public void PuzzleCompleted()
    {
        _challengesCompleted++;
        OnPuzzleCompleted?.Invoke();
    }
    */
}
