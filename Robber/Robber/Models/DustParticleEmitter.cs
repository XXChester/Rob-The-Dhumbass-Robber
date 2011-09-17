using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
namespace Robber {
	public class DustParticleEmitter : IRenderable {
		#region Class variables
		private Texture2D particleTexture;
		private BaseParticle2DParams particleParms;
		private List<QuestionMarkParticle> dustParticles;
		private float elapsedSpawnTime;
		private float elapsedEmittingTime;
		private const float SPAWN_DELAY = 10f;
		private const float TIME_TO_LIVE = 2000f;
		private readonly Color COLOUR = new Color(210, 200, 190);
		private readonly Random RANDOM;
		private const int MAX_RANGE_FROM_EMITTER = 32;
		#endregion Class variables

		#region Class properties
		public Vector2 PlayerPosition { get; set; }
		#endregion Class properties

		#region Constructor
		public DustParticleEmitter(ContentManager content) {
			this.particleTexture = LoadingUtils.loadTexture2D(content, "Dust1");
			this.particleParms = new BaseParticle2DParams();
			this.particleParms.Scale = new Vector2(.25f);
			this.particleParms.Origin = new Vector2(32f, 32f);
			this.particleParms.Texture = this.particleTexture;
			this.particleParms.LightColour = COLOUR;
			this.particleParms.TimeToLive = TIME_TO_LIVE;
			this.dustParticles = new List<QuestionMarkParticle>();
			this.elapsedSpawnTime = SPAWN_DELAY;
			this.elapsedEmittingTime = 0f;
			this.RANDOM = new Random();
		}
		#endregion Constructor

		#region Support methods
		public void createParticle() {
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

			this.particleParms.Position = new Vector2(x, y);
			this.dustParticles.Add(new QuestionMarkParticle(this.particleParms));
			this.elapsedSpawnTime = 0f;
		}

		public void update(float elapsed) {
			this.elapsedSpawnTime += elapsed;
			this.elapsedEmittingTime += elapsed;
			List<int> indexesUpForRemoval = new List<int>();
			BaseParticle2D smokeParticle = null;
			for (int i = 0; i < this.dustParticles.Count; i++) {
				smokeParticle = this.dustParticles[i];
				smokeParticle.update(elapsed);
				if (smokeParticle.TimeAlive >= smokeParticle.TimeToLive) {
					// mark the particle for removal to avoid concurrent access violations
					indexesUpForRemoval.Add(i);
				}
			}
			for (int i = indexesUpForRemoval.Count - 1; i >= 0; i--) {
				this.dustParticles.RemoveAt(indexesUpForRemoval[i]);
			}
		}

		public void render(SpriteBatch spriteBatch) {
			if (this.dustParticles != null) {
				foreach (BaseParticle2D smokeParticle in this.dustParticles) {
					smokeParticle.render(spriteBatch);
				}
			}
		}
		#endregion Support methods

		#region Destructor
		public void dispose() {
			if (this.particleTexture != null) {
				this.particleTexture.Dispose();
			}
		}
		#endregion Destructor
	}
}
