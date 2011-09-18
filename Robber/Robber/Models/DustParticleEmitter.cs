using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
namespace Robber {
	public class DustParticleEmitter : BaseParticle2DEmitter {
		#region Class variables
		private const float TIME_TO_LIVE = 4000f;
		private const int MAX_RANGE_FROM_EMITTER = 32;
		public const float SPAWN_DELAY = 10f;
		public static Color COLOUR = new Color(210, 200, 190);
		#endregion Class variables

		#region Class properties
		public Vector2 PlayerPosition { get; set; }
		#endregion Class properties

		#region Constructor
		public DustParticleEmitter(BaseParticle2DEmitterParams parms)
			:base(parms) {
			BaseParticle2DParams particleParams = new BaseParticle2DParams();
			particleParams.Scale = new Vector2(.25f);
			particleParams.Origin = new Vector2(32f, 32f);
			particleParams.Texture = parms.ParticleTexture;
			particleParams.LightColour = COLOUR;
			particleParams.TimeToLive = TIME_TO_LIVE;
			base.particleParams = particleParams;
		}
		#endregion Constructor

		#region Support methods
		public void updateColours(Color colour) {
			if (base.particles != null) {
				foreach (BaseParticle2D particle in base.particles) {
					particle.LightColour = colour;
				}
			}
		}

		public override void createParticle() {
			int positionX = this.RANDOM.Next(MAX_RANGE_FROM_EMITTER);
			int positionY = this.RANDOM.Next(MAX_RANGE_FROM_EMITTER);
			int directionX = this.RANDOM.Next(2);
			int directionY = this.RANDOM.Next(2);
			float x, y;
			if (directionX == 0) {
				x = PlayerPosition.X + positionX;
			} else {
				x = PlayerPosition.X - positionX;
			}

			if (directionY == 0) {
				y = PlayerPosition.Y + positionY;
			} else {
				y = PlayerPosition.Y - positionY;
			}

			base.particleParams.Position = new Vector2(x, y);
			base.particles.Add(new QuestionMarkParticle(base.particleParams));
			base.createParticle();
		}
		#endregion Support methods
	}
}
