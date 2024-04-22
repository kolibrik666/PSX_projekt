using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranslateText : MonoBehaviour
{
    [SerializeField] string _textId;

    TextMeshProUGUI _text;
    Text _classicText;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _classicText = GetComponent<Text>();
        Translate();
    }

    private void OnEnable()
    {
        Translate();
        TextDatabase.OnCurrentLanguageChanged += Translate;
    }

    private void OnDisable()
    {
        TextDatabase.OnCurrentLanguageChanged -= Translate;
    }

    private void Translate()
    {
        if (_text) _text.text = TextDatabase.Translate(_textId);
        if (_classicText) _classicText.text = TextDatabase.Translate(_textId);
    }
}
