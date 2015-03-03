using System;
using CocosSharp;

namespace DynamicTexture
{
	public class TestDrawNode : CCDrawNode
	{

		CCTexture2D texture;
		CCGeometryBatch geoBatch = new CCGeometryBatch();

		public TestDrawNode (CCTexture2D texture) : base()
		{
			this.texture = texture;
			//texture = CCTextureCache.SharedTextureCache.AddImage("Images/BackGround.png");
		}

		protected override void AddedToScene()
		{
			base.AddedToScene();

			var visibleRect = VisibleBoundsWorldspace;

			// We will not clear the primitives after creating them
			// This will allow us to keep drawing the same over and over.
			geoBatch.AutoClearInstances = false;

			geoBatch.Begin();

			var item = geoBatch.CreateGeometryInstance(3, 3);

			var vertices = item.GeometryPacket.Vertices;

			var windowSize = VisibleBoundsWorldspace.Size;
			var packet = item.GeometryPacket;

			packet.Texture = texture;
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

			vertices[0].Colors = CCColor4B.Orange;
			vertices[1].Colors = CCColor4B.Green;
			vertices[2].Colors = CCColor4B.Magenta;

			// Texture coordinates use a normalized value 0 to 1
			vertices[0].TexCoords.U = 1;
			vertices[0].TexCoords.V = 0;

			vertices[1].TexCoords.U = 0;
			vertices[1].TexCoords.V = 0;

			vertices[2].TexCoords.U = 0;
			vertices[2].TexCoords.V = 1;

			// Set screen coordinates.
			vertices[0].Vertices.X = 50;
			vertices[0].Vertices.Y = 450;

			vertices[1].Vertices.X = 450;
			vertices[1].Vertices.Y = 450;

			vertices[2].Vertices.X = 50;
			vertices[2].Vertices.Y = 50;

			item.GeometryPacket.Indicies = new int[] { 0, 2, 1 };

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

		protected override void Draw()
		{
			base.Draw();

			geoBatch.Draw();
		}

	}
}

