using System;
using System.Collections.Generic;
using CocosSharp;

namespace DynamicTexture
{
	/// <summary>
	/// Main layer for the demo.
	/// </summary>
	public class MainLayer : CCLayerGradient
	{
		public MainLayer () : base(CCColor4B.Blue, CCColor4B.Black)
		{
		}

		const float SCROLL_SPEED_PIXELS_PER_SECOND = 60;
		HillLayer hillLayer;

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Create a nice initial texture for the hills.
			var initialTexture = CreateStripedSprite (new CCTexture2D ("images/pattern.png"), new CCColor4B(201,217,254,255), new CCColor4B(232,254,201,255), 128).Texture;

			// Create the hill layer.
			this.hillLayer = new HillLayer (
				texture: initialTexture,
				numHillKeyPoints: 1000,
				segmentWidthPixels: 8,
				hillWidthPixels: 256,
				showDebug: false)
			{
				AnchorPoint = CCPoint.AnchorLowerLeft,
				Position = new CCPoint(0, 0)
			};
			this.AddChild (this.hillLayer);

			// Tapping the screen changes to another random texture.
			this.AddEventListener (new CCEventListenerTouchOneByOne ()
				{
					OnTouchBegan = (touch, ev) => {
						this.hillLayer.Texture = CreateRandomTexture();
						return true;
					}
				});

			var lbl = new CCLabel ("Tap screen to change texture", "Arial", 24) {
				Position = new CCPoint (this.ContentSize.Center.X, this.ContentSize.Height - 40)
			};
			this.AddChild(lbl);

			// Start scheduling updates to scroll the hill layer.
			this.Schedule ();
		}

		public override void OnExit ()
		{
			base.OnExit ();
			this.UnscheduleAll ();
		}

		public override void Update (float dt)
		{
			base.Update (dt);

			// Move hills a bit.
			this.hillLayer.OffsetX += SCROLL_SPEED_PIXELS_PER_SECOND * dt;
		}

		/// <summary>
		/// Helper to create a random texture.
		/// </summary>
		/// <returns>The random texture.</returns>
		static CCTexture2D CreateRandomTexture()
		{
			var dynamicSprite = CreateStripedSprite (new CCTexture2D ("images/pattern.png"), Util.CreateRandomBrightColor(100), Util.CreateRandomBrightColor(), (float)Math.Pow(2, Util.Randomizer.Next(3, 9)));
			return dynamicSprite.Texture;
		}

		/// <summary>
		/// Helper to create a striped sprite dynamically.
		/// </summary>
		/// <returns>The striped sprite.</returns>
		/// <param name="texture">Texture.</param>
		/// <param name="backgroundColor">Background color.</param>
		/// <param name="stripeColor">Stripe color.</param>
		/// <param name="stripeWidth">Stripe width.</param>
		static CCSprite CreateStripedSprite (CCTexture2D texture, CCColor4B backgroundColor, CCColor4B stripeColor, float stripeWidth)
		{
			// Load the base sprite. That's a texture we draw the stripes on.
			var textureSprite = new CCSprite (texture) {
				AnchorPoint = CCPoint.AnchorMiddle,
				BlendFunc = new CCBlendFunc (CCOGLES.GL_DST_COLOR, CCOGLES.GL_ZERO)
			};

			// Create a dynamic texture we can draw on.
			var renderTexture = new CCRenderTexture (textureSprite.ContentSize, textureSprite.ContentSize);

			// Calling this will direct all drawing calls onto the render texture.
			renderTexture.BeginWithClear (backgroundColor);

			CCDrawingPrimitives.Begin ();

			// Start drawing stripes left of the texture so the complete texture is covered with diagonal stripes.
			float stripeEdgeX = -textureSprite.ContentSize.Width;
			while (stripeEdgeX <= textureSprite.ContentSize.Width)
			{
				var polyPoints = new List<CCPoint> {
					new CCPoint (stripeEdgeX - stripeWidth, 0),
					new CCPoint (stripeEdgeX + textureSprite.ContentSize.Width - stripeWidth, textureSprite.ContentSize.Height),
					new CCPoint (stripeEdgeX + textureSprite.ContentSize.Width, textureSprite.ContentSize.Height),
					new CCPoint (stripeEdgeX, 0),
				};

				CCDrawingPrimitives.DrawSolidPoly (polyPoints.ToArray (), stripeColor);
				stripeEdgeX += stripeWidth * 2;
			}
			CCDrawingPrimitives.End ();

			textureSprite.Position = textureSprite.ContentSize.Center;
			textureSprite.Visit ();

			renderTexture.End ();

			return renderTexture.Sprite;
		}
	}
}
