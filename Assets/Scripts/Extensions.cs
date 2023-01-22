using System.Collections.Generic;
using System;

public static class Extensions
{
    public static T[] AddToArray<T>(this T[] target, T item)
    {   
        T[] result = new T[target.Length + 1];
        target.CopyTo(result, 0);
        result[target.Length] = item;
        return result;
    }


    public static T[] RemoveFromArray<T>(this T[] original, T itemToRemove)
    {
        int indexOfItem = Array.IndexOf(original, itemToRemove);

        if (indexOfItem == -1) return original;

        List<T> tmp = new List<T>(original);
        tmp.RemoveAt(indexOfItem);

        return tmp.ToArray();
    }
}
