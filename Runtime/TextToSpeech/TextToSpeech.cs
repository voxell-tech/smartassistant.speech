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
using System.Threading;

namespace Voxell.Speech.TTS
{
  public partial class TextToSpeech : MonoBehaviour
  {
    public AudioSource audioSource;
    public LogImportance debugLevel;

    private int sampleLength;
    private float[] audioSample;
    private AudioClip clip;
    private Thread speakThread;
    [HideInInspector] public bool playingAudio = false;
    private Logging logger;

    void Start()
    {
      InitTTSProcessor();
      InitTTSInference();
      logger = new Logging(debugLevel);
    }

    void Update()
    {
      if (playingAudio)
      {
        clip = AudioClip.Create("Speak", sampleLength, 1, 22050, false);
        clip.SetData(audioSample, 0);
        audioSource.PlayOneShot(clip);
        playingAudio = false;
      }
    }

    void OnDisable()
    {
      speakThread?.Abort();
    }

    public void Speak(string text)
    {
      speakThread = new Thread(new ParameterizedThreadStart(SpeakTask));
      speakThread.Start(text);
    }

    private void SpeakTask(object inputText)
    {
      string text = inputText as string;
      CleanText(ref text);
      int[] inputIDs = TextToSequence(text);
      float[,,] fastspeechOutput = FastspeechInference(ref inputIDs);
      float[,,] melganOutput = MelganInference(ref fastspeechOutput);

      sampleLength = melganOutput.GetLength(1);
      audioSample = new float[sampleLength];
      for (int s=0; s < sampleLength; s++) audioSample[s] = melganOutput[0, s, 0];
      playingAudio = true;
    }

    public void CleanText(ref string text)
    {
      text = text.ToLower();
      // TODO: also convert numbers to words using the NumberToWords class
    }
  }
}