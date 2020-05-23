using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Matrix4x4))]
public class Matrix4x4PropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{

		EditorGUI.BeginProperty(position, label, property);
		EditorGUI.LabelField(position, label);

		SerializedProperty[,] fields = new SerializedProperty[4, 4];
		for (int j = 0; j < 4; j++)
		{
			for (int i = 0; i < 4; i++)
			{
				fields[j, i] = property.FindPropertyRelative("e" + i + j);
			}
		}
		Rect[,] rects = new Rect[4, 4];
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{

				float size = position.width / 2 + 25 - 85 + 85 * (258 / position.width);
				float offset = 24 + 36 * (position.width / 1349);
				//                rects [i, j] =    new Rect (position.center - new Vector2 (position.center.x - 10f - i * offset, position.center.y / 8 - j * 20 - 8), new Vector2 (size, 15));    
				rects[i, j] = new Rect(position.center - new Vector2(position.center.x - 10f - i * offset, 30 - j * 20 - 8), new Vector2(size, 15));

			}
		}
		for (int j = 0; j < 3; j++)
		{
			EditorGUI.DrawRect(new Rect(rects[0, j].min, new Vector2(23, 15)), Color.red);
		}
		for (int j = 0; j < 3; j++)
		{
			EditorGUI.DrawRect(new Rect(rects[1, j].min, new Vector2(23, 15)), Color.green);
		}
		for (int j = 0; j < 3; j++)
		{
			EditorGUI.DrawRect(new Rect(rects[2, j].min, new Vector2(23, 15)), Color.blue);
		}

		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				fields[i, j].floatValue = (float)System.Math.Round((float)fields[i, j].floatValue, 2);
				EditorGUI.PropertyField(rects[i, j], fields[i, j]);
			}
		}
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property, label) + 100;
	}
}
