using System;
using System.Collections.Generic;
using CocosSharp;

namespace DynamicTexture
{
	public class HillLayer : CCLayerColor
	{
		public HillLayer () : base (CCColor4B.Gray)
		{

			this.Schedule ();
		}


		HillNode hillNode;

		protected override void AddedToScene ()
		{
			base.AddedToScene ();


			// Initialize hill creator. Pass TRUE to enable debug mode.
			this.hillNode = new HillNode (this.ContentSize, CreateRandomTexture(), false) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Position = this.ContentSize.Center
			};
			this.AddChild (this.hillNode);

			this.AddEventListener (new CCEventListenerTouchOneByOne ()
			{
					OnTouchBegan = (touch, ev) => {
						this.hillNode.Texture = CreateRandomTexture();
						return true;
				}
			});

			var lbl = new CCLabel ("Tap screen to change texture", "Arial", 24) {
				Position = new CCPoint (this.ContentSize.Center.X, this.ContentSize.Height - 40)
			};
			this.AddChild(lbl);
		}

		static CCTexture2D CreateRandomTexture()
		{
			var dynamicSprite = CreateStripedSprite (new CCTexture2D ("images/pattern.png"), Util.CreateRandomBrightColor(), Util.CreateRandomBrightColor(), Util.Randomizer.Next(60, 200));
			return dynamicSprite.Texture;
		}

		public override void OnExit ()
		{
			base.OnExit ();
			this.Unschedule ();
		}

		public override void Update (float dt)
		{
			base.Update (dt);
			this.hillNode.OffsetX += 50 * dt;
		}

		public static CCSprite CreateStripedSprite (CCTexture2D texture, CCColor4B backgroundColor, CCColor4B stripeColor, float stripeWidth)
		{
			var textureSprite = new CCSprite (texture) {
				AnchorPoint = CCPoint.AnchorMiddle,
				BlendFunc = new CCBlendFunc (CCOGLES.GL_DST_COLOR, CCOGLES.GL_ZERO)
			};

			var renderTexture = new CCRenderTexture (textureSprite.ContentSize, textureSprite.ContentSize);
			renderTexture.BeginWithClear (backgroundColor);
			CCDrawingPrimitives.Begin ();

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
