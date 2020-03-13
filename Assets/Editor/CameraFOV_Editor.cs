using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CameraController))]
public class GroundEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CameraController camera = (CameraController)target;

        Camera cam = camera.GetComponentInChildren<Camera>();
        GUILayout.Space(15f);
        GUILayout.Label("Can be change in editor");
        cam.fieldOfView = EditorGUILayout.Slider("Camera FOV", cam.fieldOfView, 50f, 110f);
        cam.fieldOfView = cam.fieldOfView;
    }
}
