using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zelda
{
    public class Enemy
    {
        public Point Position { get; private set; }
        private Game1 game;
        private Vector2 pixelPosition;
        private float moveSpeed = 1.5f;

        // Variáveis para animação
        private Texture2D spriteSheet;
        private Rectangle[] frames;
        private int currentFrame;
        private float frameTime = 0.15f;
        private float elapsedTime;
        private int animationDirection; // 0=down, 1=up, 2=left, 3=right

        public Enemy(Game1 game, int x, int y)
        {
            this.game = game;
            Position = new Point(x, y);
            pixelPosition = new Vector2(x * game.TileSize, y * game.TileSize);
        }

        public void LoadContent(Texture2D sheet)
        {
            spriteSheet = sheet;

            // Configuração dos frames da spritesheet (assumindo layout 4x4 como o player)
            int frameWidth = sheet.Width / 4;
            int frameHeight = sheet.Height / 4;

            frames = new Rectangle[4 * 4]; // 4 direções x 4 frames

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    frames[y * 4 + x] = new Rectangle(
                        x * frameWidth,
                        y * frameHeight,
                        frameWidth,
                        frameHeight);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            // Verificação de tela (como antes)
            bool playerInSameScreen =
                game.currentScreen.X == Position.X / Game1.screenWidthInTiles &&
                game.currentScreen.Y == Position.Y / Game1.screenHeightInTiles;

            Vector2 direction = Vector2.Zero;

            if (playerInSameScreen)
            {
                // Determina direção para o jogador
                if (game.player.Position.X < Position.X)
                {
                    direction.X = -1;
                    animationDirection = 2; // Left
                }
                else if (game.player.Position.X > Position.X)
                {
                    direction.X = 1;
                    animationDirection = 3; // Right
                }

                if (game.player.Position.Y < Position.Y)
                {
                    direction.Y = -1;
                    animationDirection = 1; // Up
                }
                else if (game.player.Position.Y > Position.Y)
                {
                    direction.Y = 1;
                    animationDirection = 0; // Down
                }

                if (direction != Vector2.Zero)
                    direction.Normalize();

                Vector2 newPixelPosition = pixelPosition + direction * moveSpeed * game.TileSize *
                                         (float)gameTime.ElapsedGameTime.TotalSeconds;

                Point newTilePosition = new Point(
                    (int)(newPixelPosition.X / game.TileSize),
                    (int)(newPixelPosition.Y / game.TileSize));

                if (game.FreeTile(newTilePosition.X, newTilePosition.Y))
                {
                    Position = newTilePosition;
                    pixelPosition = newPixelPosition;

                    // Atualiza animação apenas quando está se movendo
                    elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (elapsedTime >= frameTime)
                    {
                        elapsedTime = 0;
                        currentFrame = (currentFrame + 1) % 4; // Cicla entre 0-3
                    }
                }
            }
            else
            {
                // Reseta animação quando não está se movendo
                currentFrame = 0;
                elapsedTime = 0;
            }

            // Verifica colisão com o jogador
            if (Position == game.player.Position)
            {
                game.Exit(); // Fecha o jogo imediatamente
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenOffset)
        {
            int frameIndex = animationDirection * 4 + currentFrame;
            Vector2 drawPosition = new Vector2(
                pixelPosition.X - screenOffset.X,
                pixelPosition.Y - screenOffset.Y);

            spriteBatch.Draw(
                spriteSheet,
                drawPosition,
                frames[frameIndex],
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f);
        }
    }
}
