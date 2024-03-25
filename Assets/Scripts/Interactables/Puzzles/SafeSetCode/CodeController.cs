using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CodeController : MonoBehaviour
{
    //[SerializeField] List<GameObject> _codeCylinderObjs = new();
    [SerializeField] List<CodeCylinder> _codeCylinders = new();
    [SerializeField] SafeDoorAnimation _doorAnimation;
    List<int> _code = new();
    int _codeCompletedCount = 0; // for some reason its get to -1 when it where 0
    private void OnEnable()
    {
        _codeCylinders.ForEach(c => 
        {
            var code = RandomNumGen.Range(0, 9);
            Debug.Log(code);
            if (code == 0) CodeCheck(true);
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
        if (_codeCompletedCount >= _codeCylinders.Count) _doorAnimation.PlayAnim();
    }
}
