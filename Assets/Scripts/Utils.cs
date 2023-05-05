using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static System.Random random = new System.Random();

    public static string FloatToTimeMillisecondsFormat(float timeInput)
    {
        double timePlayedDouble = (double)timeInput;

        System.TimeSpan time = System.TimeSpan.FromSeconds(timePlayedDouble);

        string displayTime = time.ToString("mm\\:ss\\.fff");

        return displayTime;
    }

    public static string FloatToTimeSecondsFormat(float timeInput)
    {
        double timePlayedDouble = (double)timeInput;

        System.TimeSpan time = System.TimeSpan.FromSeconds(timePlayedDouble);

        string displayTime = time.ToString("mm\\:ss");

        return displayTime;
    }

    public static Dictionary<string, string> ToDictionary(object obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        return dictionary;
    }

    public static bool IsSubclassOfRawGeneric(Type toCheck, Type generic)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }

            toCheck = toCheck.BaseType;
        }

        return false;
    }

    public static Item GetItemPrefabFromName(string itemSOName, ItemType itemType)
    {
        GameObject instantiatedPrefab = GameObject.Instantiate(Resources.Load("Prefabs/Item Prefabs/" + itemType.ToString() + "/" + itemSOName)) as GameObject;

        instantiatedPrefab.TryGetComponent(out Item item);

        return item;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
