using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CodeController : MonoBehaviour
{
    //[SerializeField] List<GameObject> _codeCylinderObjs = new();
    [SerializeField] List<CodeCylinder> _codeCylinders = new();
    List<int> _code = new();
    int _codeCompletedCount = 0;
    public static event Action CodeCompleted;
    private void OnEnable()
    {
        _codeCylinders.ForEach(c => 
        {
            var code = RandomNumGen.Range(0, 9);
            Debug.Log(code);
            _code.Add(code);
            c.Setup(code);
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
        if (_codeCompletedCount >= _codeCylinders.Count) CodeCompleted?.Invoke();
    }
}
