using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonInvaders.Util.Entities;

namespace DungeonInvaders.Util.Items
{
    public enum WeaponType
    {
        Sword,
        Axe,
        Mace,
        Hammer,
        Pickaxe,
        Bow,
        Staff
    }

    public class WeaponItem : Item
    {
        #region Properties

        public LiveEntity.DamageType Type { get; set; }
        public bool Consumes { get; set; }

        public int WeaponID { get; set; }

        public float DistanceFromUser { get; set; }

        public WeaponType WeaponType { get; set; }

        public override String Description
        {
            get
            {
                return "Atk: " + WeaponEntity.Get(WeaponID).Power + " Spd: " +
                    (Main.Graphics.GraphicsDevice.DisplayMode.RefreshRate / Cooldown) + " Atk Lv Req: " + RequiredLevel;
            }
        }

        #endregion

        #region Methods

        public WeaponItem(int weaponID, int distanceFromUser, bool consume)
        {
            WeaponID = weaponID;
            DistanceFromUser = distanceFromUser;
            Consumes = consume;
            ClassType = Item.Class.Weapon;
        }

        public override bool OnUse(Entity user)
        {
            if (CooldownTimer > 0)
            {
                return false;
            }

            if (user is LiveEntity)
            {
                LiveEntity live = (LiveEntity)user;

                CooldownTimer = Cooldown;

                WeaponEntity weapon = WeaponEntity.Get(WeaponID);
                weapon.User = user;
                weapon.Offset = DistanceFromUser;
                weapon.Rotation = live.Rotation;
                weapon.Position = live.Position + new Vector2((float)Math.Sin(live.Rotation) * DistanceFromUser,
                (float)Math.Cos(live.Rotation) * -DistanceFromUser);

                Room r = Map.Current.GetRoomFromPosition(user.Position);

                if (r == null)
                {
                    return false;
                }

                weapon.OnStart();
                r.Entities.Add(weapon);

                return Consumes;
            }

            return false;
        }

        #endregion
    }
}
