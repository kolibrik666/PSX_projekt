using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{   
    private int _challengesCompleted = 0;
    private SpawnablePuzzle _spawnablePuzzle;

    public event Action OnPuzzleCompleted;

    //
    public void Setup(SpawnablePuzzle spawnablePuzzle)
    {
        _spawnablePuzzle = spawnablePuzzle;
    }
    public void PuzzleCompleted()
    {
        _challengesCompleted++;
        OnPuzzleCompleted?.Invoke();
    }
}
