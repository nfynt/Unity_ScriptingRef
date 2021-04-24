using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GameObjectExtensions {


    public static void Trigger(this GameObject self, UnityEvent evt)
    {
        if (evt != null)
        {
            evt.Invoke();
        }
        else
        {
            Debug.LogWarning("Tried to invoke a null UnityEvent");
        }
    }

    /// <summary>
    /// Safely invoke a UnityEvent
    /// </summary>
    /// <typeparam name="T">The type of the UnityEvent</typeparam>
    /// <param name="self"></param>
    /// <param name="evt">The event</param>
    /// <param name="data">The payload for the event</param>
    public static void Trigger<T>(this GameObject self, UnityEvent<T> evt, T data)
    {
        if (evt != null)
        {
            evt.Invoke(data);
        }
        else
        {
            Debug.LogWarning("Tried to invoke a null UnityEvent on " + self.name + " with type '" + typeof(T).ToString() + "' with the follwing payload: " + data.ToString());
        }
    }

    /// <summary>
    /// Get a component, log an error if it's not there
    /// </summary>
    /// <typeparam name="T">The type of component to get</typeparam>
    /// <param name="self"></param>
    /// <returns>The component, if found</returns>
    public static T GetComponentRequired<T>(this GameObject self) where T : Component
    {
        T component = self.GetComponent<T>();

        if (component == null) Debug.LogError("Could not find " + typeof(T) + " on " + self.name);

        return component;
    }

    /// <summary>
    /// Perform an action if a component exists, skip otherwise
    /// </summary>
    /// <typeparam name="T">The type of component required</typeparam>
    /// <param name="self"></param>
    /// <param name="callback">The action to take</param>
    /// <returns>The component found</returns>
    public static T GetComponent<T>(this GameObject self, System.Action<T> callback) where T : Component
    {
        var component = self.GetComponent<T>();

        if (component != null)
        {
            callback.Invoke(component);
        }

        return component;
    }

    /// <summary>
    /// Take an action only if a component exists, error if it's not there
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="callback"></param>
    /// <returns>The component, if found</returns>
    public static T GetComponentRequired<T>(this GameObject self, System.Action<T> callback) where T : Component
    {
        var component = self.GetComponentRequired<T>();

        if (component != null)
        {
            callback.Invoke(component);
        }

        return component;
    }

    /// <summary>
    /// Get a component, take a different action if it isn't there
    /// </summary>
    /// <typeparam name="T">Component Type</typeparam>
    /// <param name="self">object being extended</param>
    /// <param name="success">Take this action if the component exists</param>
    /// <param name="failure">Take this action if the component does not exist</param>
    /// <returns></returns>
    public static T GetComponent<T>(this GameObject self, System.Action<T> success, System.Action failure) where T : Component
    {
        var component = self.GetComponent<T>();

        if (component != null)
        {
            success.Invoke(component);
            return component;
        }
        else
        {
            failure.Invoke();
            return null;
        }
    }
}
