using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnablePuzzle", order = 1)]
public class SpawnablePuzzle : SpawnableGeneral
{
    public List<SpawnablePuzzle> SpawnablesPartsList = new();
    public int ChallengesToComplete;
}
