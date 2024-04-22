using UnityEditor;

[CustomEditor(typeof(TextButton))]
[CanEditMultipleObjects]
public class TextButtonEditor : ButtonHintExtendedEditor
{
    //SerializedProperty _transform;
    SerializedProperty _text;
    SerializedProperty _textOther;
    SerializedProperty _useCustomColors;
    SerializedProperty _colors;

    bool _useCustomColorsSelected;

    protected override void OnEnable()
    {
        base.OnEnable();
        //_transform = serializedObject.FindProperty("_transform");
        _text = serializedObject.FindProperty("_text");
        _textOther = serializedObject.FindProperty("_textOther");

        _useCustomColors = serializedObject.FindProperty("_useCustomColors");
        _colors = serializedObject.FindProperty("_colors");

        _useCustomColorsSelected = _useCustomColors.boolValue;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();

        //EditorGUILayout.PropertyField(_transform);
        EditorGUILayout.PropertyField(_text);
        EditorGUILayout.PropertyField(_textOther);

        _useCustomColorsSelected = EditorGUILayout.Toggle("Use Custom Colors", _useCustomColorsSelected);
        _useCustomColors.boolValue = _useCustomColorsSelected;
        if (_useCustomColorsSelected) EditorGUILayout.PropertyField(_colors);
        serializedObject.ApplyModifiedProperties();
    }
}
