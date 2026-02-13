using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    [SerializeField] private ColourSettings _colourSettingsSO;
    [SerializeField] private Image _inputFieldBackground;
    [SerializeField] private TextMeshProUGUI _inputFieldText;

    private void Awake()
    {
        _inputFieldBackground.color = _colourSettingsSO.BackgroundColour;
        _inputFieldText.color = _colourSettingsSO.TextColour;
    }
}
