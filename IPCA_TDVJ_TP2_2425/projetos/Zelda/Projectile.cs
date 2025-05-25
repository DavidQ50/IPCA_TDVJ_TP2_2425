using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zelda
{
    public class Projectile
    {
        public Vector2 Position { get; private set; }
        public Vector2 Direction { get; private set; }
        public float Speed { get; private set; }
        public bool Active { get; private set; }
        private Game1 game;
        private Texture2D texture;

        public Projectile(Game1 game, Vector2 startPosition, Vector2 direction, float speed)
        {
            this.game = game;
            Position = startPosition;
            Direction = direction;
            Speed = speed;
            Active = true;
        }

        public void LoadContent(Texture2D tex)
        {
            texture = tex;
        }

        public void Update(GameTime gameTime)
        {
            if (!Active) return;

            Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Verifica se saiu da tela
            if (Position.X < 0 || Position.X > game.mapWidth * game.TileSize ||
                Position.Y < 0 || Position.Y > game.mapHeight * game.TileSize)
            {
                Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenOffset)
        {
            if (!Active) return;

            Vector2 drawPosition = Position - screenOffset;
            float rotation = 0f;
            SpriteEffects effects = SpriteEffects.None;

            // Ajusta a rotação baseada na direção (apenas 4 direções)
            switch (GetDirectionIndex(Direction))
            {
                case 0: // Cima
                    rotation = MathHelper.PiOver2; // 90 graus
                    break;
                case 1: // Baixo
                    rotation = -MathHelper.PiOver2; // -90 graus
                    break;
                case 2: // Esquerda
                    effects = SpriteEffects.None; // Sprite já aponta para esquerda
                    break;
                case 3: // Direita
                    effects = SpriteEffects.FlipHorizontally; // Flip horizontal
                    break;
            }

            spriteBatch.Draw(
                texture,
                drawPosition,
                null,
                Color.White,
                rotation,
                Vector2.Zero, // Origem no canto superior esquerdo
                1f,
                effects,
                0f);
        }

        // Auxiliar: Converte vetor direção para índice (0-3)
        private int GetDirectionIndex(Vector2 dir)
        {
            if (dir.Y < 0) return 0;    // Cima
            if (dir.Y > 0) return 1;    // Baixo
            if (dir.X < 0) return 2;    // Esquerda
            return 3;                   // Direita
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
        }
    }
}
 