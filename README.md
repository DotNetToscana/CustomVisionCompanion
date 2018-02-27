# Custom Vision Companion

A Xamarin.Forms app showcasing how to use Custom Vision Service with either online and offline models.            

![A screenshot of the Android version](https://raw.githubusercontent.com/DotNetToscana/CustomVisionCompanion/master/Screenshots/Android-1.png)

**Getting started**

To use an online model, you just need to set *Prediction Key* and *Project Id* from Custom Vision Service Project Settings  in the [Constants.cs](https://github.com/DotNetToscana/CustomVisionCompanion/blob/master/Src/CustomVisionCompanion/CustomVisionCompanion/Common/Constants.cs) file.

The sample contains an offline model exported from Custom Vision Service, either for [Android](https://github.com/DotNetToscana/CustomVisionCompanion/tree/master/Src/CustomVisionCompanion/CustomVisionCompanion.Android/Assets) and [iOS](https://github.com/DotNetToscana/CustomVisionCompanion/blob/master/Src/CustomVisionCompanion/CustomVisionCompanion.iOS/Computer.mlmodel). It has been trained to recognize laptop, keboard and mouse. It isn't much accurate, but you can easily replace it with your own. You just need to download the model from [Custom Vision Service Portal](https://docs.microsoft.com/en-us/azure/cognitive-services/custom-vision-service/export-your-model).

- For Android, you just need to replace the *.pb* and *.txt* files in the [Assets](https://github.com/DotNetToscana/CustomVisionCompanion/tree/master/Src/CustomVisionCompanion/CustomVisionCompanion.Android/Assets) folder.

- For iOS, it is necessary to compile the ML model. The CoreML compiler is included with the Xcode developer tools (Xcode 9 and higher). To compile an *.mlmodel*, run:

```
xcrun coremlcompiler compile {model.mlmodel} {outputFolder}
```

And then put the output in the [Resources](https://github.com/DotNetToscana/CustomVisionCompanion/tree/master/Src/CustomVisionCompanion/CustomVisionCompanion.iOS/Resources/Computer.mlmodelc) folder.

**Contribute**

The project is continuously evolving. We welcome contributions. Feel free to file issues and pull requests on the repo and we'll address them as we can.
