using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RangeInt), true)]
[CustomPropertyDrawer(typeof(RangeFloat), true)]
[CustomPropertyDrawer(typeof(RangeColor), true)]
public class RangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var labelWidth = 30;
        var maxElements = 2;
        var margin = 5;
        var labels = 0;
        var fields = 0;

        var widthPer = Mathf.Max(1, (position.width / maxElements) - labelWidth);

        // Equally spaced elements
        var minLbl = new Rect(position.x + (labelWidth * labels++) + (widthPer * fields), position.y, labelWidth, position.height);
        var minRct = new Rect(position.x + (labelWidth * labels) + (widthPer * fields++), position.y, widthPer - margin, position.height);
        var maxLbl = new Rect(position.x + (labelWidth * labels++) + (widthPer * fields), position.y, labelWidth, position.height);
        var maxRct = new Rect(position.x + (labelWidth * labels) + (widthPer * fields++), position.y, widthPer - margin, position.height);

        EditorGUI.LabelField(minLbl, "Min");
        EditorGUI.PropertyField(minRct, property.FindPropertyRelative("Min"), GUIContent.none);
        EditorGUI.LabelField(maxLbl, "Max");
        EditorGUI.PropertyField(maxRct, property.FindPropertyRelative("Max"), GUIContent.none);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
