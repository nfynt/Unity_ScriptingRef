using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions {

    /// <summary>
    /// For each component in an array, take an action
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="callback">The action to take</param>
    public static void ForEachComponent<T>(this T[] array, System.Action<T> callback) where T : Component
    {
        for (var i = 0; i < array.Length; i++)
        {
            callback.Invoke(array[i]);
        }
    }
}
