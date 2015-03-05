using System;
using System.Linq;
using System.Collections.Generic;
using CocosSharp;

namespace DynamicTexture
{
	/// <summary>
	/// Renders a layer that contains random textured hills.
	/// </summary>
	public class HillLayer : CCLayer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicTexture.HillLayer"/> class.
		/// </summary>
		/// <param name="texture">Texture.</param>
		/// <param name="numHillKeyPoints">Number of hill key points (how many hills would you like?)</param>
		/// <param name="segmentWidthPixels">Width of segments that are used for smoothing between to key points.</param>
		/// <param name="hillWidthPixels">Width of one hill in pixels</param>
		/// <param name="showDebug">If set to <c>true</c> show debug.</param>
		public HillLayer (CCTexture2D texture, int numHillKeyPoints, int segmentWidthPixels, int hillWidthPixels, bool showDebug) : base()
		{
			this.Texture = texture;
			this.showDebug = showDebug;
			this.numHillKeyPoints = numHillKeyPoints;
			this.segmentWidthPixels = segmentWidthPixels;
			this.hillWidthPixels = hillWidthPixels;

			this.keyHillPoints = new List<CCPoint> (numHillKeyPoints);
		}

		bool showDebug;
		int numHillKeyPoints;
		int segmentWidthPixels;
		int hillWidthPixels;

		List<CCPoint> keyHillPoints;
		int minKeyPointIndex;
		int maxKeyPointIndex;
		CCGeometryBatch geoBatch;
		float offsetX;

		/// <summary>
		/// Gets or sets the texture.
		/// </summary>
		/// <value>The texture.</value>
		public CCTexture2D Texture
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the current scrolling position of the hill landscape. 
		/// </summary>
		/// <value>The offset x.</value>
		public float OffsetX
		{
			get
			{
				return this.offsetX;
			}
			set
			{
				this.PositionX = -value;
				this.offsetX = value;
				this.UpdateMinMaxVisibleKeyPointIndex ();
			}
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			this.keyHillPoints.Clear ();
			this.offsetX = 0;
			this.geoBatch = new CCGeometryBatch ()
			{
				AutoClearInstances = false
			};

			this.ContentSize = this.Parent.ContentSize;
			this.GenerateRandomHillKeyPoints (hillWidthPixels: hillWidthPixels);
			this.UpdateMinMaxVisibleKeyPointIndex ();
		}

		public override void OnExit ()
		{
			base.OnExit ();
			this.geoBatch.ClearInstances();
		}

		/// <summary>
		/// Generates random keypoints for the hills.
		/// </summary>
		/// <param name="hillWidthPixels">Hill width pixels.</param>
		/// <param name="minHillHeightPixels">Minimum hill height pixels.</param>
		/// <param name="maxHillHeightPixels">Max hill height pixels.</param>
		void GenerateRandomHillKeyPoints(int hillWidthPixels, int minHillHeightPixels = 5, int maxHillHeightPixels = 400)
		{
			float x = 0;
			float y;
			for (int i = 0; i < numHillKeyPoints; ++i)
			{
				y = Util.Randomizer.Next (minHillHeightPixels, maxHillHeightPixels);
				this.keyHillPoints.Add (new CCPoint (x, y));
				x += hillWidthPixels;
			}
		}

		/// <summary>
		/// Recalculates the current visible minimum keypoint index and the maximum visible keypoint index.
		/// As the landscapes scrolls, only a view keypoints will be visible. No need to process the ones not currently shown.
		/// </summary>
		void UpdateMinMaxVisibleKeyPointIndex()
		{
			if (this.keyHillPoints.Count <= 0)
			{
				return;
			}

			// Keep increasing min index until we cross the left border of the screen.
			while (this.keyHillPoints[this.minKeyPointIndex].X < this.OffsetX - hillWidthPixels && this.minKeyPointIndex < this.keyHillPoints.Count - 1)
			{
				this.minKeyPointIndex++;
			}

			// Keep increasing max index until we cross right border of the screen.
			while (this.keyHillPoints[this.maxKeyPointIndex].X <= this.OffsetX + this.ContentSize.Width + hillWidthPixels * 2 && this.maxKeyPointIndex < this.keyHillPoints.Count - 1)
			{
				this.maxKeyPointIndex++;
			}

			// Redraw all quads.
			this.geoBatch.ClearInstances ();
			geoBatch.Begin();

			// Loop all visible keypoints.
			for (int keypointIndex = this.minKeyPointIndex; keypointIndex < this.maxKeyPointIndex; keypointIndex++)
			{
				// Always work on the current and the next keypoint and texture the segments between these two.
				var p0 = this.keyHillPoints [keypointIndex];
				var p1 = this.keyHillPoints [keypointIndex + 1];

				// Apply textures to segments.
				var segmentPoints = HillLayer.GetSmoothSegmentLocations (p0, p1, segmentWidthPixels);

				var previousSegmentPoint = segmentPoints [0];
				// Calculate how wide one segment will be in the texture by dividing texture width by number of segments
				// (remember: there is one more segment point than segments, just like a fence has one more post than spaces!).
				float textureSegmentWidthPixels = this.Texture.PixelsWide / (segmentPoints.Count - 1);

				// Draw a quad for each segment.
				for (int segmentIndex = 1; segmentIndex < segmentPoints.Count; segmentIndex++)
				{
					var currentSegmentPoint = segmentPoints [segmentIndex];

					// Get the proportional X coordinate within the texture for the previous segment point and the current one.
					// That defines the part of the texture we will draw.
					float previousSegmentPropX = (float)((segmentIndex - 1) * textureSegmentWidthPixels) / this.Texture.PixelsWide;
					float currentSegmentPropX = (float)(segmentIndex * textureSegmentWidthPixels) / this.Texture.PixelsWide;

					// Draw the quad.
					this.geoBatch.AddTexturedQuad (
						lowerLeft: new CCPoint (previousSegmentPoint.X, 0),
						upperLeft: previousSegmentPoint,
						upperRight: currentSegmentPoint,
						lowerRight: new CCPoint (currentSegmentPoint.X, 0),
						texture: this.Texture,
						texturePropLowerLeft: new CCPoint (previousSegmentPropX, 0),
						texturePropUpperLeft: new CCPoint (previousSegmentPropX, 1),
						texturePropUpperRight: new CCPoint (currentSegmentPropX, 1),
						texturePropLowerRight: new CCPoint (currentSegmentPropX, 0)
					);

					previousSegmentPoint = currentSegmentPoint;
				}
			}

			geoBatch.End();
		}

		protected override void Draw ()
		{
			base.Draw ();

			// Draw debug output if requested
			// White line = straight line between hill keypoints.
			// Yellow curve = smoothed curve between hill keypoints.
			if (this.showDebug)
			{
				CCDrawingPrimitives.Begin ();

				// Visualize keypoints and smoothed segments.
				CCDrawingPrimitives.LineWidth = 2;
				for (int i = (int)Math.Max (1, this.minKeyPointIndex); i <= this.maxKeyPointIndex; i++)
				{
					var p0 = this.keyHillPoints [i - 1];
					var p1 = this.keyHillPoints [i];

					// Draw straight line between two keypoints.
					CCDrawingPrimitives.DrawLine (p0, p1, CCColor4B.White);

					// Draw smoothed curve.
					var segmentPoints = HillLayer.GetSmoothSegmentLocations (p0, p1, segmentWidthPixels);
					var previousSegmentPoint = segmentPoints [0];
					for (int segmentIndex = 1; segmentIndex < segmentPoints.Count; segmentIndex++)
					{
						var currentSegmentPoint = segmentPoints [segmentIndex];
						CCDrawingPrimitives.DrawLine (previousSegmentPoint, currentSegmentPoint, CCColor4B.Yellow);
						previousSegmentPoint = currentSegmentPoint;
					}
				}

				CCDrawingPrimitives.End ();
			}

			// Draw the textured quads.
			this.geoBatch.Draw ();
		}

		/// <summary>
		/// Gets the segment locations between two keypoints. Applies a smoothing function.
		/// </summary>
		/// <returns>The segment locations.</returns>
		/// <param name="keyPoint0">Key point 0.</param>
		/// <param name="keyPoint1">Key point 1.</param>
		/// <param name="segmentWidthPixels">width of one segment; the lower, the smoother the curve will be</param> 
		static IList<CCPoint> GetSmoothSegmentLocations(CCPoint keyPoint0, CCPoint keyPoint1, float segmentWidthPixels)
		{
			/*
			 * 
			 * We map a cosine curve from p0 to p1 to smooth the hills.
			 * The x-distance between p0 and p1 is divided into equally wide segments.
			 * We map cos(0) to p0 and cos(Pi) to p1. (cos(0) == 1 and cos(Pi) == -1)
			 * The values between are calculated for each segment.
			 * By multiplying the resulting values with the amplitude, we get a smoothed curve.
			*/

			var segmentPoints = new List<CCPoint> ();


			// Divide distance between two keypoints into equally wide segments.
			int numSegments = (int)Math.Floor((keyPoint1.X - keyPoint0.X) / segmentWidthPixels);

			// Calculate the exact x stepping for each segment.
			float pixelsDeltaX = (keyPoint1.X - keyPoint0.X) / numSegments;
			// Also divide range from 0 to Pi into equal parts.
			float deltaAnglePerSegment = CCMathHelper.Pi / numSegments;
			// Amplitude is half of the difference of the Y coordinates.
			float pixelsAmplitude = (keyPoint0.Y - keyPoint1.Y) / 2;
			// The middle of the two Y positions is our base line.
			float pixelsMidY = (keyPoint0.Y + keyPoint1.Y) / 2;

			CCPoint currentSegmentPoint;

			// Loop all segments and for each segment draw a line to the next segment's location.
			// The segment locations are calculated from the cosine function.
			for (int segment = 0; segment <= numSegments; segment++)
			{
				// X value is increased by stepping.
				currentSegmentPoint.X = keyPoint0.X + segment * pixelsDeltaX;
				// Y value is using smoothed curve.
				currentSegmentPoint.Y = pixelsMidY + pixelsAmplitude * (float)Math.Cos (deltaAnglePerSegment * segment);

				segmentPoints.Add (currentSegmentPoint);
			}

			return segmentPoints;
		}

	}
}
