
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
    public static ListItem<T> RemoveRandomItemFromList<T>(this List<T> list)
    {
        if (list == null || list.Count == 0) return default;

        int randomIndex = RandomNumGen.Random(0, list.Count);
        T item = list[randomIndex];

        List<T> updatedTunelList = new List<T>(list);
        updatedTunelList.RemoveAt(randomIndex);

        return new ListItem<T> { SpawnablesList = updatedTunelList, SpawnableGeneral = item };
    }

    public static T RemoveRandomItem<T>(this List<T> list)
    {
        if (list == null || list.Count == 0) return default;

        int randomIndex = RandomNumGen.Random(0, list.Count);
        T item = list[randomIndex];

        list.RemoveAt(randomIndex);

        return item;
    }
}
