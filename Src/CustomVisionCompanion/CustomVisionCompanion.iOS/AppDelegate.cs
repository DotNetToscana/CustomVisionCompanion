using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Plugin.CustomVisionEngine;
using Plugin.CustomVisionEngine.Models;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace CustomVisionCompanion.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();
			LoadApplication(new App());

			var resources = Xamarin.Forms.Application.Current.Resources.MergedDictionaries.First();
			var defaultColor = ((Color)resources["DefaultColor"]).ToUIColor();
			var statusBarColor = ((Color)resources["StatusBarColor"]).ToUIColor();

			var statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView;
			if (statusBar.RespondsToSelector(new ObjCRuntime.Selector("setBackgroundColor:")))
			{
				statusBar.BackgroundColor = statusBarColor;
			}

			if (UIApplication.SharedApplication.KeyWindow != null)
			{
				UIApplication.SharedApplication.KeyWindow.TintColor = defaultColor;
			}

			var task = CrossOfflineClassifier.Current.InitializeAsync(ModelType.General, "Computer");

			return base.FinishedLaunching(app, options);
		}
	}
}
