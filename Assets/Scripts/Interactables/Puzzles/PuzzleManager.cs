using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    private int _challengesToComplete = 0;
    private int _challengesCompleted = 0;
    private List<SpawnablePuzzle> _spawnablePuzzleList = new();

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
