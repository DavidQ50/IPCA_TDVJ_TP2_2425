using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Zelda;

public class Game1 : Game
{
    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;

    public enum Direction { Up, Down, Left, Right }
    public Direction direction = Direction.Down;

    //public List<Point> boxes = new List<Point>(); // exemplo de caixas
    //private Player player;

    //Texture2D playerTexture;
    //Texture2D boxTexture;
    //Texture2D floorTexture;

    //const int tileSize = 32;
    //const int mapWidth = 20;
    //const int mapHeight = 15;

    //bool[,] mapData = new bool[mapWidth, mapHeight]; // true = walkable

    public List<Point> boxes = new List<Point>();

    private Player player;

    Texture2D playerTexture, boxTexture, floorTexture, wallTexture;

    bool[,] mapData;
    int mapWidth;
    int mapHeight;

    const int tileSize = 32;

    public const int screenWidthInTiles = 16;
    public const int screenHeightInTiles = 12;

    
    public const int worldWidthInScreens = 3;
    public const int worldHeightInScreens = 3;

    public Point currentScreen = new Point(1, 1); // começa no centro (1,1)

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        LoadMap("Content/map.txt");
        base.Initialize();

        //player = new Player(this, 2, 2);

        //// Exemplo de terreno walkable
        //for (int x = 0; x < mapWidth; x++)
        //    for (int y = 0; y < mapHeight; y++)
        //        mapData[x, y] = true;

        //// Exemplo de obstáculos
        //mapData[5, 5] = false;

        //// Exemplo de caixas
        //boxes.Add(new Point(3, 3));

        //base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        playerTexture = new Texture2D(GraphicsDevice, tileSize, tileSize);
        Color[] data = new Color[tileSize * tileSize];
        for (int i = 0; i < data.Length; i++) data[i] = Color.Blue;
        playerTexture.SetData(data);

        boxTexture = new Texture2D(GraphicsDevice, tileSize, tileSize);
        for (int i = 0; i < data.Length; i++) data[i] = Color.SaddleBrown;
        boxTexture.SetData(data);

        floorTexture = new Texture2D(GraphicsDevice, tileSize, tileSize);
        for (int i = 0; i < data.Length; i++) data[i] = Color.LightGray;
        floorTexture.SetData(data);

        wallTexture = new Texture2D(GraphicsDevice, tileSize, tileSize);
        Color[] wallData = new Color[tileSize * tileSize];
        for (int i = 0; i < wallData.Length; i++) wallData[i] = Color.DarkSlateGray;
        wallTexture.SetData(wallData);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        player.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Texture2D tex = mapData[x, y] ? floorTexture : wallTexture;
                _spriteBatch.Draw(tex, new Vector2(x * tileSize, y * tileSize), Color.White);
            }
        }

        foreach (var box in boxes)
        {
            _spriteBatch.Draw(boxTexture, new Vector2(box.X * tileSize, box.Y * tileSize), Color.White);
        }

        _spriteBatch.Draw(playerTexture, new Vector2(player.Position.X * tileSize, player.Position.Y * tileSize), Color.White);

        int screenOffsetX = currentScreen.X * screenWidthInTiles;
        int screenOffsetY = currentScreen.Y * screenHeightInTiles;

        for (int x = 0; x < screenWidthInTiles; x++)
        {
            for (int y = 0; y < screenHeightInTiles; y++)
            {
                int worldX = x + screenOffsetX;
                int worldY = y + screenOffsetY;

                Texture2D tex = mapData[worldX, worldY] ? floorTexture : wallTexture;
                _spriteBatch.Draw(tex, new Vector2(x * tileSize, y * tileSize), Color.White);
            }
        }

        foreach (var box in boxes)
        {
            if (box.X >= screenOffsetX && box.X < screenOffsetX + screenWidthInTiles &&
                box.Y >= screenOffsetY && box.Y < screenOffsetY + screenHeightInTiles)
            {
                Vector2 drawPos = new Vector2((box.X - screenOffsetX) * tileSize, (box.Y - screenOffsetY) * tileSize);
                _spriteBatch.Draw(boxTexture, drawPos, Color.White);
            }
        }

        Vector2 playerDrawPos = new Vector2((player.Position.X - screenOffsetX) * tileSize,
                                             (player.Position.Y - screenOffsetY) * tileSize);
        _spriteBatch.Draw(playerTexture, playerDrawPos, Color.White);

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    // Método chamado pelo Player para saber se há uma caixa
    public bool HasBox(int x, int y)
    {
        foreach (var b in boxes)
        {
            if (b.X == x && b.Y == y)
                return true;
        }
        return false;
    }

    // Método chamado pelo Player para saber se pode andar naquela tile
    public bool FreeTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight)
            return false;

        if (!mapData[x, y])
            return false;

        foreach (var b in boxes)
        {
            if (b.X == x && b.Y == y)
                return false;
        }

        return true;
    }

    private void LoadMap(string filePath)
    {
        string[] lines = System.IO.File.ReadAllLines(filePath);

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
                        mapData[x, y] = false;
                        break;
                    case ' ':
                        mapData[x, y] = true;
                        break;
                    case '#':
                        mapData[x, y] = true;
                        boxes.Add(new Point(x, y));
                        break;
                    case '.':
                        mapData[x, y] = true;
                        // Podes guardar zonas especiais se quiseres
                        break;
                    case 'Y':
                        mapData[x, y] = true;
                        player = new Player(this, x, y);
                        break;
                    default:
                        mapData[x, y] = true;
                        break;
                }
            }
        }

        // fallback se 'Y' não for encontrado
        if (player == null)
            player = new Player(this, 2, 2);

        mapData = new bool[mapWidth, mapHeight]; // mapWidth = 48, mapHeight = 36
    }
}

