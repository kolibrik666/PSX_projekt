using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CodeController : MonoBehaviour
{
    [SerializeField] CodePapers _codePapers;
    [SerializeField] List<Transform> _codePapersSpawnpoints = new();
    [SerializeField] List<CodeCylinder> _codeCylinders = new();
    [SerializeField] SafeDoorAnimation _doorAnimation;

    List<int> _code = new();
    int _codeCompletedCount = 0; // for some reason its get to -1 when it where 0
    private void OnEnable()
    {
        List<Transform> spawnpoints = _codePapersSpawnpoints;
        _codeCylinders.ForEach(c => 
        {
            int randomIndex = RandomNumGen.Range(0, spawnpoints.Count);
            Transform randomSpawnpoint = spawnpoints[randomIndex];
            spawnpoints.RemoveAt(randomIndex);

            var code = RandomNumGen.Range(0, 9);
            GameObject foundPaper = _codePapers.CodePapersList.Find(paper => paper.Number == code).SpawnablePrefab;

            var paper = Instantiate(foundPaper, foundPaper.transform.position, foundPaper.transform.rotation);
            paper.transform.SetParent(randomSpawnpoint.transform, false);

            if (code == 0) CodeCheck(true);
            _code.Add(code);
            c.Setup(code);
            Debug.Log(code);
        });
        CodeCylinder.OnCompleted += CodeCheck;
    }
    private void OnDisable()
    {
        CodeCylinder.OnCompleted -= CodeCheck;
    }

    private void CodeCheck(bool b)
    {
        if (b) _codeCompletedCount++;
        else _codeCompletedCount--;
        if (_codeCompletedCount >= _codeCylinders.Count) _doorAnimation.PlayAnim();
    }
}
