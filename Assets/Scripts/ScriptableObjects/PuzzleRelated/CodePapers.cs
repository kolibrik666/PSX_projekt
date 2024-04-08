using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Lists/CodePapers", order = 1)]

public class CodePapers : ScriptableObject
{
    public List<CodePaper> CodePapersList = new List<CodePaper>();

}
