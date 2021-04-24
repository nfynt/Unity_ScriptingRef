using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachedComponent<T> where T : UnityEngine.Object {
    private T cache;

    public void clear()
    {
        cache = null;
    }

    // Get the cached version of T, or look for it if no cached version exists
    public T instance(MonoBehaviour container)
    {
        if (cache == null)
        {
            var possibles = GameObject.FindObjectsOfType<T>();
            if (possibles.Length == 0)
            {
                Debug.LogError("No '" + typeof(T).Name + "' found when " + container.name + " searched for it.");
            }
            else
            {
                cache = possibles[0];
                if (possibles.Length > 1)
                {
                    Debug.LogWarning("Multiple '" + typeof(T).Name + "' found when " + container.name + " searched. Using the first one.");
                }
            }
        }

        return cache;
    }
}
