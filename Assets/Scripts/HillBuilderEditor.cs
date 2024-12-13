using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HillBuilder))]
public class HillBuilderEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HillBuilder _target = (HillBuilder)target;
        if (_target == null) return;
        Undo.RecordObject(_target, "Hill Builder Undo");
        if (GUILayout.Button("Generate"))
        {
            _target.GeneratePoints();
        }
    }

}
