# Speech functionality for Smart Assistant

## What does this repository contatins??

This repository provides a fully functional end to end Text To Speech and Speech To Text solution intergrated in Unity with C#!!! Anyone can use this repo in any way they want as long as they credit the author and also respect the [license](LICENSE) agreement.

### Text To Speech

The model that we use for TTS is FastSpeech. The TFLite model that we used is converted from a pre-trained model found in the [TensorflowTTS repository](https://github.com/TensorSpeech/TensorFlowTTS).

To prevent Unity from freezing when inferencing the TFLite model, we run the inference process in a new thread and play the audio in the main thread once it is ready.

### Speech To Text

In progress...

## How to use?

All TFLite model inferencing will not be possible without the help of the [Unity TFLite](https://github.com/asus4/tf-lite-unity-sample) repository.

1. Clone the repository mentioned above.
2. Go into the Packages folder and copy the `com.github.asus4.tflite` folder into your project's Packages folder.
3. Clone this repository and the [Core Repository](https://github.com/voxell-tech/smartassistant.core) into your project's Packages folder.
4. And you are ready to go!

## Support the project!

This project is part of the [Smart Assistant](https://github.com/voxell-tech/SmartAssistant) project. If you feel like supporting the development of this project, simply click the "Sponsor" button on this page or support us on [Patreon](https://www.patreon.com/smartassistant)! Thank you!
