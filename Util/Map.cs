using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonInvaders.Util.Entities;

namespace DungeonInvaders.Util
{
    public class Map
    {
        #region Types

        public enum Size : byte
        {
            Tiny,
            Small,
            Medium,
            Large,
            Huge,
            Massive
        }

        enum Direction : byte
        {
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3
        }

        #endregion

        #region Fields

        public Room[,] Rooms;
        int[,] OpenRooms;
        bool[, ,] PassageOpen;

        List<Point> BuildOrder = new List<Point>();

        int Difficulty = 0;
        int Width = 5;
        int Height = 5;

        int RoomCount = 0;
        int BossRoomID = -1;

        public PlayerEntity Player;

        public static List<PathFindingParams> Paths = new List<PathFindingParams>();

        public static int StartEXP = 0;

        #endregion

        #region Properties

        public static Map Current { get; private set; }

        #endregion

        #region Methods

        public Map(int difficulty, int area, Size size)
        {
            Current = this;
            LastPosEntity = null;

            Player = new PlayerEntity();

            StartEXP = User.EXP;

            Difficulty = difficulty;

            Room.ClearRenderTargets();

            if (size == Size.Tiny)
            {
                Width = 3;
                Height = 3;
            }
            else if (size == Size.Small)
            {
                Width = 6;
                Height = 6;
            }
            else if (size == Size.Medium)
            {
                Width = 9;
                Height = 9;
            }
            else if (size == Size.Large)
            {
                Width = 12;
                Height = 12;
            }
            else if (size == Size.Huge)
            {
                Width = 15;
                Height = 15;
            }
            else
            {
                Width = 18;
                Height = 18;
            }

            OpenRooms = new int[Width + 1, Height + 1];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    OpenRooms[i, j] = -1;
                }
            }

            Rooms = new Room[Width + 1, Height + 1];
            PassageOpen = new bool[Width + 1, Height + 1, 4];

            Random r = new Random();

            int x = Width / 2;
            int y = Height / 2;

            OpenRooms[x, y] = 0;

            DoRoom(x, y, (Direction) 255, r);

            if (BossRoomID == -1)
            {
                BossRoomID = BuildOrder.Count - 1;
                OpenRooms[BuildOrder[BossRoomID].X, BuildOrder[BossRoomID].Y] = r.Next(Room.BossCount(area));
            }

            
            Console.WriteLine("Generated {0} rooms", RoomCount);

            foreach(Point p in BuildOrder)
            {
                bool boss = p.X == BuildOrder[BossRoomID].X && p.Y == BuildOrder[BossRoomID].Y;

                Rooms[p.X, p.Y] = new Room(Difficulty, OpenRooms[p.X, p.Y], area, boss, new Vector2(p.X, p.Y), r);

                Rooms[p.X, p.Y].LeftConnection = PassageOpen[p.X, p.Y, 0];
                Rooms[p.X, p.Y].RightConnection = PassageOpen[p.X, p.Y, 1];
                Rooms[p.X, p.Y].TopConnection = PassageOpen[p.X, p.Y, 2];
                Rooms[p.X, p.Y].BottomConnection = PassageOpen[p.X, p.Y, 3];

            }

            int k = 0;
            for (k = 0; k < (int)Math.Floor(RoomCount / 10f); k++)
            {
                int rmID = -1;

                while (rmID < 0 || rmID == BossRoomID)
                {
                    rmID = r.Next(1, BuildOrder.Count - 1);
                }

                AddLockedDoorAndKey(rmID, k, r);
            }

            AddLockedDoorAndKey(BossRoomID, k + 1, r);

            Room start = Rooms[BuildOrder[0].X, BuildOrder[0].Y];

            Player.Position = Rooms[BuildOrder[0].X, BuildOrder[0].Y].Position *
                new Vector2(Rooms[BuildOrder[0].X, BuildOrder[0].Y].Width * Rooms[BuildOrder[0].X, BuildOrder[0].Y].TileWidth,
                    Rooms[BuildOrder[0].X, BuildOrder[0].Y].Height * Rooms[BuildOrder[0].X, BuildOrder[0].Y].TileHeight);

            Player.Position += new Vector2(32 * 12);

            Player.PreviousPosition = Player.Position;
        }

        bool DoRoom(int x, int y, Direction prevDir, Random r)
        {
            if (OpenRooms[x, y] == -1)
            {
                OpenRooms[x, y] = r.Next(1, Room.MapCount(States.DungeonSettingsState.Area));
            }
            else if (RoomCount > 0)
            {
                return false;
            }

            BuildOrder.Add(new Point(x, y));

            RoomCount++;

            int roomMinCount = (int)((Width + Height) / 1.5f);

            if ((int)prevDir < 4)
            {
                PassageOpen[x, y, (int)prevDir] = true;
            }

            bool doRight = r.Next(100) < 53;
            bool doDown = r.Next(100) < 53;
            bool doLeft = r.Next(100) < 53;
            bool doUp = r.Next(100) < 53;

            if (prevDir == Direction.Left)
            {
                doLeft = false;
            }
            else if (prevDir == Direction.Right)
            {
                doRight = false;
            }
            else if (prevDir == Direction.Bottom)
            {
                doDown = false;
            }
            else if (prevDir == Direction.Top)
            {
                doUp = false;
            }

            doLeft = x == 0 || OpenRooms[x - 1, y] > -1 ? false : doLeft;
            doRight = x == Width || OpenRooms[x + 1, y] > -1 ? false : doRight;
            doUp = y == 0 || OpenRooms[x, y - 1] > -1 ? false : doUp;
            doDown = y == Height || OpenRooms[x, y + 1] > -1 ? false : doDown;

            if (!doLeft && !doRight && !doUp && !doDown && RoomCount < roomMinCount)
            {
                int dir = r.Next(3) + (int)(prevDir + 1);
                dir %= 4;

                if (x == 0 && dir == 0)
                {
                    dir = 1;
                    if (prevDir == Direction.Right)
                    {
                        dir = 2;
                    }
                }
                else if (x == Width && dir == 1)
                {
                    dir = 0;
                    if (prevDir == Direction.Left)
                    {
                        dir = 3;
                    }
                }
                else if (y == 0 && dir == 2)
                {
                    dir = 3;
                    if (prevDir == Direction.Bottom)
                    {
                        dir = 0;
                    }
                }
                else if (y == Height && dir == 3)
                {
                    dir = 2;
                    if (prevDir == Direction.Top)
                    {
                        dir = 1;
                    }
                }


                if (dir == 0)
                {
                    doRight = prevDir != Direction.Right;
                    doUp = prevDir != Direction.Top;
                    doDown = prevDir != Direction.Bottom;
                }
                else if (dir == 1)
                {
                    doLeft = prevDir != Direction.Left;
                    doUp = prevDir != Direction.Top;
                    doDown = prevDir != Direction.Bottom;
                }
                else if (dir == 2)
                {
                    doLeft = prevDir != Direction.Left;
                    doRight = prevDir != Direction.Right;
                    doDown = prevDir != Direction.Bottom;
                }
                else if (dir == 3)
                {
                    doLeft = prevDir != Direction.Left;
                    doRight = prevDir != Direction.Right;
                    doUp = prevDir != Direction.Top;
                }
            }

            if (doLeft)
            {
                if (x == 0) { doLeft = false; }
                else if (OpenRooms[x - 1, y] > -1) { doLeft = false; }
            }

            if (doRight)
            {
                if (x == Width) { doRight = false; }
                else if (OpenRooms[x + 1, y] > -1) { doRight = false; }
            }

            if (doUp)
            {
                if (y == 0) { doUp = false; }
                else if (OpenRooms[x, y - 1] > -1) { doUp = false; }
            }

            if (doDown)
            {
                if (y == Height) { doDown = false; }
                else if (OpenRooms[x, y + 1] > -1) { doDown = false; }
            }

            if (doLeft)
            {
                PassageOpen[x, y, (int)Direction.Left] = DoRoom(x - 1, y, Direction.Right, r);
            }

            if (doRight)
            {
                PassageOpen[x, y, (int)Direction.Right] =  DoRoom(x + 1, y, Direction.Left, r);
            }

            if (doUp)
            {
                PassageOpen[x, y, (int)Direction.Top] = DoRoom(x, y - 1, Direction.Bottom, r);
            }

            if (doDown)
            {
                PassageOpen[x, y, (int)Direction.Bottom] = DoRoom(x, y + 1, Direction.Top, r);
            }

            return true;
        }

        public void Update()
        {
            FurnaceEntity.Distance = int.MaxValue;

            Rectangle CameraRect = new Rectangle((int)Camera.Position.X - 32, (int)Camera.Position.Y - 32,
                Main.Graphics.PreferredBackBufferWidth + 32, Main.Graphics.PreferredBackBufferHeight + 32);

            Player.Update();

            if (Player.CanDispose)
            {
                if (States.StateManager.CurrentState is States.GameState)
                {
                    States.GameState g = (States.GameState)States.StateManager.CurrentState;
                    g.GameOver = true;
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Room room = Rooms[x, y];
                    if (room == null) { continue; }

                    Rectangle rect = new Rectangle();
                    rect.X = ((int)room.Position.X) * (room.Width * room.TileWidth);
                    rect.Y = ((int)room.Position.Y) * (room.Height * room.TileHeight);
                    rect.Width = room.Width * room.TileWidth;
                    rect.Height = room.Height * room.TileHeight;

                    if (!CameraRect.Intersects(rect)) { continue; }

                    room.Update(Player);
                }
            }
        }

        public void Draw()
        {
            Rectangle CameraRect = new Rectangle((int)Camera.Position.X - 32, (int)Camera.Position.Y - 32,
                Main.Graphics.PreferredBackBufferWidth + 32, Main.Graphics.PreferredBackBufferHeight + 32);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Room room = Rooms[x, y];
                    if (room == null) { continue; }

                    Rectangle rect = new Rectangle();
                    rect.X = ((int)room.Position.X) * (room.Width * room.TileWidth);
                    rect.Y = ((int)room.Position.Y) * (room.Height * room.TileHeight);
                    rect.Width = room.Width * room.TileWidth;
                    rect.Height = room.Height * room.TileHeight;

                    if (!CameraRect.Intersects(rect)) { continue; }

                    room.Draw(Color.White);
                }
            }

            Player.Draw();
        }

        public Room GetRoomFromPosition(Vector2 pos)
        {
            int x, y;

            x = (int)Math.Floor(pos.X / (Rooms[BuildOrder[0].X, BuildOrder[0].Y].Width *
                Rooms[BuildOrder[0].X, BuildOrder[0].Y].TileWidth));

            y = (int)Math.Floor(pos.Y / (Rooms[BuildOrder[0].X, BuildOrder[0].Y].Height *
                Rooms[BuildOrder[0].X, BuildOrder[0].Y].TileHeight));


            x = x < 0 ? 0 : x > Width ? Width : x;
            y = y < 0 ? 0 : y > Height ? Height : y;

            return Rooms[x, y];
        }

        static Entity LastPosEntity = null;

        public bool CanMoveToPosition(Vector2 pos, Vector2 speed, bool monster, out Rectangle rect)
        {
            rect = new Rectangle();

            Room r = GetRoomFromPosition(pos);

            if (r == null) { return false; }

            Vector2 local = pos - (r.Position * new Vector2(r.Width * r.TileWidth, r.Height * r.TileHeight));

            int x = (int)Math.Floor(local.X / r.TileWidth);
            int y = (int)Math.Floor(local.Y / r.TileHeight);

            if (x < 0 || y < 0) { return false; }

            int gID = 0;

            int layer = r.TileLayers.Count;

            // Get the top most tile at the position
            while (gID == 0 && layer > 0)
            {
                 gID = r.TileLayers[layer -= 1].Data[x + (y * r.Width)];
            }

            if (gID == 0)
            {
                return true;
            }

            int ts = 0;

            for (int i = 0; i < r.Tilesets.Count; i++)
            {
                if (r.Tilesets[i].StartGID <= gID)
                {
                    ts = i;
                }
                else
                {
                    break;
                }
            }

            if (r.Tilesets[ts].Physical)
            {
                foreach (Entity e in r.Entities)
                {
                    if (monster) { break; }

                    if (!(e is TileEntity))
                    {
                        continue;
                    }

                    TileEntity t = (TileEntity)e;

                    if (e.Bounds.Contains(pos))
                    {
                        if (LastPosEntity != null)
                        {
                            t.OnCollide(LastPosEntity);
                            LastPosEntity.OnCollide(t);
                        }
                    }

                    if (t.Data == 0 && !r.TopConnection) { continue; }
                    else if (t.Data == 1 && !r.BottomConnection) { continue; }
                    else if (t.Data == 2 && !r.LeftConnection) { continue; }
                    else if (t.Data == 3 && !r.RightConnection) { continue; }
                    else if (t.Data == 4 && (!r.TopConnection || speed.Y < 0)) { continue; }
                    else if (t.Data == 5 && (!r.BottomConnection || speed.Y > 0)) { continue; }
                    else if (t.Data == 6 && (!r.LeftConnection || speed.X < 0)) { continue; }
                    else if (t.Data == 7 && (!r.RightConnection || speed.X > 0)) { continue; }
                    else if (t.Data > 7) { continue; }

                    if (e.Bounds.Contains(pos))
                    {
                        return true;
                    }
                }

                rect = new Rectangle((x * r.TileWidth) + (r.Width * r.TileWidth * (int)r.Position.X),
                    (y * r.TileHeight) + (r.Height * r.TileHeight * (int)r.Position.Y), r.TileWidth, r.TileHeight);

                return false;
            }

            foreach (Entity e in r.Entities)
            {
                if (monster) { break; }

                if (!(e is TileEntity))
                {
                    if (e.IsPhysical && e.Bounds.Contains(pos))
                    {
                        rect = e.Bounds;
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanMoveToPosition(Entity entity, out Rectangle outBounds)
        {
            LastPosEntity = entity;

            bool monster = entity is MonsterEntity;

            outBounds = new Rectangle();

            Room r = GetRoomFromPosition(entity.Position);

            if (r == null) { return false; }

            bool ul = false, ur = false, ll = false, lr = false;

            foreach (Entities.Entity e in r.Entities)
            {
                Rectangle bounds = e.Bounds;

                if (!(e is TileEntity))
                {
                    continue;
                }

                TileEntity t = (TileEntity)e;

                if (t.Data == 0 && !r.TopConnection) { continue; }
                else if (t.Data == 1 && !r.BottomConnection) { continue; }
                else if (t.Data == 2 && !r.LeftConnection) { continue; }
                else if (t.Data == 3 && !r.RightConnection) { continue; }
                else if (t.Data == 4 && (!r.TopConnection || entity.Speed.Y < 0)) { continue; }
                else if (t.Data == 5 && (!r.BottomConnection || entity.Speed.Y > 0)) { continue; }
                else if (t.Data == 6 && (!r.LeftConnection || entity.Speed.X < 0)) { continue; }
                else if (t.Data == 7 && (!r.RightConnection || entity.Speed.X > 0)) { continue; }
                else if (t.Data > 7) { continue; }

                if (bounds.Contains(new Vector2(entity.Bounds.X - 1, entity.Bounds.Y - 1))) { ul = true; }
                if (bounds.Contains(new Vector2(entity.Bounds.X + 1 + entity.Bounds.Width, entity.Bounds.Y - 1))) { ur = true; }
                if (bounds.Contains(new Vector2(entity.Bounds.X - 1, entity.Bounds.Y + entity.Bounds.Height + 1))) { ll = true; }
                if (bounds.Contains(new Vector2(entity.Bounds.X + 1 + entity.Bounds.Width, entity.Bounds.Y + entity.Bounds.Height + 1))) { lr = true; }
            }

            Rectangle a, b, c, d;

            bool cA = (!CanMoveToPosition(new Vector2(entity.Bounds.X, entity.Bounds.Y), entity.Speed, monster, out a) && !ul);
            bool cB = (!CanMoveToPosition(new Vector2(entity.Bounds.X + entity.Bounds.Width, entity.Bounds.Y), entity.Speed, monster, out b) && !ur);
            bool cC = (!CanMoveToPosition(new Vector2(entity.Bounds.X, entity.Bounds.Y + entity.Bounds.Height), entity.Speed, monster, out c) && !ll);
            bool cD = (!CanMoveToPosition(new Vector2(entity.Bounds.X + entity.Bounds.Width, entity.Bounds.Y + entity.Bounds.Height), entity.Speed, monster, out d) && !lr);

            if (cA)
            {
                outBounds = a;
            }
            else if (cB)
            {
                outBounds = b;
            }
            else if (cC)
            {
                outBounds = c;
            }
            else if (cD)
            {
                outBounds = d;
            }

            if (cA || cB || cC || cD)
            {
                return false;
            }

            return true;
        }

        public void AddLockedDoorAndKey(int roomID, int keyID, Random r)
        {
            int keyRoom = r.Next(roomID - 1);

            while (keyRoom == BossRoomID)
            {
                keyRoom = r.Next(roomID - 1);
            }

            Room A = Rooms[BuildOrder[roomID].X, BuildOrder[roomID].Y];
            Room B = null;

            for (int i = 0; i < roomID; i++)
            {
                B = Rooms[BuildOrder[i].X, BuildOrder[i].Y];

                if (A.Position.X - B.Position.X == 1 && A.LeftConnection && B.RightConnection)
                {
                    foreach(Entity e in A.Entities)
                    {
                        if (e is TileEntity)
                        {
                            TileEntity t = (TileEntity)e;

                            if (t.Data == 2)
                            {
                                t.Data = 10;
                                t.Texture = Main.ContentManager.Load<Texture2D>("DungeonLocks");
                                t.SetGID(0, A.TileWidth, A.TileHeight);
                                t.Value = (byte)keyID;
                                t.DrawColor = KeyEntity.KeyColors[keyID];
                            }
                        }
                    }
                }
                else if (A.Position.X - B.Position.X == -1 && A.RightConnection && B.LeftConnection)
                {
                    foreach (Entity e in A.Entities)
                    {
                        if (e is TileEntity)
                        {
                            TileEntity t = (TileEntity)e;

                            if (t.Data == 3)
                            {
                                t.Data = 11;
                                t.Texture = Main.ContentManager.Load<Texture2D>("DungeonLocks");
                                t.SetGID(0, A.TileWidth, A.TileHeight);
                                t.Value = (byte)keyID;
                                t.DrawColor = KeyEntity.KeyColors[keyID];
                            }
                        }
                    }
                }
                else if (A.Position.Y - B.Position.Y == 1 && A.TopConnection && B.BottomConnection)
                {
                    foreach (Entity e in A.Entities)
                    {
                        if (e is TileEntity)
                        {
                            TileEntity t = (TileEntity)e;

                            if (t.Data == 0)
                            {
                                t.Data = 8;
                                t.Texture = Main.ContentManager.Load<Texture2D>("DungeonLocks");
                                t.SetGID(0, A.TileWidth, A.TileHeight);
                                t.Value = (byte)keyID;
                                t.DrawColor = KeyEntity.KeyColors[keyID];
                            }
                        }
                    }
                }
                else if (A.Position.Y - B.Position.Y == -1 && A.BottomConnection && B.TopConnection)
                {
                    foreach (Entity e in A.Entities)
                    {
                        if (e is TileEntity)
                        {
                            TileEntity t = (TileEntity)e;

                            if (t.Data == 1)
                            {
                                t.Data = 9;
                                t.Texture = Main.ContentManager.Load<Texture2D>("DungeonLocks");
                                t.SetGID(0, A.TileWidth, A.TileHeight);
                                t.Value = (byte)keyID;
                                t.DrawColor = KeyEntity.KeyColors[keyID];
                            }
                        }
                    }
                }
            }

            A = Rooms[BuildOrder[keyRoom].X, BuildOrder[keyRoom].Y];

            if (A == null) { throw new Exception("Cannot add key to null room"); }

            Vector2 MinPos = A.Position * new Vector2(A.Width * A.TileWidth, A.Height * A.TileHeight);
            Vector2 MaxPos = MinPos + new Vector2(A.Width * A.TileWidth, A.Height * A.TileHeight);

            Vector2 pos = new Vector2(-1, -1);

            Rectangle o;

            while (!CanMoveToPosition(pos, Vector2.Zero, true, out o))
            {
                pos = new Vector2(r.Next((int)MinPos.X, (int)MaxPos.X), r.Next((int)MinPos.Y, (int)MaxPos.Y));
            }

            KeyEntity key = new KeyEntity(keyID);
            key.Position = pos;

            A.Entities.Add(key);
        }

        public void AddOreVein(int roomID, int oreType, Random r)
        {

        }

        #endregion
    }
}
