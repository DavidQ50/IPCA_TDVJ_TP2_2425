using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using static Zelda.Game1;

namespace Zelda
{
    public class Player
    {
        public Point Position { get; set; }//Posição atual em tiles
        private Game1 game;
        // Variáveis para movimento suave
        private Vector2 pixelPosition;//Posição precisa em pixels (para movimento suave)
        private float moveSpeed = 200f; //Velocidade de movimento (200 pixels/segundo)
        private Direction lastMoveDirection;
        private bool isMoving = false;
        public Texture2D Texture { get; set; }
        public int TileSize { get; set; }
        private Texture2D spriteSheet;//Sistema de animação por spritesheet
        private Rectangle[] frames;
        private int currentFrame;
        private float frameTime = 0.1f; // Tempo entre frames 
        private float elapsedTime;
        private int animationDirection;
        public List<Projectile> Projectiles { get; private set; } = new List<Projectile>();//Lista de projéteis ativos
        private float attackCooldown = 0.3f;//Tempo entre ataques (0.3 segundos)
        private float currentCooldown = 0;
        private Texture2D projectileTexture;

        public Player(Game1 game, int x, int y)
        {
            this.game = game;
            Position = new Point(x, y);
            TileSize = game.TileSize; // Armazena o tileSize localmente
            pixelPosition = new Vector2(x * game.TileSize, y * game.TileSize);
        }

        public void Update(GameTime gameTime)//Lógica de movimento, animação e ataque
        {
            KeyboardState kstate = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 movement = Vector2.Zero;
            Direction currentDirection = lastMoveDirection;

            // Verificar teclas pressionadas
            if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A))
            {
                movement.X = -1;
                currentDirection = Direction.Left;
            }
            else if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D))
            {
                movement.X = 1;
                currentDirection = Direction.Right;
            }

            if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W))
            {
                movement.Y = -1;
                currentDirection = Direction.Up;
            }
            else if (kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.S))
            {
                movement.Y = 1;
                currentDirection = Direction.Down;
            }

            // Normalizar movimento diagonal
            if (movement != Vector2.Zero)
            {
                movement.Normalize();
                game.direction = currentDirection;
                lastMoveDirection = currentDirection;
            }

            // Calcular nova posição em pixels
            Vector2 newPixelPosition = pixelPosition + movement * moveSpeed * deltaTime;
            Point newTilePosition = new Point(
                (int)(newPixelPosition.X / game.TileSize),
                (int)(newPixelPosition.Y / game.TileSize));

            // Verificar se o movimento muda de tile
            bool changedTile = (Position != newTilePosition);

            if (changedTile)
            {
                // Verificar se pode mover para o novo tile
                if (game.FreeTile(newTilePosition.X, newTilePosition.Y))
                {
                    // Verificar colisão com caixas
                    if (game.HasBox(newTilePosition.X, newTilePosition.Y))
                    {
                        int dx = newTilePosition.X - Position.X;
                        int dy = newTilePosition.Y - Position.Y;
                        Point boxTarget = new Point(newTilePosition.X + dx, newTilePosition.Y + dy);

                        if (game.FreeTile(boxTarget.X, boxTarget.Y))
                        {
                            // Mover a caixa
                            for (int i = 0; i < game.boxes.Count; i++)
                            {
                                if (game.boxes[i].X == newTilePosition.X && game.boxes[i].Y == newTilePosition.Y)
                                {
                                    game.boxes[i] = boxTarget;
                                    Position = newTilePosition;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Position = newTilePosition;
                    }
                }
            }
            

            // Atualizar posição em pixels (com limites para não ultrapassar o tile atual)
            if (Position.X == newTilePosition.X)
            {
                pixelPosition.X = newPixelPosition.X;
            }
            else
            {
                pixelPosition.X = Position.X * game.TileSize;
            }

            if (Position.Y == newTilePosition.Y)
            {
                pixelPosition.Y = newPixelPosition.Y;
            }
            else
            {
                pixelPosition.Y = Position.Y * game.TileSize;
            }

            // Atualizar estado de movimento para animações
            isMoving = (movement != Vector2.Zero);

            // Atualizar animação
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime >= frameTime)
            {
                elapsedTime = 0;
                currentFrame = (currentFrame + 1) % 4; // Cicla entre 0-3
            }

            // Definir direção da animação (baseado no seu enum Direction)
            animationDirection = game.direction switch
            {
                Direction.Down => 0,  // Primeira linha da spritesheet
                Direction.Left => 2,  // Segunda linha
                Direction.Right => 3, // Terceira linha 
                Direction.Up => 1,    // Quarta linha
                _ => 0
            };

            // Atualiza cooldown
            if (currentCooldown > 0)
            {
                currentCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Verifica disparo
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && currentCooldown <= 0)
            {
                Vector2 direction = Vector2.Zero;

                switch (game.direction)
                {
                    case Direction.Up: direction = new Vector2(0, -1); break;
                    case Direction.Down: direction = new Vector2(0, 1); break;
                    case Direction.Left: direction = new Vector2(-1, 0); break;
                    case Direction.Right: direction = new Vector2(1, 0); break;
                }

                Vector2 startPos = pixelPosition + new Vector2(game.TileSize / 2, game.TileSize / 2);
                Projectiles.Add(new Projectile(game, startPos, direction, 300f));
                Projectiles[Projectiles.Count - 1].LoadContent(projectileTexture);
                currentCooldown = attackCooldown;
            }

            // Atualiza projéteis
            foreach (var projectile in Projectiles.ToList())
            {
                projectile.Update(gameTime);
                if (!projectile.Active) Projectiles.Remove(projectile);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenPosition)//Desenha o jogador
        {
            // Desenhar o jogador na posição em pixels
            int frameIndex = animationDirection * 4 + currentFrame;
            Vector2 drawPosition = pixelPosition - screenPosition;

            spriteBatch.Draw(
                spriteSheet,
                drawPosition,
                frames[frameIndex],
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                (game.direction == Direction.Right) ? SpriteEffects.None : SpriteEffects.None,
                0f
            );
        }
            public void DrawProjectiles(SpriteBatch spriteBatch, Vector2 screenOffset)//Desenha todos os projéteis
        {
            foreach (var projectile in Projectiles)
                {
                    projectile.Draw(spriteBatch, screenOffset);
                }
            }
        

        public void LoadContent(Texture2D sheet)//Carrega a spritesheet e configura animações
        {
            spriteSheet = sheet;
            int frameWidth = sheet.Width / 4;  // 4 frames por linha
            int frameHeight = sheet.Height / 4; // 4 direções

            frames = new Rectangle[4 * 4]; // 4 direções x 4 frames

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    frames[y * 4 + x] = new Rectangle(
                        x * frameWidth,
                        y * frameHeight,
                        frameWidth,
                        frameHeight
                    );
                }
            }
        }

        public void LoadProjectileTexture(Texture2D tex)//Configura textura dos projéteis
        {
            projectileTexture = tex;
        }
    }
}


