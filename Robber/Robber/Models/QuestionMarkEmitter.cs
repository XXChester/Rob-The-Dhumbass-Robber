using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
namespace Robber {
	public class QuestionMarkEmitter : IRenderable {
		#region Class variables
		private Texture2D particleTexture;
		private BaseParticle2DParams particleParms;
		private List<QuestionMarkParticle> questionMarkParticles;
		private float elapsedSpawnTime;
		private float elapsedEmittingTime;
		private int lastUsedIndex;
		private const float SPAWN_DELAY = 500f;
		private const float TIME_TO_LIVE = 2000f;
		private readonly Color COLOUR = Color.LightGreen;
		private readonly Random RANDOM;
		private readonly Vector2 MOVEMENT_SPEED = new Vector2(0f, -20f / 1000);//speed per second
		private readonly Vector2[] SPAWN_LOCATIONS = new Vector2[] {
			new Vector2(348f, 122f),
			new Vector2(390f, 125f),
			new Vector2(441f, 122f)
		};
		#endregion Class variables

		#region Class properties
		public bool Emitt { get; set; }
		#endregion Class properties

		#region Constructor
		public QuestionMarkEmitter(ContentManager content) {
			this.particleTexture = LoadingUtils.loadTexture2D(content, "QuestionParticle");
			this.particleParms = new BaseParticle2DParams();
			this.particleParms.Scale = new Vector2(.5f);
			this.particleParms.Origin = new Vector2(32f, 32f);
			this.particleParms.Texture = this.particleTexture;
			this.particleParms.LightColour = COLOUR;
			this.particleParms.TimeToLive = TIME_TO_LIVE;
			this.particleParms.Direction = MOVEMENT_SPEED;
			this.questionMarkParticles = new List<QuestionMarkParticle>();
			this.elapsedSpawnTime = SPAWN_DELAY;
			this.elapsedEmittingTime = 0f;
			this.RANDOM = new Random();
		}
		#endregion Constructor

		#region Support methods
		public void update(float elapsed) {
			this.elapsedSpawnTime += elapsed;
			this.elapsedEmittingTime += elapsed;
			List<int> indexesUpForRemoval = new List<int>();
			BaseParticle2D smokeParticle = null;
			for (int i = 0; i < this.questionMarkParticles.Count; i++) {
				smokeParticle = this.questionMarkParticles[i];
				smokeParticle.update(elapsed);
				if (smokeParticle.TimeAlive >= smokeParticle.TimeToLive) {
					// mark the particle for removal to avoid concurrent access violations
					indexesUpForRemoval.Add(i);
				}
			}
			for (int i = 0; i < indexesUpForRemoval.Count; i++) {
				this.questionMarkParticles.RemoveAt(indexesUpForRemoval[i]);
			}

			// update our existing particles positions
			for (int i = 0; i < this.questionMarkParticles.Count; i++) {
				smokeParticle = this.questionMarkParticles[i];
				if (smokeParticle != null) {
					smokeParticle.Position = smokeParticle.Position + (smokeParticle.Direction * elapsed);
				}
			}

			// add any new particles if required
			if (this.Emitt && this.elapsedSpawnTime >= SPAWN_DELAY) {
				int nextIndex = -1;
				do {
					nextIndex = RANDOM.Next(this.SPAWN_LOCATIONS.Length);
				} while (this.lastUsedIndex == nextIndex);
				this.lastUsedIndex = nextIndex;
				this.particleParms.Position = this.SPAWN_LOCATIONS[this.lastUsedIndex];
				this.questionMarkParticles.Add(new QuestionMarkParticle(this.particleParms));
				this.elapsedSpawnTime = 0f;
			}
		}

		public void render(SpriteBatch spriteBatch) {
			if (this.questionMarkParticles != null) {
				foreach (BaseParticle2D smokeParticle in this.questionMarkParticles) {
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
