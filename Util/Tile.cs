using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Util
{
    public class Tile
    {
        public static int tilesize = 32;
        private Vector2 tilePos;
        public Vector2 TilePos { get { return tilePos; } }
        private int id = 0;
        public int ID
        { get { return id; } }
        public Pathfinder.TileType Type = 0;

        public Rectangle recTile
        {
            get { return new Rectangle((int)tilePos.X * tilesize, (int)tilePos.Y * tilesize, tilesize, tilesize); }
        }
        public Tile(Vector2 tilePos, GraphicsDevice gDevice, int newId)
        {
            this.tilePos = tilePos;
            this.id = newId;
        }

        public static Point PosToPoint(Vector2 Pos)
        {
            return new Point((int)Math.Floor(Pos.X / tilesize), (int)Math.Floor(Pos.Y / tilesize));
        }
    }
}
