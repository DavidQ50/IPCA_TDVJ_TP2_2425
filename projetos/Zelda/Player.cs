using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Zelda.Game1;

namespace Zelda
{
    class Player
    { // Current player position in the matrix (multiply by tileSize prior to drawing)

        private Point position; //Point = Vector2, mas são inteiros
        public Point Position => position; //auto função (equivalente a ter só get sem put) - AUTOPROPERTY
                                           //public Vector2 Position
                                           //{
                                           // get{return position;}
                                           //}
        private Game1 game; //reference from Game1 to Player
        private bool keysReleased = true;


        //private Texture2D[][] sprites;
        private Direction direction = Direction.Down;
        private Vector2 directionVector;
        private int speed = 2; //Nota: tem de ser divisor de tileSize
        private int delta = 0;

        public Player(Game1 game1, int x, int y) //constructor que dada a as posições guarda a sua posição
        {
            position = new Point(x, y);
            game = game1;
        }

        public void LoadContents()
        {
            //player = new Texture2D[4];
            //player[(int)Direction.Down] = Content.Load<Texture2D>("Character4");
            //player[(int)Direction.Up] = Content.Load<Texture2D>("Character7");
            //player[(int)Direction.Left] = Content.Load<Texture2D>("Character1");
            //player[(int)Direction.Right] = Content.Load<Texture2D>("Character2");
        }

        public void Update(GameTime gameTime)
        {
            //point lastposition = position;
            //keyboardstate kstate = keyboard.getstate();
            //if (keysreleased)
            //{
            //    keysreleased = false;
            //    if ((kstate.iskeydown(keys.a)) || (kstate.iskeydown(keys.left)))
            //    {
            //        position.x--;
            //        game.direction = direction.left;
            //    }
            //    else if ((kstate.iskeydown(keys.w)) || (kstate.iskeydown(keys.up)))
            //    {
            //        position.y--;
            //        game.direction = direction.up;
            //    }
            //    else if ((kstate.iskeydown(keys.s)) || (kstate.iskeydown(keys.down)))
            //    {
            //        position.y++;
            //        game.direction = direction.down;
            //    }
            //    else if ((kstate.iskeydown(keys.d)) || (kstate.iskeydown(keys.right)))
            //    {
            //        position.x++;
            //        game.direction = direction.right;
            //    }
            //    else keysreleased = true;
            //}
            //else
            //{
            //    if (kstate.iskeyup(keys.a) && kstate.iskeyup(keys.w) &&
            //    kstate.iskeyup(keys.s) && kstate.iskeyup(keys.d))
            //    {
            //        keysreleased = true;
            //    }
            //}

            //// destino é caixa?
            //if (game.hasbox(position.x, position.y))
            //{
            //    int deltax = position.x - lastposition.x;
            //    int deltay = position.y - lastposition.y;
            //    point boxtarget = new point(deltax + position.x, deltay + position.y);
            //    // se sim, caixa pode mover-se?
            //    if (game.freetile(boxtarget.x, boxtarget.y))
            //    {
            //        for (int i = 0; i < game.boxes.count; i++)
            //        {
            //            if (game.boxes[i].x == position.x && game.boxes[i].y == position.y)
            //            {
            //                game.boxes[i] = boxtarget;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        position = lastposition;
            //    }
            //}
            //else
            //{
            //    // se não é caixa, se não está livre, parado!
            //    if (!game.freetile(position.x, position.y))
            //        position = lastposition;
            //}

            int maxX = game.currentScreen.X * screenWidthInTiles + screenWidthInTiles - 1;
            int minX = game.currentScreen.X * screenWidthInTiles;
            int maxY = game.currentScreen.Y * screenHeightInTiles + screenHeightInTiles - 1;
            int minY = game.currentScreen.Y * screenHeightInTiles;

            if (position.X > maxX && game.currentScreen.X < worldWidthInScreens - 1)
            {
                game.currentScreen.X++;
                position.X = minX; // reaparece do outro lado
            }
            else if (position.X < minX && game.currentScreen.X > 0)
            {
                game.currentScreen.X--;
                position.X = maxX;
            }
            else if (position.Y > maxY && game.currentScreen.Y < worldHeightInScreens - 1)
            {
                game.currentScreen.Y++;
                position.Y = minY;
            }
            else if (position.Y < minY && game.currentScreen.Y > 0)
            {
                game.currentScreen.Y--;
                position.Y = maxY;
            }
        }

    }

}
    

