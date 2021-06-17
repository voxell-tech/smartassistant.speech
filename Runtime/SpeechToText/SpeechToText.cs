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

namespace SmartAssistant.Speech.STT
{
  public partial class SpeechToText : MonoBehaviour
  {
    public AudioClip audioClip;

    private Thread recognizeThread;
    [HideInInspector] public bool recognized = false;
    [HideInInspector] public string recognizedWords;

    void Start()
    {
      InitSTTInference();
      print(audioClip.samples);
      print(MathUtil.CalculateGrids(audioClip.samples, inferenceSize));
      // Recognize(audioClip);
    }

    void Update()
    {
      if (recognized)
      {
        print(recognizedWords);
        recognized = false;
      }
    }

    void OnDisable()
    {
      recognizeThread?.Abort();
    }

    public void Recognize(AudioClip clip)
    {
      float[] clipData = new float[clip.samples];
      clip.GetData(clipData, 0);
      recognizeThread = new Thread(new ParameterizedThreadStart(RecognizeTask));
      recognizeThread.Start(clipData);
    }

    private void RecognizeTask(object clipData)
    {
      float[] inputStream = (float[])clipData;
      char[] conformerOutput = DeepspeechInference(ref inputStream);
      recognizedWords = conformerOutput.ToString();
      recognized = true;
    }
  }
}