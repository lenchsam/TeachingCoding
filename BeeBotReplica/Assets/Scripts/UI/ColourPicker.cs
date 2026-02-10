using UnityEngine;
using UnityEngine.UI;

public class ColourPicker : MonoBehaviour
{
    public Slider red;
    public Slider green;
    public Slider blue;

    public Image preview;

    void Start()
    {
        red.onValueChanged.AddListener(UpdateColour);
        green.onValueChanged.AddListener(UpdateColour);
        blue.onValueChanged.AddListener(UpdateColour);

        UpdateColour();
    }

    void UpdateColour(float value = 0)
    {
        Color c = new Color(red.value / 255, green.value / 255, blue.value / 255);
        preview.color = c;
    }

    public Color GetColour()
    {
        return preview.color;
    }
}
