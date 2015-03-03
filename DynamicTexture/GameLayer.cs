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

		CCGeometryBatch geoBatch = new CCGeometryBatch();

		protected override void AddedToScene()
		{
			base.AddedToScene();


			//const float fadeTimeSeconds = 3f;
			//			this.sprite1 = CreateRandomDynamicSprite ();
			//			this.sprite1.Position = this.ContentSize.Center;
			//			this.AddChild (this.sprite1);
			//
			//			this.sprite2 = CreateRandomDynamicSprite ();
			//			this.sprite2.Position = this.ContentSize.Center;
			//			this.AddChild (this.sprite2);
			//			bool alternate = true;
			//
			//			while(true)
			//			{
			//				if (alternate)
			//				{
			//					this.sprite2.RunAction(new CCFadeOut(fadeTimeSeconds));
			//					await this.sprite1.RunActionAsync(new CCFadeIn(fadeTimeSeconds));
			//					this.RemoveChild(this.sprite2);
			//					this.sprite2 = CreateRandomDynamicSprite ();
			//					this.sprite2.Position = this.ContentSize.Center;
			//					this.AddChild (this.sprite2);
			//				}
			//				else
			//				{
			//					this.sprite1.RunAction(new CCFadeOut(fadeTimeSeconds));
			//					await this.sprite2.RunActionAsync(new CCFadeIn(fadeTimeSeconds));
			//					this.RemoveChild(this.sprite1);
			//					this.sprite1 = CreateRandomDynamicSprite ();
			//					this.sprite1.Position = this.ContentSize.Center;
			//					this.AddChild (this.sprite1);
			//				}
			//				alternate = !alternate;
			//			}

			//			var dynSprite = CreateRandomDynamicSprite ();
			//			dynSprite.AnchorPoint = CCPoint.AnchorMiddle;
			//			dynSprite.Position = this.ContentSize.Center;
			//			this.AddChild (dynSprite);



			var visibleRect = VisibleBoundsWorldspace;

			// We will not clear the primitives after creating them
			// This will allow us to keep drawing the same over and over.
			geoBatch.AutoClearInstances = false;

			geoBatch.Begin();

			var item = geoBatch.CreateGeometryInstance(4, 6);

			var vertices = item.GeometryPacket.Vertices;

			var windowSize = VisibleBoundsWorldspace.Size;
			var packet = item.GeometryPacket;

			packet.Texture = new CCTexture2D("texture.png");
			item.GeometryPacket = packet;
			// Draw polygons

			/*
			*  TL    TR
			*   0----1 0,1,2,3 = index offsets for vertex indices
			*   |   /| 
			*   |  / |
			*   | /  |
			*   |/   |
			*   2----3
			*  BL    BR
			* 
			* */

//			vertices[0].Colors = CCColor4B.Orange;
//			vertices[1].Colors = CCColor4B.Green;
//			vertices[2].Colors = CCColor4B.Magenta;
//			vertices[3].Colors = CCColor4B.Blue;


			vertices[0].Colors = CCColor4B.White;
			vertices[1].Colors = CCColor4B.White;
			vertices[2].Colors = CCColor4B.White;
			vertices[3].Colors = CCColor4B.White;

			// Texture coordinates use a normalized value 0 to 1
			vertices[0].TexCoords.U = 0;
			vertices[0].TexCoords.V = 1;

			vertices[1].TexCoords.U = 1;
			vertices[1].TexCoords.V = 0;

			vertices[2].TexCoords.U = 0;
			vertices[2].TexCoords.V = 0;

			vertices[3].TexCoords.U = 1;
			vertices[3].TexCoords.V = 1;

			// Set screen coordinates (lower left = 0/0; going up and right).
			// Left triangle. Top right
			vertices[0].Vertices.X = 50;
			vertices[0].Vertices.Y = 450;

			// top right
			vertices[1].Vertices.X = 450;
			vertices[1].Vertices.Y = 450;

			// bottom left
			vertices[2].Vertices.X = 50;
			vertices[2].Vertices.Y = 50;

			// Right triangle (bottom right)
			vertices[3].Vertices.X = 450;
			vertices[3].Vertices.Y = 50;


			item.GeometryPacket.Indicies = new int[] { 0, 2, 1, 3, 2, 1 };

			//			var rotation = CCAffineTransform.Identity;
			//			rotation.Rotation = (float)Math.PI / 4.0f;
			//			rotation.Tx = windowSize.Center.X - texture.PixelsWide / 2;
			//			rotation.Ty = windowSize.Center.Y - texture.PixelsHigh / 2;
			//			item.InstanceAttributes.AdditionalTransform = rotation;

			geoBatch.End();
		}

		public override void OnExit()
		{
			// We will clean the batch up here.
			geoBatch.ClearInstances();
		}

		protected override void Draw ()
		{
			base.Draw ();

			CCDrawingPrimitives.Begin ();
			CCDrawingPrimitives.LineWidth = 2f;
			int i = 0;
			while (i <= this.ContentSize.Width)
			{
				CCDrawingPrimitives.DrawLine (new CCPoint (i, 0), new CCPoint (i, this.ContentSize.Height), CCColor4B.LightGray);
				i += 50;
			}

			i = 0;
			while (i <= this.ContentSize.Height)
			{
				CCDrawingPrimitives.DrawLine (new CCPoint (0, i), new CCPoint (this.ContentSize.Width, i), CCColor4B.LightGray);
				i += 50;
			}
			CCDrawingPrimitives.End ();

			geoBatch.Draw();
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
