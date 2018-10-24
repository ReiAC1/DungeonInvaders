using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonInvaders.Util.Items;

namespace DungeonInvaders.Util.Entities
{
    public class OreRockEntity : Entity
    {
        #region Fields

        static Random Random = new Random();

        public static List<int> Ores = new List<int>();
        public static List<byte> Chances = new List<byte>();

        public int Ore = 0;
        public int Amount = 0;

        #endregion

        #region Methods

        public OreRockEntity(int ore)
        {
            Ore = ore;

            Texture = Main.ContentManager.Load<Texture2D>("oreRock_" + ore);
            Amount = Random.Next(1, 5);
        }

        public override void OnCollide(Entity entity)
        {
            if (entity is WeaponEntity)
            {
                if (((WeaponEntity)entity).User is PlayerEntity && PlayerEntity.Weapon < -1)
                {

                    Item item = Item.Get(PlayerEntity.Weapon);

                    if (item is WeaponItem)
                    {
                        WeaponItem weapon = (WeaponItem)item;

                        if (weapon.WeaponType == WeaponType.Pickaxe)
                        {
                            if (Random.Next(100) <= Chances[Ore] && (Amount > 0 || Amount == -1))
                            {
                                Amount--;
                                User.AddItem(Ores[Ore], 1);
                                User.AddEXP((100 - Chances[Ore]) / 4);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
