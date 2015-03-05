using System;
using CocosSharp;


namespace DynamicTexture
{
	public class GameAppDelegate : CCApplicationDelegate
	{
		public override void ApplicationDidFinishLaunching (CCApplication application, CCWindow mainWindow)
		{
			application.PreferMultiSampling = false;
			application.ContentRootDirectory = "Content";
			application.ContentSearchPaths.Add ("animations");
			application.ContentSearchPaths.Add ("fonts");
			application.ContentSearchPaths.Add ("sounds");
			application.ContentSearchPaths.Add ("images");

			CCSize windowSize = mainWindow.WindowSizeInPixels;

			const float desiredWidth = 1024.0f;
			const float desiredHeight = 768.0f;
            
			CCScene.SetDefaultDesignResolution (desiredWidth, desiredHeight, CCSceneResolutionPolicy.ShowAll);
      
			CCScene scene = new CCScene (mainWindow);
			CCLayer layer = new MainLayer ();

			scene.AddChild (layer);

			mainWindow.RunWithScene (scene);
		}

		public override void ApplicationDidEnterBackground (CCApplication application)
		{
			application.Paused = true;
		}

		public override void ApplicationWillEnterForeground (CCApplication application)
		{
			application.Paused = false;
		}
	}
}
