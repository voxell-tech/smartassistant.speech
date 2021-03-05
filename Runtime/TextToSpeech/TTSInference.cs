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

namespace SmartAssistant.Speech.TTS
{
  public partial class TextToSpeech
  {
    public string fastspeechFilepath;
    public string melganFilepath;
    public int speakerID = 1;
    public float speedRatio = 1.0f;

    private Interpreter _fastspeechInterpreter;
    private Interpreter _melganInterpreter;
    private InterpreterOptions _options;

    void InitTTSInference()
    {
      print($"TFLite Version: {Interpreter.GetVersion()}");
      // initialize tflite fastspeech and melgan Interpreter
      _options = new InterpreterOptions() {threads = 4};
      _options.useNNAPI = true;
      _fastspeechInterpreter = new Interpreter(FileUtil.LoadFile(fastspeechFilepath), _options);
      _melganInterpreter = new Interpreter(FileUtil.LoadFile(melganFilepath), _options);
    }

    #region Inferencing
    /// <summary>
    /// Formats inputIDs, speakerID and speedRatio into arrays that is to be used as _fastspeechInterpreter input tensors.
    /// </summary>
    /// <param name="inputIDs">input token ids translated from text string letter by letter</param>
    /// <param name="speakerID">the id of the speaker that we wish to use</param>
    /// <param name="speedRatio">the speed of the output speech</param>
    /// <returns>An array of all input data.</returns>
    private Array[] PrepareInput(ref int[] inputIDs, ref int speakerID, ref float speedRatio)
    {
      Array[] inputData = new Array[3];

      int[,] formatedInputIDS = new int[1, inputIDs.Length];
      for (int i=0; i < inputIDs.Length; i++) formatedInputIDS[0, i] = inputIDs[i];
      inputData[0] = formatedInputIDS;
      inputData[1] = new int[1]{speakerID};
      inputData[2] = new float[1]{speedRatio};

      return inputData;
    }

    /// <summary>
    /// Inferencing fastspeech tflite model by taking in text and converting them into spectogram
    /// </summary>
    /// <param name="text">input text</param>
    private float[,,] FastspeechInference(ref string text)
    {
      // resize the input tensors to fit the size of inputIDs
      int[] inputIDs = TextToSequence(text);
      _fastspeechInterpreter.ResizeInputTensor(0, new int[2]{1, inputIDs.Length});
      _fastspeechInterpreter.ResizeInputTensor(1, new int[1]{1});
      _fastspeechInterpreter.ResizeInputTensor(2, new int[1]{1});

      // allocate tensors and set input data
      _fastspeechInterpreter.AllocateTensors();
      Array[] inputData = PrepareInput(ref inputIDs, ref speakerID, ref speedRatio);
      for (int d=0; d < 3; d++)
        _fastspeechInterpreter.SetInputTensorData(d, inputData[d]);

      // run the interpreter
      _fastspeechInterpreter.Invoke();

      // obtaining the output from the model
      int[] outputShape = _fastspeechInterpreter.GetOutputTensorInfo(1).shape;
      float[,,] outputData = new float[outputShape[0], outputShape[1], outputShape[2]];
      _fastspeechInterpreter.GetOutputTensorData(1, outputData);
      return outputData;
    }
    
    /// <summary>
    /// Inferencing melgan tflite model by converting spectogram to audio
    /// </summary>
    /// <param name="spectogram">input spectogram</param>
    /// <returns></returns>
    private float[,,] MelganInference(ref float[,,] spectogram)
    {
      // resize input tensors to fit the size of spectogram
      _melganInterpreter.ResizeInputTensor(0, new int[3]{
        spectogram.GetLength(0),
        spectogram.GetLength(1),
        spectogram.GetLength(2)});

      // allocate tensors and set input data
      _melganInterpreter.AllocateTensors();
      _melganInterpreter.SetInputTensorData(0, spectogram);

      // run the interpreter
      _melganInterpreter.Invoke();

      // obtaining the output from the model
      int[] outputShape = _melganInterpreter.GetOutputTensorInfo(0).shape;
      float[,,] outputData = new float[outputShape[0], outputShape[1], outputShape[2]];
      _melganInterpreter.GetOutputTensorData(0, outputData);
      return outputData;
    }
    #endregion

  }
}