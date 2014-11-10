using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IntroGameDesign {
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game {
		public GraphicsDeviceManager graphics;
		public SpriteBatch spriteBatch;

		TimeSpan TimeToAsteroid;

		#region Components
		Player Player1;
		List<Bullet> Bullets;
		List<Asteroid> Asteroids;
		#endregion

		SpriteFont CourierNew12;
		public Game1() {
			graphics = new GraphicsDeviceManager( this );
			Content.RootDirectory = "Content";

			Player1 = new Player( this );
			Bullets = new List<Bullet>();
			Asteroids = new List<Asteroid>();

			Components.ComponentAdded += ComponentAdded;
			Components.ComponentRemoved += ComponentRemoved;
		}

		void ComponentAdded( object sender, GameComponentCollectionEventArgs e ) {
			if( e.GameComponent is Bullet )
				Bullets.Add( e.GameComponent as Bullet );
			else if( e.GameComponent is Asteroid )
				Asteroids.Add( e.GameComponent as Asteroid );
		}

		void ComponentRemoved( object sender, GameComponentCollectionEventArgs e ) {
			if( e.GameComponent is Bullet )
				Bullets.Remove( e.GameComponent as Bullet );
			else if( e.GameComponent is Asteroid )
				Asteroids.Remove( e.GameComponent as Asteroid );
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize() {
			Components.Add( Player1 );
			Components.Add( new Asteroid( this ) );

			TimeToAsteroid = TimeSpan.FromMilliseconds( 500 );
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch( GraphicsDevice );

			Bullet.Sprite = Content.Load<Texture2D>( "Bullet" );
			CourierNew12 = Content.Load<SpriteFont>( "ScoreFont" );
			base.LoadContent();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() {
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update( GameTime gameTime ) {
			if( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed )
				this.Exit();

			base.Update( gameTime );

			if( TimeToAsteroid <= TimeSpan.Zero ) {
				Components.Add( new Asteroid( this ) );
				TimeToAsteroid = TimeSpan.FromMilliseconds( 500 );
			} else {
				TimeToAsteroid -= gameTime.ElapsedGameTime;
			}

			for( int _AsteroidIndex = 0; _AsteroidIndex < Asteroids.Count; ++_AsteroidIndex ) {
				bool _AsteroidDestroyed = false;
				for( int _BulletIndex = 0; _BulletIndex < Bullets.Count; ++_BulletIndex ) {
					if( ContainmentType.Disjoint != Asteroids[_AsteroidIndex].Bounds.Contains( Bullets[_BulletIndex].Bounds ) ) {
						_AsteroidDestroyed = true;
						Components.Remove( Asteroids[_AsteroidIndex--] );
						Components.Remove( Bullets[_BulletIndex--] );
						Player1.Score += 1000;
						break;
					}
				}

				if( _AsteroidDestroyed )
					continue;

				if( ContainmentType.Disjoint != Asteroids[_AsteroidIndex].Bounds.Contains( Player1.Bounds ) ) {
					Components.Remove( Asteroids[_AsteroidIndex--] );
					if( 0 == Player1.Lives-- ) {
						Player1.Lives = 0;
						Components.Remove( Player1 );
					} else {
						Player1.Position = new Vector2( GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - Player1.Sprite.Height );
					}
				}
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw( GameTime gameTime ) {
			GraphicsDevice.Clear( Color.Black );
			spriteBatch.Begin();

			spriteBatch.DrawString( CourierNew12, string.Format( "Score: {0}\nLives: {1}", Player1.Score, Player1.Lives ), new Vector2( 50, 50 ), Color.Red );
			base.Draw( gameTime );

			spriteBatch.End();
		}
	}
}
