using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(TextToggle))]
[CanEditMultipleObjects]
public class TextToggleEditor : ToggleEditor
{
    SerializedProperty _text;
    SerializedProperty _useCustomColors;
    SerializedProperty _colors;

    bool _useCustomColorsSelected;

    protected override void OnEnable()
    {
        base.OnEnable();
        _text = serializedObject.FindProperty("_text");
        _useCustomColors = serializedObject.FindProperty("_useCustomColors");
        _colors = serializedObject.FindProperty("_colors");

        _useCustomColorsSelected = _useCustomColors.boolValue;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();

        EditorGUILayout.PropertyField(_text);
        _useCustomColorsSelected = EditorGUILayout.Toggle("Use Custom Colors", _useCustomColorsSelected);
        _useCustomColors.boolValue = _useCustomColorsSelected;
        if (_useCustomColorsSelected) EditorGUILayout.PropertyField(_colors);
        serializedObject.ApplyModifiedProperties();
    }
}
