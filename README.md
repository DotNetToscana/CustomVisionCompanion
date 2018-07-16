# Custom Vision Companion

A Xamarin.Forms app showcasing how to use Custom Vision Service with either online and offline models. Android, iOS and UWP are currently supported.

![A screenshot of the Android version](https://raw.githubusercontent.com/DotNetToscana/CustomVisionCompanion/master/Screenshots/Android-1.png)

![A screenshot of the UWP version](https://raw.githubusercontent.com/DotNetToscana/CustomVisionCompanion/master/Screenshots/Uwp-1.png)

**Getting started**

To use an online model, launch the app, go to the Settings page and then set *Prediction Key* and *Project Id* from Custom Vision Service Project Settings.

The sample contains an offline model exported from Custom Vision Service, for [Android](https://github.com/DotNetToscana/CustomVisionCompanion/tree/master/Src/CustomVisionCompanion/CustomVisionCompanion.Android/Assets), [iOS](https://github.com/DotNetToscana/CustomVisionCompanion/blob/master/Src/CustomVisionCompanion/CustomVisionCompanion.iOS/Resources/Computer.mlmodel) and [iOS](https://github.com/DotNetToscana/CustomVisionCompanion/blob/master/Src/CustomVisionCompanion/CustomVisionCompanion.UWP/Assets/Models/Computer.onnx). It has been trained to recognize laptops, keyboards, mice, monitors and controllers. You can easily replace it with your own: after the training, you just need to download the models from [Custom Vision Service Portal](https://docs.microsoft.com/en-us/azure/cognitive-services/custom-vision-service/export-your-model).

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

2. Put the output folder inside the [Resources](https://github.com/DotNetToscana/CustomVisionCompanion/tree/master/Src/CustomVisionCompanion/CustomVisionCompanion.iOS/Resources) folder. Be sure that the Build Action of each file must be set to *BundleResource*.

- Add the following line in the [FinishedLaunching](https://github.com/DotNetToscana/CustomVisionCompanion/blob/master/Src/CustomVisionCompanion/CustomVisionCompanion.iOS/AppDelegate.cs#L47) method in *AppDelegate.cs*:

```
CrossOfflineClassifier.Current.InitializeAsync(ModelType.General, "Computer");
```

**UWP offline models**

- Replace the *Computer.onnx* file in the [Assets/Models](https://github.com/DotNetToscana/CustomVisionCompanion/tree/master/Src/CustomVisionCompanion/CustomVisionCompanion.UWP/Assets/Models) folder with your own model. Be sure that the Build Action of this file must be set to *Content*. Note that, when you add the file to the project, Visual Studio will automatically create a .cs file containing the code to use the model. You can safely delete it, as this project already contains a standard implementation.

- Add the following line in the [constructor](https://github.com/DotNetToscana/CustomVisionCompanion/blob/master/Src/CustomVisionCompanion/CustomVisionCompanion.UWP/MainPage.xaml.cs#L28) of *MainPage.xaml.cs*:

```
CrossOfflineClassifier.Current.InitializeAsync(ModelType.General, "ms-appx:///Assets/Models/Computer.onnx",
    "controller", "keyboard", "laptop", "monitor", "mouse");
```

- In the line above, after the name of the ONNX file you need to pass all the tags of the models, in the same order you find in Custom Vision Portal. This method of passing data may be improved in the future.

**Contribute**

The project is continuously evolving. We welcome contributions. Feel free to file issues and pull requests on the repo and we'll address them as we can.
