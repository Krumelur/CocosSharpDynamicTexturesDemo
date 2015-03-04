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

			float desiredWidth = 1024.0f;
			float desiredHeight = 768.0f;
            
			// This will set the world bounds to be (0,0, w, h)
			// CCSceneResolutionPolicy.ShowAll will ensure that the aspect ratio is preserved
			CCScene.SetDefaultDesignResolution (desiredWidth, desiredHeight, CCSceneResolutionPolicy.ShowAll);
      
			CCScene scene = new CCScene (mainWindow);
			CCLayer layer = new HillLayer ();// new GameLayer ();

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
