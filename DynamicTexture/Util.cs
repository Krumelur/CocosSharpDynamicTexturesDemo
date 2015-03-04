using System;
using CocosSharp;

namespace DynamicTexture
{
	public static class Util
	{
		/// <summary>
		/// Randomizer.
		/// </summary>
		public static Random Randomizer
		{
			get
			{
				if (randomizer == null)
				{
					randomizer = new Random ();
				}
				return randomizer;
			}
		}
		static Random randomizer;

		/// <summary>
		/// Creates a random bright color.
		/// </summary>
		/// <returns>The random bright color.</returns>
		/// <param name="minBrightness">Minimum brightness.</param>
		public static CCColor4B CreateRandomBrightColor(int minBrightness = 192)
		{
			byte red = (byte)Randomizer.Next (minBrightness, 255);
			byte green = (byte)Randomizer.Next (minBrightness, 255);
			byte blue = (byte)Randomizer.Next (minBrightness, 255);
			var randomColor = new CCColor4B (red, green, blue);
			return randomColor;
		}

		public static CCGeometryInstance AddTexturedQuad(
			this CCGeometryBatch geoBatch,
			CCPoint lowerLeft,
			CCPoint upperLeft,
			CCPoint upperRight,
			CCPoint lowerRight,
			CCTexture2D texture,
			CCPoint texturePropLowerLeft = default(CCPoint),
			CCPoint texturePropUpperLeft = default(CCPoint),
			CCPoint texturePropUpperRight= default(CCPoint),
			CCPoint texturePropLowerRight = default(CCPoint),
			CCColor4B colorLowerLeft = default(CCColor4B),
			CCColor4B colorUpperLeft = default(CCColor4B),
			CCColor4B colorUpperRight = default(CCColor4B),
			CCColor4B colorLowerRight = default(CCColor4B))
		{
			var item = geoBatch.CreateGeometryInstance(4, 6);
			var vertices = item.GeometryPacket.Vertices;
			var packet = item.GeometryPacket;

			packet.Texture = texture;
			item.GeometryPacket = packet;

			/* Draw polygons
			* 
			*  TL    TR
			*   1----2 
			*   |   /| 
			*   |  / |
			*   | /  |
			*   |/   |
			*   0----3
			*  BL    BR
			* 
			* */

			vertices [0].Colors = colorLowerLeft == default(CCColor4B) ? CCColor4B.White : colorLowerLeft;
			vertices[1].Colors = colorUpperLeft == default(CCColor4B) ? CCColor4B.White : colorUpperLeft;
			vertices[2].Colors = colorUpperRight == default(CCColor4B) ? CCColor4B.White : colorUpperRight;
			vertices[3].Colors = colorLowerRight == default(CCColor4B) ? CCColor4B.White : colorLowerRight;

			if (texturePropLowerLeft == default(CCPoint))
			{
				texturePropLowerLeft = new CCPoint (0, 0);
			}

			if (texturePropUpperLeft == default(CCPoint))
			{
				texturePropUpperLeft = new CCPoint (0, 1);
			}

			if (texturePropUpperRight == default(CCPoint))
			{
				texturePropUpperRight = new CCPoint (1, 1);
			}

			if (texturePropLowerRight == default(CCPoint))
			{
				texturePropLowerRight = new CCPoint (1, 0);
			}

			// Texture coordinates use a normalized value 0 to 1 and upside down.
			// This method assumes the Y-coordinates going up, just like screen coordinates and converts them.
			vertices[0].TexCoords.U = texturePropLowerLeft.X;
			vertices[0].TexCoords.V = 1 - texturePropLowerLeft.Y;

			vertices[1].TexCoords.U = texturePropUpperLeft.X;
			vertices[1].TexCoords.V = 1 - texturePropUpperLeft.Y;

			vertices[2].TexCoords.U = texturePropUpperRight.X;
			vertices[2].TexCoords.V = 1 - texturePropUpperRight.Y;

			vertices[3].TexCoords.U = texturePropLowerRight.X;
			vertices[3].TexCoords.V = 1 - texturePropLowerRight.Y;

			// Set screen coordinates.
			vertices [0].Vertices.X = lowerLeft.X;
			vertices [0].Vertices.Y = lowerLeft.Y;

			vertices [1].Vertices.X = upperLeft.X;
			vertices [1].Vertices.Y = upperLeft.Y;

			vertices [2].Vertices.X = upperRight.X;
			vertices [2].Vertices.Y = upperRight.Y;

			vertices [3].Vertices.X = lowerRight.X;
			vertices [3].Vertices.Y = lowerRight.Y;

			item.GeometryPacket.Indicies = new int[] { 0, 1, 2, 0, 2, 3 };

			//Console.WriteLine ("Adding quad: BL [Pos: {0}, Texture: {1}], TL [Pos: {2}, Texture: {3}], TR [Pos: {4}, Texture: {5}], BR [Pos: {6}, Texture: {7}]", vertices [0].Vertices, vertices [0].TexCoords, vertices [1].Vertices, vertices [1].TexCoords, vertices [2].Vertices, vertices [2].TexCoords, vertices [3].Vertices, vertices [3].TexCoords);

			return item;
		}
	}
}

