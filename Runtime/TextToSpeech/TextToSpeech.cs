/*
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.

The Original Code is Copyright (C) 2020 Voxell Technologies.
All rights reserved.
*/

using UnityEngine;
using System.Threading.Tasks;

namespace SmartAssistant.Speech.TTS
{
  public partial class TextToSpeech : MonoBehaviour
  {
    public AudioSource audioSource;

    void Start()
    {
      InitTTSProcessor();
      InitTTSInference();

      Speak("Hello, my name is Vox and I am a smart assistant!");
    }

    public void Speak(string text)
    {
      // TODO: clean text first!!! (e.g. convert numbers to words)
      CleanText(ref text);
      float[,,] fastspeechOutput = FastspeechInference(ref text);
      float[,,] melganOutput = MelganInference(ref fastspeechOutput);

      int sampleLength = melganOutput.GetLength(1);
      float[] audioSample = new float[sampleLength];
      for (int s=0; s < sampleLength; s++) audioSample[s] = melganOutput[0, s, 0];

      AudioClip clip = AudioClip.Create("Speak", sampleLength, 1, 22050, false);
      clip.SetData(audioSample, 0);
      audioSource.PlayOneShot(clip);
    }

    public void CleanText(ref string text)
    {
      text = text.ToLower();
      // TODO: also convert numbers to words using the NumberToWords class
    }
  }
}