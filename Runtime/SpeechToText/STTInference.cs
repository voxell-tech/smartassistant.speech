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

namespace SmartAssistant.Speech.STT
{
  public partial class SpeechToText
  {
    public string conformerFilepath;

    private const int numRNNs = 1;
    private const int nStates = 2;
    private const int stateSize = 320;
    private int[,,,] states = new int[numRNNs, nStates, 1, stateSize];
    private Interpreter _conformerInterpreter;
    private InterpreterOptions _options;

    /// <summary>
    /// Create Conformer interpreter
    /// </summary>
    void InitSTTInference()
    {
      _options = new InterpreterOptions() {threads = 4};
      _conformerInterpreter = new Interpreter(FileUtil.LoadFile(conformerFilepath), _options);

      for (int i=0; i < numRNNs; i++)
        for (int j=0; j < nStates; j++)
          for (int k=0; k < stateSize; k++)
            states[i, j, 0, k] = 0;
    }

    #region Inferencing

    private Array[] PrepareInput(ref float[] inputStream)
    {
      Array[] inputData = new Array[3];

      inputData[0] = inputStream;
      inputData[1] = new int[1];
      inputData[2] = states;

      return inputData;
    }


    private char[] ConformerInference(ref float[] inputStream)
    {
      _conformerInterpreter.ResizeInputTensor(0, new int[1]{inputStream.Length});

      _conformerInterpreter.AllocateTensors();
      Array[] inputData = PrepareInput(ref inputStream);

      for (int d=0; d < inputData.Length; d++)
        _conformerInterpreter.SetInputTensorData(d, inputData[d]);

      _conformerInterpreter.Invoke();

      int[] outputShape = _conformerInterpreter.GetOutputTensorInfo(0).shape;
      char[] outputData = new char[outputShape[0]];
      _conformerInterpreter.GetOutputTensorData(0, outputData);
      return outputData;
    }
    #endregion
  }
}