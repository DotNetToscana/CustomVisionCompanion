using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace CustomVisionCompanion.Droid
{
    [Activity(Theme = "@style/Theme.Splash", //Indicates the theme to use for this activity
        MainLauncher = true, //Set it as boot activity
        NoHistory = true,  //Doesn't place it in back stack
        ScreenOrientation = ScreenOrientation.Landscape)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.StartActivity(typeof(MainActivity));
        }
    }
}

