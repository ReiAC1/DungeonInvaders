using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonInvaders.Util.Entities;
using DungeonInvaders.Plugins;

namespace DungeonInvaders.Util
{
    public class Room
    {
        #region Types

        public struct Tile
        {
            public Tileset Parent;
            public int ID;

            public void Draw(int x, int y, int width, int height)
            {
                Rectangle rect = new Rectangle(0, 0, width, height);

                int xAmount = (int)Math.Ceiling((float)Parent.Texture.Width / width);

                rect.X = (ID - Parent.StartGID) % xAmount;
                rect.Y = (ID - Parent.StartGID) / xAmount;

                rect.X *= width;
                rect.Y *= height;

                Main.SpriteBatch.Draw(Parent.Texture, new Vector2(x, y), rect, Color.White);
            }
        }

        public struct Tileset
        {
            public int StartGID;
            public Texture2D Texture;

            public bool Physical;

            public Tile[] Tiles;
        }

        public struct TileLayer
        {
            public string Name;
            public int[] Data;
        }

        #endregion

        #region Fields

        static Dictionary<Point, RenderTarget2D> BakedTextures = new Dictionary<Point, RenderTarget2D>();

        public const int StartMapCount = 6;
        public const int StartBossCount = 1;

        public List<Entity> Entities = new List<Entity>();
        public RenderTarget2D Map;

        public Vector2 Position = Vector2.Zero;

        public int Width, Height, TileWidth, TileHeight;

        public List<Tileset> Tilesets = new List<Tileset>();
        public List<TileLayer> TileLayers = new List<TileLayer>();

        public bool Entered = false;

        public bool LeftConnection = false,
            RightConnection = false,
            TopConnection = false,
            BottomConnection = false;

        public static Dictionary<Point, Pathfinder> Pathfinders = new Dictionary<Point, Pathfinder>();

        bool AlreadyCleared = false, Boss = false;

        int Difficulty = 0, MapID = 0;

        #endregion

        #region Properties

        public bool IsCleared
        {
            get
            {
                foreach(Entity e in Entities)
                {
                    if (e is MonsterEntity)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public Pathfinder Pathfinder { get; private set; }

        #endregion

        #region Methods


        public static void ClearRenderTargets()
        {
            foreach(KeyValuePair<Point, RenderTarget2D> rt in BakedTextures)
            {
                rt.Value.Dispose();
            }

            BakedTextures.Clear();
        }

        public static int MapCount(int area)
        {
            return StartMapCount + PluginManager.Rooms[area].Count;
        }

        public static int BossCount(int area)
        {
            return StartBossCount + PluginManager.BossRooms[area].Count;
        }

        public Room(int difficulty, int id, int area, bool boss, Vector2 position, Random r)
        {
            Position = position;
            Difficulty = difficulty;
            MapID = id;
            Boss = boss;

            string mapID = (boss ? "boss" : "room") + "[" + area + " " + id + "]";
            string pluginName = "";
            bool deleteDirectory = false;

            if (id >= StartMapCount && !boss)
            {
                mapID = PluginManager.Rooms[area][id - StartMapCount].Path + "[" + area + "," + (id - StartMapCount) + "]";
                pluginName = PluginManager.Plugins[PluginManager.Rooms[area][id - StartMapCount].PluginID].Name;
            }
            else if (id >= StartBossCount && boss)
            {
                mapID = PluginManager.BossRooms[area][id - StartBossCount].Path + "[" + area + "," + (id - StartBossCount) + "]";
                 pluginName = PluginManager.Plugins[PluginManager.BossRooms[area][id - StartBossCount].PluginID].Name;
            }

            XmlReader reader;
            Stream baseStream;

            if ((id < StartMapCount && !boss) || (id < StartBossCount && boss))
            {
                baseStream = this.GetType().Assembly.GetManifestResourceStream("DungeonInvaders.Rooms." + mapID + ".tmx");
            }
            else
            {
                if (!File.Exists("Plugins/" + pluginName + "//" + mapID + ".tmx"))
                {
                    Plugins.PluginManager.CurrentPlugin
                        [mapID + ".tmx"].Extract("Plugins/" + pluginName);
                    baseStream = File.OpenRead("Plugins/" + pluginName + "//" + mapID + ".tmx");
                    deleteDirectory = true;
                }
                else
                {
                    baseStream = File.OpenRead("Plugins/" + pluginName + "//" + mapID + ".tmx");
                }
            }

            reader = XmlReader.Create(baseStream);

            reader.MoveToContent();

            if (reader.NodeType == XmlNodeType.None)
            {
                throw new XmlException("Cannot parse room: " + mapID + " (Invalid Format)");
            }

            reader.MoveToAttribute(4);
            reader.ReadAttributeValue();

            Width = reader.ReadContentAsInt();

            reader.MoveToNextAttribute();
            reader.ReadAttributeValue();

            Height = reader.ReadContentAsInt();

            reader.MoveToNextAttribute();
            reader.ReadAttributeValue();

            TileWidth = reader.ReadContentAsInt();

            reader.MoveToNextAttribute();
            reader.ReadAttributeValue();

            TileHeight = reader.ReadContentAsInt();

            reader.MoveToElement();
            reader.ReadStartElement();

            while (!reader.EOF)
            {
                reader.ReadInnerXml();

                if (reader.Name == "tileset")
                {
                    ReadTileset(reader, mapID);
                }
                else if (reader.Name == "layer")
                {
                    ReadTileLayer(reader, mapID);
                }
                else if (reader.Name == "objectgroup")
                {
                    ReadObjectGroup(reader, mapID, r);
                }
            }

            reader.Close();
            baseStream.Close();

            id += boss ? MapCount(area) + 1 : 0;

            if (BakedTextures.ContainsKey(new Point(area, id)))
            {
                Pathfinder = Pathfinders[new Point(area, id)];
                Map = BakedTextures[new Point(area, id)];
                return;
            }

            Map = new RenderTarget2D(Main.Graphics.GraphicsDevice, Width * TileWidth, Height * TileHeight);

            Main.Graphics.GraphicsDevice.SetRenderTarget(Map);

            Main.SpriteBatch.Begin();

            for (int z = 0; z < TileLayers.Count; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        int tid = TileLayers[z].Data[x + (y * Width)];

                        int setID = 0;

                        for (int i = 1; i < Tilesets.Count; i++)
                        {
                            if (tid >= Tilesets[i].StartGID)
                            {
                                setID = i;
                            }
                            else
                            {
                                break;
                            }
                        }

                        Tilesets[setID].Tiles[tid - Tilesets[setID].StartGID].Draw(x * TileWidth, y * TileHeight,
                            TileWidth, TileHeight);
                    }
                }
            }

            Main.SpriteBatch.End();

            Main.Graphics.GraphicsDevice.SetRenderTarget(null);

            BakedTextures[new Point(area, id)] = Map;

            List<Util.Tile> PathTiles = new List<Util.Tile>();

            int xx = 0;
            int yy = 0;

            for (int tid = 0; tid < TileLayers[0].Data.Length; tid++)
            {
                int gID = TileLayers[0].Data[xx + (yy * Width)];

                int ts = 0;

                for (int i = 0; i < Tilesets.Count; i++)
                {
                    if (Tilesets[i].StartGID <= gID)
                    {
                        ts = i;
                    }
                    else
                    {
                        break;
                    }
                }

                PathTiles.Add(new Util.Tile(new Vector2(xx, yy), Main.Graphics.GraphicsDevice, 0));

                if (Tilesets[ts].Physical)
                {
                    PathTiles[PathTiles.Count - 1].Type = Util.Pathfinder.TileType.Wall;
                }
                else
                {
                    PathTiles[PathTiles.Count - 1].Type = Util.Pathfinder.TileType.Floor;
                }

                yy++;

                if (yy >= Height) { xx++; yy = 0; }
            }

            Pathfinders[new Point(area, id)] =
                new Pathfinder(PathTiles, new Vector2(Width, Height), false);

            Pathfinder = Pathfinders[new Point(area, id)];

            if (deleteDirectory)
            {
                System.IO.Directory.Delete("Plugins/ " + pluginName, true);
            }
        }

        #region TMX Loading

        void ReadTileset(XmlReader reader, string mapID)
        {
            Tileset ts = new Tileset();

            reader.MoveToFirstAttribute();
            reader.ReadAttributeValue();

            ts.StartGID = reader.ReadContentAsInt();

            reader.MoveToNextAttribute();
            reader.ReadAttributeValue();

            XmlReader tsxReader = XmlReader.Create(GetType().Assembly.GetManifestResourceStream("DungeonInvaders.Rooms." + reader.ReadContentAsString()));

            tsxReader.MoveToElement();
            tsxReader.ReadStartElement();

            tsxReader.ReadInnerXml();

            if (tsxReader.Name == "properties")
            {
                tsxReader.ReadString();

                tsxReader.MoveToAttribute(1);
                tsxReader.ReadAttributeValue();

                ts.Physical = tsxReader.ReadContentAsString() == "True";

                tsxReader.ReadToNextSibling("property");
                tsxReader.ReadEndElement();
                tsxReader.ReadString();
            }

            tsxReader.MoveToFirstAttribute();
            tsxReader.ReadAttributeValue();

            string name = tsxReader.ReadContentAsString();
            name = name.Replace(new System.IO.FileInfo(name).Extension, "");

            if (!File.Exists("Content/" + name + ".xnb"))
            {
                if (Boss)
                {
                    name = "../Plugins/" + PluginManager.Plugins[PluginManager.BossRooms[States.DungeonSettingsState.Area]
                        [MapID - StartBossCount].PluginID].Name + name;
                }
                else
                {
                    name = "../Plugins/" + PluginManager.Plugins[PluginManager.Rooms[States.DungeonSettingsState.Area]
                        [MapID - StartMapCount].PluginID].Name + name;
                }
            }

            ts.Texture = Main.ContentManager.Load<Texture2D>(name);

            tsxReader.Close();

            int xCount = ts.Texture.Width / TileWidth;
            int yCount = ts.Texture.Height / TileHeight;

            int total = xCount * yCount;

            ts.Tiles = new Tile[total];

            for (int i = 0; i < total; i++)
            {
                ts.Tiles[i] = new Tile();
                ts.Tiles[i].Parent = ts;
                ts.Tiles[i].ID = ts.StartGID + i;
            }

            Tilesets.Add(ts);
        }

        void ReadTileLayer(XmlReader reader, string mapID)
        {
            TileLayer layer = new TileLayer();

            reader.MoveToFirstAttribute();
            reader.ReadAttributeValue();

            layer.Name = reader.ReadContentAsString();

            reader.ReadString();

            reader.MoveToFirstAttribute();
            reader.ReadAttributeValue();

            string encoding = reader.ReadContentAsString();

            if (encoding != "base64" || reader.AttributeCount != 1)
            {
                throw new XmlException("Cannot parse room: " + mapID + " (Tile Encoding)");
            }
            string data = reader.ReadString();

            reader.ReadEndElement();
            reader.ReadEndElement();

            data = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(data));

            layer.Data = new int[data.Length / 4];

            for (int i = 0; i < Width * Height * 4; i += 4)
            {
                int id = data[i];
                id |= data[i + 1] << 8;
                id |= data[i + 2] << 16;
                id |= data[i + 3] << 24;

                layer.Data[i / 4] = id;
            }

            TileLayers.Add(layer);
        }

        void ReadObjectGroup(XmlReader reader, string mapID, Random r)
        {
            int depth = reader.Depth;

            reader.ReadString();

            List<bool> createdIDs = new List<bool>();

            while (reader.Depth != depth)
            {
                if (reader.AttributeCount == 7)
                {
                    reader.MoveToAttribute(1);
                    reader.ReadAttributeValue();

                    string type = reader.ReadContentAsString().ToLower();

                    reader.MoveToNextAttribute();
                    reader.ReadAttributeValue();

                    int gid = reader.ReadContentAsInt();

                    reader.MoveToNextAttribute();
                    reader.ReadAttributeValue();

                    int x = reader.ReadContentAsInt();

                    reader.MoveToNextAttribute();
                    reader.ReadAttributeValue();

                    int y = reader.ReadContentAsInt();

                    TileEntity e = null;

                    for (int i = 0; i < Tilesets.Count; i++)
                    {
                        if (gid >= Tilesets[i].StartGID)
                        {
                            e = new TileEntity(Tilesets[i].StartGID, gid, Tilesets[i].Texture, TileWidth, TileHeight);
                            e.Size = new Vector2(TileWidth, TileHeight);
                            e.Position = new Vector2(x + (TileWidth / 2), y - (TileHeight / 2));
                        }
                    }

                    e.Position += Position * new Vector2(Width * TileWidth, Height * TileHeight);
                    
                    e.Data = (byte)(type == "ng" ? 0 : (type == "sg" ? 1 : (type == "wg" ? 2 : 3)));

                    Entities.Add(e);
                }
                else if (reader.AttributeCount == 6)
                {
                    reader.MoveToAttribute(1);
                    reader.ReadAttributeValue();

                    string type = reader.ReadContentAsString().ToLower();

                    reader.MoveToNextAttribute();
                    reader.ReadAttributeValue();

                    int x = reader.ReadContentAsInt();

                    reader.MoveToNextAttribute();
                    reader.ReadAttributeValue();

                    int y = reader.ReadContentAsInt();

                    reader.MoveToNextAttribute();
                    reader.ReadAttributeValue();

                    int width = reader.ReadContentAsInt();

                    reader.MoveToNextAttribute();
                    reader.ReadAttributeValue();

                    int height = reader.ReadContentAsInt();

                    string id = "-1";
                    int minLevel = 0, maxLevel = User.MaxLevel;
                    int minDifficulty = 0, maxDifficulty = 999;
                    int rotDegrees = 0;

                    reader.ReadString();

                    int depth2 = reader.Depth;
                    int chance = 100;

                    reader.ReadString();

                    while (reader.Depth != depth2)
                    {
                        reader.MoveToFirstAttribute();
                        reader.ReadAttributeValue();

                        string str = reader.ReadContentAsString().ToLower();

                        if (str == "id")
                        {
                            reader.MoveToNextAttribute();
                            reader.ReadAttributeValue();

                            if (reader.ReadContentAsString().Contains(','))
                            {
                                string[] sids = reader.ReadContentAsString().Trim().Split(',');

                                id = sids[r.Next(sids.Length)];
                            }
                            else
                            {
                                id = reader.ReadContentAsString();
                            }
                        }
                        else if (str == "minlevel")
                        {
                            reader.MoveToNextAttribute();
                            reader.ReadAttributeValue();

                            minLevel = reader.ReadContentAsInt();
                        }
                        else if (str == "maxlevel")
                        {
                            reader.MoveToNextAttribute();
                            reader.ReadAttributeValue();

                            maxLevel = reader.ReadContentAsInt();
                        }
                        else if (str == "mindifficulty")
                        {
                            reader.MoveToNextAttribute();
                            reader.ReadAttributeValue();

                            minDifficulty = reader.ReadContentAsInt();
                        }
                        else if (str == "maxdifficulty")
                        {
                            reader.MoveToNextAttribute();
                            reader.ReadAttributeValue();

                            maxDifficulty = reader.ReadContentAsInt();
                        }
                        else if (str == "chance")
                        {
                            reader.MoveToNextAttribute();
                            reader.ReadAttributeValue();

                            chance = reader.ReadContentAsInt();
                        }
                        else if (str == "rotation" || str == "rot" || str == "direction")
                        {
                            reader.MoveToNextAttribute();
                            reader.ReadAttributeValue();

                            rotDegrees = reader.ReadContentAsInt();
                        }
                        else if (str == "unless")
                        {
                            reader.MoveToNextAttribute();
                            reader.ReadAttributeValue();

                            string[] sids = reader.ReadContentAsString().Trim().Split(',');

                            foreach (string sid in sids)
                            {
                                int rid = int.Parse(sid);

                                if (createdIDs.Count <= rid) { continue; }

                                if (createdIDs[rid])
                                {
                                    chance = -1; // remove any chance of creation
                                }
                            }
                        }
                        else if (str == "atleast")
                        {
                            reader.MoveToNextAttribute();
                            reader.ReadAttributeValue();

                            string[] sids = reader.ReadContentAsString().Trim().Split(',');

                            bool should = true;

                            foreach (string sid in sids)
                            {
                                int rid = int.Parse(sid);

                                if (createdIDs.Count <= rid) { if (rid != createdIDs.Count) { should = false; } continue; }

                                if (createdIDs[rid])
                                {
                                    should = false;
                                }
                            }

                            if (should) { chance = 100; }
                        }
                        else
                        {
                            reader.MoveToNextAttribute();
                            reader.ReadAttributeValue();
                        }

                        if (depth2 != reader.Depth)
                        {
                            reader.ReadToNextSibling("property");
                        }
                    }

                    if (depth2 != reader.Depth)
                    {
                        reader.ReadInnerXml();
                    }

                    reader.ReadEndElement();
                    reader.ReadEndElement();

                    /*
                    if (id >= MonsterEntity.TypeCache.Count && type == "monster")
                    {
                        if (!Boss)
                        {
                            id += PluginManager.Plugins[PluginManager.Rooms[States.DungeonSettingsState.Area]
                                [MapID - StartMapCount].PluginID].EntityStart;
                        }
                        else if (Boss)
                        {
                            id += PluginManager.Plugins[PluginManager.BossRooms[States.DungeonSettingsState.Area]
                                [MapID - StartBossCount].PluginID].EntityStart;
                        }
                    }
                    else if (id >= Items.Item.TypeCache.Count && type == "item")
                    {
                        if (!Boss)
                        {
                            id += PluginManager.Plugins[PluginManager.Rooms[States.DungeonSettingsState.Area]
                                [MapID - StartMapCount].PluginID].ItemStart;
                        }
                        else if (Boss)
                        {
                            id += PluginManager.Plugins[PluginManager.BossRooms[States.DungeonSettingsState.Area]
                                [MapID - StartBossCount].PluginID].ItemStart;
                        }
                    }
                    */
                    if (User.Level >= minLevel && User.Level <= maxLevel &&
                        Difficulty >= minDifficulty && Difficulty <= maxDifficulty && r.Next(100) <= chance)
                    {
                        createdIDs.Add(true);
                        if (type == "monster")
                        {
                            Entities.Add(Entity.CreateMonster(id, maxLevel, x, y, Difficulty));
                        }
                        else if (type == "item")
                        {
                            Entities.Add(Entity.CreateItem(int.Parse(id), x, y));
                        }
                        else if (type == "npc")
                        {
                            Entities.Add(Entity.CreateNPC(int.Parse(id), x, y));
                        }
                        else if (type == "furnace")
                        {
                            FurnaceEntity fr = new FurnaceEntity();
                            fr.Position.X = x;
                            fr.Position.Y = y;

                            Entities.Add(fr);
                        }
                        else if (type == "exit")
                        {
                            ExitEntity ex = new ExitEntity();
                            ex.Position.X = x;
                            ex.Position.Y = y;

                            Entities.Add(ex);
                        }
                        else if (type == "entity")
                        {
                            Entity e = Entity.Get(int.Parse(id));
                            e.Position = new Vector2(x, y);

                            Entities.Add(e);
                        }
                        else
                        {
                            throw new XmlException("Cannot parse room: " + mapID + " (Invalid Object: " + type + ")");
                        }


                        if (Entities[Entities.Count - 1] != null)
                        {
                            Entities[Entities.Count - 1].Position += Position * new Vector2(Width * TileWidth, Height * TileHeight);
                            Entities[Entities.Count - 1].Rotation = MathHelper.ToRadians(rotDegrees);
                        }
                    }
                    else
                    {
                        createdIDs.Add(false);
                    }
                }
                else
                {
                    throw new XmlException("Cannot parse room: " + mapID + " (Invalid Object) {" + reader.AttributeCount + "}");
                }

                reader.ReadToNextSibling("object");
            }

            reader.ReadEndElement();
        }

        #endregion

        public void Update(PlayerEntity player)
        {
            if (!IsCleared)
            {
                for (int i = 0; i < Entities.Count; i++)
                {
                    if (Entities[i] is TileEntity)
                    {
                        TileEntity t = (TileEntity)Entities[i];

                        if (t.Data > 7) { continue; }


                        t.Data = (byte)((t.Data % 4) + 4);
                        t.Texture = Main.ContentManager.Load<Texture2D>("DungeonDoors");
                        t.SetGID(t.InitialGID, TileWidth, TileHeight);
                    }
                }
            }

            if (Util.Map.Current.GetRoomFromPosition(player.Position).Position == Position)
            {
                if (!AlreadyCleared && IsCleared)
                {
                    for (int i = 0; i < Entities.Count; i++)
                    {
                        if (Entities[i] is TileEntity)
                        {
                            TileEntity t = (TileEntity)Entities[i];

                            if (t.Data > 3)
                            {
                                while (t.Data > 3) { t.Data -= 4; }

                                t.Texture = Tilesets[0].Texture;
                                t.SetGID(t.InitialGID, TileWidth, TileHeight);
                            }
                        }
                        else if (Entities[i] is ExitEntity)
                        {
                            Entities[i].Texture = Main.ContentManager.Load<Texture2D>("Exit");
                        }
                    }

                    AlreadyCleared = true;
                }
            }

            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] == null) { Entities.RemoveAt(i); i--; continue; }
                Entities[i].Update();

                if (Entities[i].CanDispose) { Entities[i].OnDestroy(); Entities.RemoveAt(i); i--; }

                Entities[i].CheckColliding(player);
            }

            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] == null) { continue; }
                for (int j = i + 1; j < Entities.Count; j++)
                {
                    if (Entities[j] == null) { continue; }
                    Entities[i].CheckColliding(Entities[j]);
                }
            }
        }

        public void Draw(Color color)
        {
            Main.SpriteBatch.Draw(Map, Position * new Vector2(Width * TileWidth, Height * TileHeight), null, color,
                0, Vector2.Zero, 1, SpriteEffects.None, 1);

            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] == null) { continue; }

                if (Entities[i] is TileEntity)
                {
                    TileEntity t = (TileEntity)Entities[i];
                    //if (!IsCleared) { continue; }

                    if (!TopConnection && (t.Data == 0 || t.Data == 4 || t.Data == 8)) { continue; }
                    if (!BottomConnection && (t.Data == 1 || t.Data == 5 || t.Data == 9)) { continue; }
                    if (!LeftConnection && (t.Data == 2 || t.Data == 6 || t.Data == 10)) { continue; }
                    if (!RightConnection && (t.Data == 3 || t.Data == 7 || t.Data == 11)) { continue; }

                }

                Entities[i].Draw();
            }
        }

        #endregion
    }
}
