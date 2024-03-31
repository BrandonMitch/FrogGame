using UnityEngine;

public class GameEventListenerJustNumUpdateText : GameEventListener
{
    [Space]
    [Header("Text to update")]
    public UnityEngine.UI.Text Text;
    public FloatVariable numberToText;

    public void UpdateText()
    {
        Text.text = numberToText.value.ToString();
    }
}