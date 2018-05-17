using Android.App;
using Android.Content.PM;
using Android.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVisionCompanion.Droid
{
    [Activity(Theme = "@style/SplashTheme", //Indicates the theme to use for this activity
            MainLauncher = true, //Set it as boot activity
            NoHistory = true,  //Doesn't place it in back stack
            ScreenOrientation = ScreenOrientation.Portrait,
            ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            StartActivity(typeof(MainActivity));
            Finish();
        }
    }
}
