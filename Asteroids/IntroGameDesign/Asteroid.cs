using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IntroGameDesign {
	public class Asteroid : DrawableGameComponent {
		#region Static Area
		private static Random s_rng;
		private static List<Texture2D> AsteroidSprites;
		static Asteroid() {
			s_rng = new Random();
			AsteroidSprites = new List<Texture2D>();
		}
		#endregion

		private int SpriteIndex;
		private Vector2 Trajectory;

		public Vector2 Position;
		public Texture2D Sprite {
			get {
				return AsteroidSprites[SpriteIndex];
			}
		}

		private float Radius;
		public BoundingSphere Bounds {
			get {
				return new BoundingSphere( new Vector3( Position.X + Radius, Position.Y + Radius, 0 ), Radius );
			}
		}

		public Asteroid( Game game ) : base( game ) {
		}

		public override void Initialize() {
			Trajectory = new Vector2( s_rng.Next( -5, 5 ), s_rng.Next( 1, 10 ) );
			base.Initialize();
		}

		protected override void LoadContent() {
			if( 0 == AsteroidSprites.Count ) {
				AsteroidSprites.Add( Game.Content.Load<Texture2D>( "Asteroids\\blue-one" ) );
				AsteroidSprites.Add( Game.Content.Load<Texture2D>( "Asteroids\\cool-rock" ) );
				AsteroidSprites.Add( Game.Content.Load<Texture2D>( "Asteroids\\exploder" ) );
				AsteroidSprites.Add( Game.Content.Load<Texture2D>( "Asteroids\\frozen" ) );
				AsteroidSprites.Add( Game.Content.Load<Texture2D>( "Asteroids\\green-chunk" ) );
				AsteroidSprites.Add( Game.Content.Load<Texture2D>( "Asteroids\\hot-rock" ) );
				AsteroidSprites.Add( Game.Content.Load<Texture2D>( "Asteroids\\hunk-o-rock" ) );
				AsteroidSprites.Add( Game.Content.Load<Texture2D>( "Asteroids\\magma" ) );
			}

			SpriteIndex = s_rng.Next( 0, AsteroidSprites.Count );
			Position = new Vector2( s_rng.Next( 0, Game.GraphicsDevice.Viewport.Width ), Sprite.Height * -1 );

			Radius = Sprite.Width / 2;
			base.LoadContent();
		}

		public override void Update( GameTime gameTime ) {
			Position += Trajectory;

			if( ( Position.Y > Game.GraphicsDevice.Viewport.Height ) ||	// Off the bottom of the screen
				( Position.X > Game.GraphicsDevice.Viewport.Width ) ||	// Off the right side of the screen
				( Position.X < ( Sprite.Width * -1 ) ) ) {				// Off the left side of the screen
				// NOTE: You'd probably want to pool "dead" Asteroids in a real game
				// This would reduce the number you had to create from scratch every time.
				if( Game.Components.Remove( this ) ) {
					Dispose();
				}
			}

			base.Update( gameTime );
		}

		public override void Draw( GameTime gameTime ) {
			( Game as Game1 ).spriteBatch.Draw( Sprite, Position, Color.White );
			base.Draw( gameTime );
		}
	}
}
