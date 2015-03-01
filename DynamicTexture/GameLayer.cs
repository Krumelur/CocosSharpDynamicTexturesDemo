using System;
using System.Collections.Generic;
using CocosSharp;

namespace DynamicTexture
{
	public class GameLayer : CCLayerColor
	{
		public GameLayer () : base(CCColor4B.Black)
		{

		}

		protected async override void AddedToScene ()
		{
			base.AddedToScene ();

			const float fadeTimeSeconds = 3f;

			this.sprite1 = CreateRandomDynamicSprite ();
			this.sprite1.Position = this.ContentSize.Center;
			this.AddChild (this.sprite1);

			this.sprite2 = CreateRandomDynamicSprite ();
			this.sprite2.Position = this.ContentSize.Center;
			this.AddChild (this.sprite2);
			bool alternate = true;

			while(true)
			{
				if (alternate)
				{
					this.sprite2.RunAction(new CCFadeOut(fadeTimeSeconds));
					await this.sprite1.RunActionAsync(new CCFadeIn(fadeTimeSeconds));
					this.RemoveChild(this.sprite2);
					this.sprite2 = CreateRandomDynamicSprite ();
					this.sprite2.Position = this.ContentSize.Center;
					this.AddChild (this.sprite2);
				}
				else
				{
					this.sprite1.RunAction(new CCFadeOut(fadeTimeSeconds));
					await this.sprite2.RunActionAsync(new CCFadeIn(fadeTimeSeconds));
					this.RemoveChild(this.sprite1);
					this.sprite1 = CreateRandomDynamicSprite ();
					this.sprite1.Position = this.ContentSize.Center;
					this.AddChild (this.sprite1);
				}
				alternate = !alternate;
			}
		}

		static CCSprite CreateRandomDynamicSprite()
		{
			var sprite = CreateStripedSprite (new CCTexture2D ("images/pattern.png"), CreateRandomBrightColor(), CreateRandomBrightColor(), rand.Next(60, 200));
			return sprite;
		}

		CCSprite sprite1, sprite2;

		public static CCSprite CreateStripedSprite(CCTexture2D texture, CCColor4B backgroundColor, CCColor4B stripeColor, float stripeWidth)
		{
			var textureSprite = new CCSprite (texture)
			{
				AnchorPoint = CCPoint.AnchorMiddle,
				BlendFunc = new CCBlendFunc(CCOGLES.GL_DST_COLOR, CCOGLES.GL_ZERO)
			};

			var renderTexture = new CCRenderTexture (textureSprite.ContentSize, textureSprite.ContentSize);
			renderTexture.BeginWithClear (backgroundColor);
			CCDrawingPrimitives.Begin ();

			float stripeEdgeX = -textureSprite.ContentSize.Width;
			while(stripeEdgeX <= textureSprite.ContentSize.Width)
			{
				var polyPoints = new List<CCPoint> () {
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

		static Random rand = new Random();

		public static CCColor4B CreateRandomBrightColor(int minBrightness = 192)
		{
			byte red = (byte)rand.Next (minBrightness, 255);
			byte green = (byte)rand.Next (minBrightness, 255);
			byte blue = (byte)rand.Next (minBrightness, 255);
			var randomColor = new CCColor4B (red, green, blue);
			return randomColor;
		}
	}
}
