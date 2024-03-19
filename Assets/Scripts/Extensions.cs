
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static CreateRandom;


namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

public static class Extensions
{
    public static T GetRandomItemFromList<T>(this List<T> list)
    {
        if (list == null || list.Count == 0) return default;

        int randomIndex = RandomNumGen.Range(0, list.Count);
        T item = list[randomIndex];

        return item;
    }
    public static ListItem<T> RemoveRandomItemFromList<T>(this List<T> list)
    {
        if (list == null || list.Count == 0) return default;

        int randomIndex = RandomNumGen.Range(0, list.Count);
        T item = list[randomIndex];

        List<T> updatedList = new List<T>(list);
        updatedList.RemoveAt(randomIndex);

        return new ListItem<T> { SpawnablesList = updatedList, Spawnable = item };
    }

    public static T RemoveRandomItem<T>(this List<T> list)
    {
        if (list == null || list.Count == 0) return default;

        int randomIndex = RandomNumGen.Range(0, list.Count);
        T item = list[randomIndex];

        list.RemoveAt(randomIndex);

        return item;
    }
    public static Vector3 GenerateRandomVectorInsideUnitSphere()
    {
        float x = RandomNumGen.Range(-1f, 1f);
        float y = RandomNumGen.Range(-1f, 1f);
        float z = RandomNumGen.Range(-1f, 1f);

        Vector3 randomVector = new Vector3(x, y, z).normalized;
        return randomVector;
    }
    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}
