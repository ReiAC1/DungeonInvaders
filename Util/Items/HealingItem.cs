using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using DungeonInvaders.Util.Entities;

namespace DungeonInvaders.Util.Items
{
    public class HealingItem : Item
    {
        #region Fields

        static SoundEffect DrinkEffect;

        #endregion

        #region Properties

        public int HealthRestore { get; protected set; }

        public override string Description
        {
            get { return "Health Restore: " + HealthRestore; }
        }

        #endregion

        public HealingItem(int restore)
        {
            HealthRestore = restore;

            if (DrinkEffect == null)
            {
                DrinkEffect = Main.ContentManager.Load<SoundEffect>("potion");
            }
        }

        public override bool OnUse(Entity user)
        {
            if (!(user is LiveEntity)) { return false; }

            LiveEntity live = (LiveEntity)user;

            if (live.Health < live.MaxHealth)
            {
                DrinkEffect.Play(Settings.SFXVolume, 0, 0);
                live.Health += HealthRestore;
                return true;
            }

            return false;
        }
    }
}
