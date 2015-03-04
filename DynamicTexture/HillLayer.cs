using System;
using System.Collections.Generic;
using CocosSharp;

namespace DynamicTexture
{
	public class HillLayer : CCLayerColor
	{
		public HillLayer () : base(CCColor4B.Gray)
		{

			this.Schedule ();
		}
	

		HillNode hillNode;

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			this.hillNode = new HillNode (this.ContentSize, new CCTexture2D("texture.png")) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Position = this.ContentSize.Center
			};
			this.AddChild (this.hillNode);
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
	}
}
