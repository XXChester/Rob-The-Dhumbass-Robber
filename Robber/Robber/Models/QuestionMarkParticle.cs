﻿using Microsoft.Xna.Framework;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
namespace Robber {
	public class QuestionMarkParticle : BaseParticle2D {
		#region Class variables
		private readonly Vector2 SCALE_BY = new Vector2(1f / 1000f, .75f / 1000f);//scale per second
		private readonly float ROTATION_SPEED = 20f / 1000f;//rotation per second
		#endregion Class variables

		#region Constructor
		public QuestionMarkParticle(BaseParticle2DParams parms)
			: base(parms) {
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			base.update(elapsed);
			base.fadeOutAsLifeProgresses();
			base.scaleAsLifeProgresses(SCALE_BY);
			base.rotateAsLifeProgresses(ROTATION_SPEED);
			base.Position += (base.Direction * elapsed);
		}
		#endregion Support methods
	}
}
