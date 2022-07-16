using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class Utils
{
    public enum ShuffleType
    {
        First,
        Second
    }

    public static void Swap<T>(this T a, T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }

    public static void Swap<T>(this IList<T> list, int indexA, int indexB)
    {
        var temp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = temp;
    }

    public static IEnumerable<T> Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count - 1; ++i)
            list.Swap(i, UnityEngine.Random.Range(i, list.Count));
        return list;
    }

    public static void Shufle(this IList list, Func<int, int, int> randomFunction)
    {
        for (int i = 0; i < list.Count - 1; ++i)
            Swap(list[i], list[randomFunction(i, list.Count - 1)]);
    }

    public static void Shufle<T>(this IList<T> list, ShuffleType type = ShuffleType.First)
    {
        switch (type)
        {
            case ShuffleType.First:
                for (int i = 0; i < list.Count - 1; ++i)
                    list.Swap(i, UnityEngine.Random.Range(i, list.Count));
                break;
            case ShuffleType.Second:
                System.Random rng = new System.Random();
                int n = list.Count;

                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
                break;
        }
    }

    public static void Shufle<T>(this IList<T> list, Func<int, int, int> randomFunction)
    {
        for (int i = 0; i < list.Count - 1; ++i)
            Swap(list[i], list[randomFunction(i, list.Count - 1)]);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> input, Func<int, int, int> randomFunction = null)
    {
        var result = input.ToList();
        for (int i = 0; i < result.Count - 1; ++i)
            result.Swap(i, randomFunction(i, result.Count));
        return result;
    }

    public static T NextEnumValue<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new System.ArgumentException(System.String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])System.Enum.GetValues(src.GetType());
        int j = System.Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }

    public static void SetParentWithSiblingIndex(this Transform thisTr, Transform parent, int siblingIndex)
    {
        thisTr.SetParent(parent);
        thisTr.SetSiblingIndex(siblingIndex);
    }

    public static void SetParentWithLastSiblingIndex(this Transform thisTr, Transform parent)
    {
        thisTr.SetParent(parent);
        thisTr.SetAsLastSibling();
    }

    public static void ResetTransformLocal(this Transform thisTr, bool resetPosition = true, bool resetRotation = true, bool resetScale = true)
    {
        if (resetPosition)
        {
            thisTr.localPosition = Vector3.zero;
        }

        if (resetRotation)
        {
            thisTr.localRotation = Quaternion.identity;
        }

        if (resetScale)
        {
            thisTr.localScale = Vector3.one;
        }
    }

    public static void ResetTransformWorld(this Transform thisTr, bool resetPosition = true, bool resetRotation = true, bool resetScale = true)
    {
        if (resetPosition)
        {
            thisTr.position = Vector3.zero;
        }

        if (resetRotation)
        {
            thisTr.rotation = Quaternion.identity;
        }

        if (resetScale)
        {
            thisTr.localScale = Vector3.one;
        }
    }

    static public T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }

    static public Task Wait(float seconds)
    {
        var ms = TimeSpan.FromSeconds(seconds);
        return Task.Delay(ms);
    }

    static public Task WaitForEndOfFrame()
    {
        var ms = TimeSpan.FromSeconds(Time.deltaTime);
        return Task.Delay(ms);
    }

    public static bool GetRandomBoolean()
    {
        System.Random rand = new System.Random();
        return rand.Next(2) == 1;
    }
}