using Plugin.CustomVisionEngine;
using Plugin.CustomVisionEngine.Models;

namespace CustomVisionCompanion.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            LoadApplication(new CustomVisionCompanion.App());

            _ = CrossOfflineClassifier.Current.InitializeAsync(ModelType.General, "ms-appx:///Assets/Models/Computer.onnx");
        }
    }
}
