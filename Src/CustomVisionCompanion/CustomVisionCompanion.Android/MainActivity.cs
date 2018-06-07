using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Acr.UserDialogs;
using Plugin.CustomVisionEngine;
using Plugin.CustomVisionEngine.Models;
using Plugin.Permissions;

namespace CustomVisionCompanion.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme",
           ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            UserDialogs.Init(this);
            LoadApplication(new App());

            var task = CrossOfflineClassifier.Current.InitializeAsync(ModelType.General, "model.pb", "labels.txt");
        }

        protected override void OnPause()
        {
            App.IsPausing = true;
            base.OnPause();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

