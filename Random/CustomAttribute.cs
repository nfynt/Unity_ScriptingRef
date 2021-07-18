using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Nfynt
{
    public class CustomAttribute : MonoBehaviour
    {
        [UserListPopup]
        public string UserString;
    }

    [ExecuteInEditMode]
    public class UserListPopupAttribute : PropertyAttribute
    {
        publicUserListPopupAttribute()
        {

        }
    }

    [ExecuteInEditMode]
    [CustomPropertyDrawer(typeof(UserListPopupAttribute))]
    public class UserListPopupDrawer : PropertyDrawer
    {
        private static List<string> sources;

        private void OnEnable()
        {
            UpdateList();
        }

        private static void UpdateList()
        {
            sources = new List<string>(){ "one", "two", "three" };
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            UserListPopupAttribute atb = attribute as UserListPopupAttribute;

            if (sources != null && sources.Count > 0)
            {
                int index = Mathf.Max(sources.IndexOf(property.stringValue), 0);
                index = EditorGUI.Popup(position, property.name, index, sources.ToArray());
                property.stringValue = sources[index];
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}