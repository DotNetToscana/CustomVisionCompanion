# Custom Vision Companion

A Xamarin.Forms app showcasing how to use Custom Vision Service with either online and offline models.

![A screenshot of the Android version](https://raw.githubusercontent.com/DotNetToscana/CustomVisionCompanion/master/Screenshots/Android-1.png)

**Getting started**

To use an online model, you just need to set *Prediction Key* and *Project Id* from Custom Vision Service Project Settings  in the [Constants.cs](https://github.com/DotNetToscana/CustomVisionCompanion/blob/master/Src/CustomVisionCompanion/CustomVisionCompanion/Common/Constants.cs) file.

The sample contains an offline model exported from Custom Vision Service, either for [Android](https://github.com/DotNetToscana/CustomVisionCompanion/tree/master/Src/CustomVisionCompanion/CustomVisionCompanion.Android/Assets) and [iOS](https://github.com/DotNetToscana/CustomVisionCompanion/blob/master/Src/CustomVisionCompanion/CustomVisionCompanion.iOS/Computer.mlmodel). It has been trained to recognize laptop, keboard and mouse. It isn't much accurate, but you can easily replace it with your own. You just need to download the model from [Custom Vision Service Portal](https://docs.microsoft.com/en-us/azure/cognitive-services/custom-vision-service/export-your-model).

**Android offline models**

- Replace the *model.pb* and *labels.txt* files in the [Assets](https://github.com/DotNetToscana/CustomVisionCompanion/tree/master/Src/CustomVisionCompanion/CustomVisionCompanion.Android/Assets) folder with your own model. Be sure that the Build Action of both files must be set to *AndroidAsset*.

- Add the following line in the [OnCreate](https://github.com/DotNetToscana/CustomVisionCompanion/blob/master/Src/CustomVisionCompanion/CustomVisionCompanion.Android/MainActivity.cs#L31) method in *MainActivity.cs*:

```
CrossOfflineClassifier.Current.InitializeAsync(ModelType.General, "model.pb", "labels.txt");
```

**iOS offline models**

- Replace the *.mlmodel* file in the [Resources](https://github.com/DotNetToscana/CustomVisionCompanion/tree/master/Src/CustomVisionCompanion/CustomVisionCompanion.iOS/Resources) folder with your own model. Be sure that the Build Action of this file must be set to *BundleResource*. The model will be automatically compiled at app startup.
- OR
1. Manually compile the ML model and add it to the project. The CoreML compiler is included with the Xcode developer tools (Xcode 9 and higher). To compile an *.mlmodel*, run:

```
xcrun coremlcompiler compile {model.mlmodel} {outputFolder}
```

2. Put the output folder in the [Resources](https://github.com/DotNetToscana/CustomVisionCompanion/tree/master/Src/CustomVisionCompanion/CustomVisionCompanion.iOS/Resources/Computer.mlmodelc) folder. Be sure that the Build Action of each file must be set to *BundleResource*.

- Add the following line in the [FinishedLaunching](https://github.com/DotNetToscana/CustomVisionCompanion/blob/master/Src/CustomVisionCompanion/CustomVisionCompanion.iOS/AppDelegate.cs#L47) method in *AppDelegate.cs*:

```
CrossOfflineClassifier.Current.InitializeAsync(ModelType.General, "Computer");
```

**Contribute**

The project is continuously evolving. We welcome contributions. Feel free to file issues and pull requests on the repo and we'll address them as we can.
