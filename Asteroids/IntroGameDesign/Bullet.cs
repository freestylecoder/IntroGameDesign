using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IntroGameDesign {
	public class Bullet : DrawableGameComponent {
		private Player m_Owner;
		
		public Vector2 Position;
		public static Texture2D Sprite;

		private float Radius;
		public BoundingSphere Bounds {
			get {
				return new BoundingSphere( new Vector3( Position.X + Radius, Position.Y + Radius, 0 ), Radius );
			}
		}

		public Bullet( Game p_game, Player p_player )
			: base( p_game ) {
			m_Owner = p_player;
		}

		public override void Initialize() {
			base.Initialize();
		}

		protected override void LoadContent() {
			if( null == Sprite )
				Sprite = Game.Content.Load<Texture2D>( "Bullet" );

			Position = new Vector2( m_Owner.Position.X + ( m_Owner.Sprite.Width / 2 ), m_Owner.Position.Y );
			Radius = Sprite.Width / 2;
			base.LoadContent();
		}

		public override void Update( GameTime gameTime ) {
			Position -= new Vector2( 0, 10 );

			if( Position.Y < ( Sprite.Height * -1 ) ) {
				if( Game.Components.Remove( this ) ) {
					Dispose();
				}
			}

			base.Update( gameTime );
		}

		public override void Draw( GameTime gameTime ) {
			( Game as Game1 ).spriteBatch.Draw( Sprite, Position, Color.Red );
			base.Draw( gameTime );
		}
	}
}
