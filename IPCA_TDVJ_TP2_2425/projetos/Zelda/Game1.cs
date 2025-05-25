using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework.Media;
using System;

namespace Zelda
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        public enum Direction { Up, Down, Left, Right }
        public Direction direction = Direction.Down;

        public List<Point> boxes = new List<Point>();
        public Player player;

        Texture2D playerTexture, boxTexture, floorTexture, wallTexture;

        bool[,] mapData;
        public int mapWidth { get; private set; }
        public int mapHeight { get; private set; }

        public int TileSize { get; } = 64;
        public const int screenWidthInTiles = 16;
        public const int screenHeightInTiles = 12;

        public Point currentScreen = Point.Zero; // Começa na tela (0,0)

        public Vector2 cameraPosition = Vector2.Zero;
        private float cameraLerpSpeed = 0.1f; // Suavização do movimento

        public List<Enemy> enemies = new List<Enemy>();
        //Texture2D enemyTexture;

        private Song _song;
        private float _volume = 0.5f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = screenWidthInTiles * TileSize;
            _graphics.PreferredBackBufferHeight = screenHeightInTiles * TileSize;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            LoadMap("map.txt");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Criar texturas simples
            playerTexture = CreateColoredTexture(Color.Blue);
            boxTexture = CreateColoredTexture(Color.SaddleBrown);
            floorTexture = Content.Load<Texture2D>("thornfloor damp");
            wallTexture = CreateColoredTexture(Color.DarkSlateGray);

            // Inicializar o player com as texturas necessárias
            if (player != null)
            {
                player.Texture = playerTexture;
                player.TileSize = TileSize;
            }

            Texture2D playerSheet = Content.Load<Texture2D>("player");
            player.LoadContent(playerSheet);

            // Carrega as texturas dos inimigos
            foreach (var enemy in enemies)
            {
                Texture2D enemySheet = Content.Load<Texture2D>("enemy");

                foreach (var currentEnemy in enemies)
                {
                    currentEnemy.LoadContent(enemySheet);
                }
            }

            Texture2D projectileTexture = Content.Load<Texture2D>("projectile");
            player.LoadProjectileTexture(projectileTexture);

            // Carrega Sons - Musica
            _song = Content.Load<Song>("sound");
            MediaPlayer.Volume = 0.25f;
            MediaPlayer.Play(_song);
            MediaPlayer.IsRepeating = true; // Ativa o loop
          
        }

        private Texture2D CreateColoredTexture(Color color)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, TileSize, TileSize);
            Color[] data = new Color[TileSize * TileSize];
            for (int i = 0; i < data.Length; i++) data[i] = color;
            texture.SetData(data);
            return texture;
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateCamera();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player?.Update(gameTime);
            

            // Atualiza todos os inimigos
            foreach (var enemy in enemies)
            {
                enemy.Update(gameTime);
            }

            CheckScreenTransition();

            base.Update(gameTime);

            foreach (var projectile in player.Projectiles.ToList())
            {
                foreach (var enemy in enemies.ToList())
                {
                    Rectangle projBounds = projectile.GetBounds();
                    Rectangle enemyBounds = new Rectangle(
                        enemy.Position.X * TileSize,
                        enemy.Position.Y * TileSize,
                        TileSize,
                        TileSize);

                    if (projBounds.Intersects(enemyBounds))
                    {
                        player.Projectiles.Remove(projectile);
                        enemies.Remove(enemy);
                        break;
                    }
                }
            }

            if (enemies.Count == 0)
            {
                Exit(); // Fecha o jogo
            }

            base.Update(gameTime);

            // Controla o volume da musica
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                _volume += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                _volume -= 0.1f;
            }
            // Garante que o volume fique entre 0 e 1
            _volume = (float)Math.Clamp(_volume, 0.0, 1.0);
            // Modifica o volume da música
            MediaPlayer.Volume = _volume;

        }

        private void CheckScreenTransition()
        {
            if (player == null) return;

            // Atualizar currentScreen baseado na posição do jogador
            Point newScreen = new Point(
                player.Position.X / screenWidthInTiles,
                player.Position.Y / screenHeightInTiles);

            if (newScreen != currentScreen)
            {
                currentScreen = newScreen;

                // Garantir que currentScreen não saia dos limites
                currentScreen.X = MathHelper.Clamp(currentScreen.X, 0, (mapWidth / screenWidthInTiles) - 1);
                currentScreen.Y = MathHelper.Clamp(currentScreen.Y, 0, (mapHeight / screenHeightInTiles) - 1);
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            // Calcular deslocamento da tela atual em pixels
            int screenPixelOffsetX = currentScreen.X * screenWidthInTiles * TileSize;
            int screenPixelOffsetY = currentScreen.Y * screenHeightInTiles * TileSize;

            // Calcula a posição de repetição baseada na câmera
            Vector2 repeatOffset = new Vector2(
                (int)cameraPosition.X % TileSize,
                (int)cameraPosition.Y % TileSize
            );

            // Desenha o piso repetido
            for (int x = -1; x <= screenWidthInTiles; x++)
            {
                for (int y = -1; y <= screenHeightInTiles; y++)
                {
                    Vector2 pos = new Vector2(
                        x * TileSize - repeatOffset.X,
                        y * TileSize - repeatOffset.Y
                    );
                    _spriteBatch.Draw(floorTexture, pos, Color.White);
                }
            }

            // Desenhar a porção visível do mapa
            for (int x = 0; x < screenWidthInTiles; x++)
            {
                for (int y = 0; y < screenHeightInTiles; y++)
                {
                    int worldX = x + (currentScreen.X * screenWidthInTiles);
                    int worldY = y + (currentScreen.Y * screenHeightInTiles);

                    if (worldX >= 0 && worldX < mapWidth && worldY >= 0 && worldY < mapHeight)
                    {
                        Texture2D tex = mapData[worldX, worldY] ? floorTexture : wallTexture;
                        _spriteBatch.Draw(tex, new Vector2(x * TileSize, y * TileSize), Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(wallTexture, new Vector2(x * TileSize, y * TileSize), Color.White);
                    }
                }
            }

            // Desenhar caixas visíveis
            foreach (var box in boxes)
            {
                int boxScreenX = box.X - (currentScreen.X * screenWidthInTiles);
                int boxScreenY = box.Y - (currentScreen.Y * screenHeightInTiles);

                if (boxScreenX >= 0 && boxScreenX < screenWidthInTiles &&
                    boxScreenY >= 0 && boxScreenY < screenHeightInTiles)
                {
                    _spriteBatch.Draw(boxTexture,
                        new Vector2(boxScreenX * TileSize, boxScreenY * TileSize),
                        Color.White);
                }
            }

            // Desenhar jogador (sempre visível)
            if (player != null)
            {
                player.Draw(_spriteBatch, new Vector2(
                    currentScreen.X * screenWidthInTiles * TileSize,
                    currentScreen.Y * screenHeightInTiles * TileSize));
            }

            // Desenha todos os inimigos
            foreach (var enemy in enemies)
            {
                enemy.Draw(_spriteBatch, new Vector2(
                    currentScreen.X * screenWidthInTiles * TileSize,
                    currentScreen.Y * screenHeightInTiles * TileSize));
            }

            player.DrawProjectiles(_spriteBatch, new Vector2(
        currentScreen.X * screenWidthInTiles * TileSize,
        currentScreen.Y * screenHeightInTiles * TileSize));

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public bool HasBox(int x, int y)
        {
            return boxes.Any(b => b.X == x && b.Y == y);
        }

        public bool FreeTile(int x, int y)
        {
            // Verificar limites do mapa
            if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight)
                return false;

            // Verificar se o tile é caminhável
            if (!mapData[x, y])
                return false;

            // Verificar se há uma caixa no tile
            if (HasBox(x, y))
                return false;

            return true;
        }

        void LoadMap(string filePath)
        {
            string[] lines = File.ReadAllLines(Path.Combine(Content.RootDirectory, filePath));

            mapWidth = lines.Max(line => line.Length);
            mapHeight = lines.Length;

            mapData = new bool[mapWidth, mapHeight];
            boxes = new List<Point>();

            for (int y = 0; y < lines.Length; y++)
            {
                string line = lines[y].PadRight(mapWidth); // evita linhas curtas
                for (int x = 0; x < line.Length; x++)
                {
                    char tile = line[x];
                    switch (tile)
                    {
                        case 'X':
                            mapData[x, y] = false; // Parede
                            break;
                        case ' ':
                            mapData[x, y] = true; // Chão
                            break;
                        case '#':
                            mapData[x, y] = true; // Chão com caixa
                            boxes.Add(new Point(x, y));
                            break;
                        case 'Y':
                            mapData[x, y] = true; // Chão com jogador
                            player = new Player(this, x, y);
                            break;
                        default:
                            mapData[x, y] = true; // Por padrão, é chão
                            break;
                        case 'E': // Inimigo
                            mapData[x, y] = true; // Chão com inimigo
                            enemies.Add(new Enemy(this, x, y));
                            break;
                    }
                }
            }
        }
        private void UpdateCamera()
        {
            if (player == null) return;

            // Calcular posição desejada (centralizar no jogador)
            Vector2 targetPosition = new Vector2(
                player.Position.X * TileSize - (_graphics.PreferredBackBufferWidth / 2),
                player.Position.Y * TileSize - (_graphics.PreferredBackBufferHeight / 2)
            );

            // Limitar aos bordas do mapa (opcional)
            targetPosition.X = MathHelper.Clamp(targetPosition.X, 0, mapWidth * TileSize - _graphics.PreferredBackBufferWidth);
            targetPosition.Y = MathHelper.Clamp(targetPosition.Y, 0, mapHeight * TileSize - _graphics.PreferredBackBufferHeight);

            // Suavizar movimento
            cameraPosition = Vector2.Lerp(cameraPosition, targetPosition, cameraLerpSpeed);
        }
    }
}
