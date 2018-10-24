using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonInvaders.Plugins;

namespace DungeonInvaders.Util.Items
{
    public class Item
    {
        #region Types

        public enum Class
        {
            HeadGear,
            BodyGear,
            LegGear,
            Weapon,
            Ring,
            Necklace,
            Spell,
            Arrow,
            Item
        }

        #endregion
        
        
        #region Fields

        public static List<Item> TypeCache = new List<Item>();

        public int Cooldown { get; set; }
        public Texture2D Texture;
        public String Name = "";

        public int CooldownTimer = 0;
        public int RequiredLevel = 0, SecondaryRequiredLevel = 0;

        public Class ClassType = Class.Item, SecondaryClass = Class.Item;

        #endregion

        #region Properties

        public virtual String Description
        {
            get { return ""; }
        }

        #endregion

        #region Methods

        public virtual bool OnUse(Entities.Entity user) { return false; }

        public static int Register(Item item)
        {
            TypeCache.Add(item);
            return TypeCache.Count - 1;
        }

        public static Item Get(int ID)
        {
            if (ID >= TypeCache.Count)
            {
                return PluginManager.Items[ID - TypeCache.Count];
            }

            return TypeCache[ID];
        }

        #endregion
    }
}
