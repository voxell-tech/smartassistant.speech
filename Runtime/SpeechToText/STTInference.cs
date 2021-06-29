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

using TensorFlowLite;
using System;
using Voxell.Mathx;
using Voxell.DeepLearning;

namespace Voxell.Speech.STT
{
  public partial class SpeechToText
  {
    public TFLiteAsset deepspeech;

    private Interpreter _deepspeechInterpreter;
    private InterpreterOptions _options;

    private const int inferenceSize = 512;

    /// <summary>
    /// Create Deepspeech interpreter
    /// </summary>
    void InitSTTInference()
    {
      _options = new InterpreterOptions() {threads = 4};
      _deepspeechInterpreter = new Interpreter(FileUtil.ReadAssetFileByte(deepspeech), _options);
    }

    #region Inferencing

    private Array[] PrepareInput(ref float[] inputStream)
    {
      Array[] inputData = new Array[3];

      inputData[0] = inputStream;

      return inputData;
    }

    private char[] DeepspeechInference(ref float[] inputStream)
    {
      _deepspeechInterpreter.ResizeInputTensor(0, new int[1]{inputStream.Length});

      _deepspeechInterpreter.AllocateTensors();
      Array[] inputData = PrepareInput(ref inputStream);

      for (int d=0; d < inputData.Length; d++)
        _deepspeechInterpreter.SetInputTensorData(d, inputData[d]);

      _deepspeechInterpreter.Invoke();

      int[] outputShape = _deepspeechInterpreter.GetOutputTensorInfo(0).shape;
      char[] outputData = new char[outputShape[0]];
      _deepspeechInterpreter.GetOutputTensorData(0, outputData);
      return outputData;
    }

    private float[][] SplitStream(ref float[] inputStream)
    {
      int totalSplits = MathUtil.CalculateGrids(inputStream.Length, inferenceSize);

      float[][] streams = new float[totalSplits][];

      // for ()

      return streams;
    }
    #endregion
  }
}