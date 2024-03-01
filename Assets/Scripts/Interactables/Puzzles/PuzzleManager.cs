using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    protected int _challengesToComplete = 0;
    protected int _challengesCompleted = 0;
    protected List<SpawnablePuzzle> _spawnablePuzzleList = new();

    public event Action OnPuzzleCompleted;
   
    public void Setup(List<SpawnablePuzzle> list)
    {
        _spawnablePuzzleList = list;
        foreach(var puzzle in _spawnablePuzzleList)
        {
            _challengesToComplete += puzzle.ChallengesToComplete;
        }
        Debug.Log(_challengesToComplete);
    }
    public void PuzzleCompleted()
    {
        _challengesCompleted++;
        OnPuzzleCompleted?.Invoke();
    }
}
