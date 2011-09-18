using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
namespace Robber {
	public class QuestionMarkEmitter : BaseParticle2DEmitter {
		#region Class variables
		private int lastUsedIndex;
		private const float TIME_TO_LIVE = 2500f;
		private readonly Color COLOUR = Color.LightGreen;
		private readonly Vector2 MOVEMENT_SPEED = new Vector2(0f, -20f / 1000);//speed per second
		private readonly Vector2[] SPAWN_LOCATIONS = new Vector2[] {
			new Vector2(348f, 122f),
			new Vector2(390f, 125f),
			new Vector2(441f, 122f)
		};
		public const float SPAWN_DELAY = 500f;
		#endregion Class variables

		#region Class properties
		public bool Emitt { get; set; }
		#endregion Class properties

		#region Constructor
		public QuestionMarkEmitter(BaseParticle2DEmitterParams parms)
			: base (parms) {
			BaseParticle2DParams particleParams = new BaseParticle2DParams();
			particleParams.Scale = new Vector2(.5f);
			particleParams.Origin = new Vector2(32f, 32f);
			particleParams.Texture = parms.ParticleTexture;
			particleParams.LightColour = COLOUR;
			particleParams.TimeToLive = TIME_TO_LIVE;
			particleParams.Direction = MOVEMENT_SPEED;
			base.particleParams = particleParams;
		}
		#endregion Constructor

		#region Support methods
		public override void createParticle() {
			int nextIndex = -1;
			do {
				nextIndex = RANDOM.Next(this.SPAWN_LOCATIONS.Length);
			} while (this.lastUsedIndex == nextIndex);
			this.lastUsedIndex = nextIndex;
			base.particleParams.Position = this.SPAWN_LOCATIONS[this.lastUsedIndex];
			base.particles.Add(new QuestionMarkParticle(base.particleParams));
			base.createParticle();
		}

		public override void update(float elapsed) {
			base.update(elapsed);
			// add any new particles if required
			if (this.Emitt && this.elapsedSpawnTime >= SPAWN_DELAY) {
				createParticle();
			}
		}
		#endregion Support methods
	}
}
