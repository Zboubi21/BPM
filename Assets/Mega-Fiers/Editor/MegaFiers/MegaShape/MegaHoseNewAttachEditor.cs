
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaHoseNewAttach))]
public class MegaHoseNewAttachEditor : Editor
{
	public override void OnInspectorGUI()
	{
		MegaHoseNewAttach mod = (MegaHoseNewAttach)target;

#if !UNITY_5 && !UNITY_2017 && !UNITY_2018 && !UNITY_2019
		EditorGUIUtility.LookLikeControls();
#endif

		mod.hose = (MegaHoseNew)EditorGUILayout.ObjectField("Hose", mod.hose, typeof(MegaHoseNew), true);
		mod.alpha = EditorGUILayout.FloatField("Alpha", mod.alpha);
		mod.offset = EditorGUILayout.Vector3Field("Offset", mod.offset);

		mod.rot = EditorGUILayout.BeginToggleGroup("Rot On", mod.rot);
		mod.rotate = EditorGUILayout.Vector3Field("Rotate", mod.rotate);
		EditorGUILayout.EndToggleGroup();
		mod.doLateUpdate = EditorGUILayout.Toggle("Late Update", mod.doLateUpdate);

		if ( GUI.changed )	//rebuild )
		{
			EditorUtility.SetDirty(mod);
		}
	}
}