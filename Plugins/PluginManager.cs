using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonInvaders.Util;
using DungeonInvaders.Util.Entities;
using DungeonInvaders.Util.Items;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using IronRuby;

namespace DungeonInvaders.Plugins
{
    public static class PluginManager
    {
        #region Fields

        public static ScriptEngine Engine = Ruby.CreateEngine();

        public static List<Entity> Entities = new List<Entity>();
        public static List<Item> Items = new List<Item>();
        public static List<List<PluginRoom>> Rooms = new List<List<PluginRoom>>();
        public static List<List<PluginRoom>> BossRooms = new List<List<PluginRoom>>();

        public static List<Plugin> Plugins = new List<Plugin>();

        public static Ionic.Zip.ZipFile CurrentPlugin;

        #endregion

        #region Methods

        public static void Initialize()
        {
            Engine.Runtime.Globals.SetVariable("Entities__", Entities);
            Engine.Runtime.Globals.SetVariable("Items__", Items);
            Engine.Runtime.Globals.SetVariable("Rooms__", Rooms);
            Engine.Runtime.Globals.SetVariable("BossRooms__", BossRooms);

            var asm = System.Reflection.Assembly.GetAssembly(typeof(Entity));

            Engine.Runtime.LoadAssembly(asm);
            Engine.Runtime.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(Microsoft.Xna.Framework.Vector2)));

            ExecuteFile("Plugins/System.rb");
        }

        public static void Load()
        {
            for (int i = 0; i < 5; i++)
            {
                Rooms.Add(new List<PluginRoom>());
                BossRooms.Add(new List<PluginRoom>());
            }

            foreach (string pluginFile in Directory.GetFiles("Plugins", "*.xnb", SearchOption.TopDirectoryOnly))
            {
                CurrentPlugin = PluginPackager.LoadPlugin(pluginFile);

                if (CurrentPlugin.ContainsEntry("init.rb"))
                {
                    Plugin plugin = new Plugin();
                    plugin.Name = System.IO.Path.GetFileNameWithoutExtension(pluginFile);
                    plugin.EntityStart = Entities.Count;
                    plugin.GenerateUniqueID(plugin.Name);

                    var stream = CurrentPlugin["version.txt"].OpenReader();

                    var data = "";

                    for (int i = 0; i < stream.Length; i++)
                    {
                        data += (char)stream.ReadByte();
                    }

                    stream.Close();

                    plugin.Version = int.Parse(data);

                    Plugins.Add(plugin);

                    Main.ContentManager.RootDirectory = "Plugins/" + System.IO.Path.GetFileNameWithoutExtension(pluginFile);
                    Execute("$__root_directory__ = \"Plugins\\\\" + plugin.Name + "\"");
                    Execute("$__plugin_id__ = " + (Plugins.Count - 1));

                    stream = CurrentPlugin["init.rb"].OpenReader();

                    data = "";

                    for (int i = 0; i < stream.Length; i++)
                    {
                        data += (char)stream.ReadByte();
                    }

                    stream.Close();

                    Execute(data);

                    Main.ContentManager.RootDirectory = "Content";
                }
            }

            int ep = 0;

            foreach(Entity e in Entities)
            {
                Plugin plugin = Plugins[0];

                for (int i = 1; i < Plugins.Count; i++)
                {
                    if (ep < Plugins[i].EntityStart)
                    {
                        plugin = Plugins[i - 1];
                    }
                }

                int itemStart = plugin.Version != User.Version ? User.OldItemStarts[plugin.Version] : User.ItemStart;

                if (e is MonsterEntity)
                {
                    MonsterEntity monster = e as MonsterEntity;

                    if (monster.DropItems != null)
                    {
                        for (int i = 0; i < monster.DropItems.Length; i++)
                        {
                            if (monster.DropItems[i] >= itemStart)
                            {
                                monster.DropItems[i] = (short)(monster.DropItems[i] - itemStart + User.ItemStart);
                            }
                        }
                    }
                }
                else if (e is ItemEntity)
                {
                    ItemEntity item = e as ItemEntity;

                    if (item.ItemID >= itemStart)
                    {
                        item.ItemID = (short)(item.ItemID - itemStart + User.ItemStart);
                    }
                }

                ep++;
            }
        }

        public static void Unload()
        {
            Engine = Ruby.CreateEngine();
            Entities.Clear();
            Items.Clear();
            Rooms.Clear();
            BossRooms.Clear();

            GC.Collect();
        }

        public static dynamic Execute(string code)
        {
            return Engine.Execute(code, Engine.Runtime.Globals);
        }

        public static dynamic ExecuteFile(string file)
        {
            return Engine.ExecuteFile(file, Engine.Runtime.Globals);
        }

        #endregion
    }

    public struct Plugin
    {
        public String Name;
        public int EntityStart, RoomStart, ItemStart;
        public int Version, UniqueID;

        public void GenerateUniqueID(string str)
        {
            UniqueID = str.Length;

            unchecked
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        UniqueID ^= (i / 2) ^ (str[i] / 2);
                    }
                    else
                    {
                        UniqueID ^= i ^ str[i];
                    }
                }
            }
        }
    }

    public struct PluginRoom
    {
        public String Path;
        public int PluginID;

        public PluginRoom(string path, int id)
        {
            Path = path;
            PluginID = id;
        }
    }
}
