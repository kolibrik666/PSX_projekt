using Zenject;
using System;
public static class RandomNumGen
{
    static System.Random _random;
    public static void Init()
    {
        _random = new Random();
    }
    public static int Range(int min, int max)
    {
        Init();
        return _random.Next(min, max);
    }
    public static float Range(float minValue, float maxValue)
    {
        double num = _random.NextDouble();
        var range = maxValue - minValue + 0.1f;
        num = num * range;
        num += minValue;
        num = Math.Min(maxValue, num);
        return (float)num;
    }
}
