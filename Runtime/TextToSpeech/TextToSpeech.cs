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
using TensorFlowLite;
using System;

namespace SmartAssistant.Speech.TTS
{
  public partial class TextToSpeech : MonoBehaviour
  {
    void Start()
    {
      InitTTSProcessor();
      InitTTSInference();
    }

    public void Speak(string text)
    {
      // TODO: clean text first!!! (e.g. convert numbers to words)
      float[,,] fastspeechOutput = FastspeechInference(ref text);
      float[,,] melganOutput = MelganInference(ref fastspeechOutput);

      print(melganOutput);
    }
  }
}