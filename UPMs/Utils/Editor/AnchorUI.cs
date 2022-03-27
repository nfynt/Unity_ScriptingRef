using UnityEditor;
using UnityEngine;

namespace nfynt.utils.editor
{
	public class AnchorUI : MonoBehaviour
	{
		[MenuItem("NFYNT/UI/Set Anchors To Corner %U")]
		public static void SetAnchorsToCorner()
		{
			EditorGUI.BeginChangeCheck();
			Undo.RecordObjects(Selection.transforms, "Changed UI anchors");
			foreach (Transform transform in Selection.transforms)
			{
				RectTransform t = transform as RectTransform;
				RectTransform pt = Selection.activeTransform.parent as RectTransform;

				if (t == null || pt == null) return;

				Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
					t.anchorMin.y + t.offsetMin.y / pt.rect.height);
				Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
					t.anchorMax.y + t.offsetMax.y / pt.rect.height);

				t.anchorMin = newAnchorsMin;
				t.anchorMax = newAnchorsMax;
				t.offsetMin = t.offsetMax = new Vector2(0, 0);
			}
		}
	}
}


/*
 __  _ _____   ____  _ _____  
|  \| | __\ `v' /  \| |_   _| 
| | ' | _| `. .'| | ' | | |   
|_|\__|_|   !_! |_|\__| |_|
 

*/
