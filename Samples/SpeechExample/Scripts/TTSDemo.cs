using TMPro;
using UnityEngine;


namespace SmartAssistant.Speech.TTS.Demo
{
  public class TTSDemo : MonoBehaviour
  {
    public TextMeshProUGUI inputField;
    public TextToSpeech textToSpeech;

    public void SpeakInput() => textToSpeech.Speak(inputField.text);
  }
}