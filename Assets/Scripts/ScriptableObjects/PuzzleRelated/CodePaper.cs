using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CodePaper", order = 1)]
public class CodePaper : ScriptableObject
{
    public GameObject SpawnablePrefab;
    public int Number;
}
