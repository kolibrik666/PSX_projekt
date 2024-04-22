using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(ButtonHintExtended))]
[CanEditMultipleObjects]
public class ButtonHintExtendedEditor : ButtonEditor
{
    SerializedProperty _hint;
    SerializedProperty _hintText;

    protected override void OnEnable()
    {
        base.OnEnable();
        _hint = serializedObject.FindProperty("_hint");
        _hintText = serializedObject.FindProperty("_hintText");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();

        EditorGUILayout.PropertyField(_hint);
        EditorGUILayout.PropertyField(_hintText);
        serializedObject.ApplyModifiedProperties();
    }
}
