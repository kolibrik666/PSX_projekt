using System.Text.RegularExpressions;
using System;
using UnityEngine;
using Unity.VisualScripting;
using Unity.Mathematics;

public static class RandomNumGen
{
    static System.Random _random;
    public static void Init()
    {
        _random = new System.Random();
    }
    public static int Random(int min, int max)
    {
        Init();
        return _random.Next(min, max);
    }
}
