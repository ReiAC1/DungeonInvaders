using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace DungeonInvaders.Util.Items
{
    public class TeleportHomeItem : Item
    {
        static SoundEffect Effect;

        public override string Description
        {
            get { return "Takes you back to the start of the dungeon."; }
        }

        public override bool OnUse(Entities.Entity user)
        {
            if (Effect == null)
            {
                Effect = Main.ContentManager.Load<SoundEffect>("teleport");
            }

            if (CooldownTimer > 0 || !Map.Current.GetRoomFromPosition(user.Position).IsCleared)
            {
                return false;
            }

            if (user is Entities.PlayerEntity)
            {
                CooldownTimer = Cooldown;

                Entities.PlayerEntity player = (Entities.PlayerEntity)user;

                float oSize = States.GameState.LightLevel;
                float size = oSize;

                player.Speed = Vector2.Zero;

                Effect.Play(Settings.SFXVolume, 0, 0);

                float speed = 7;

                while (size > 0)
                {
                    size -= speed;
                    size = size < 0 ? 0 : size;
                    States.GameState.LightLevel = size;
                    System.Windows.Forms.Application.DoEvents();
                    Main.Singleton.MiniUpdate();
                    Main.Singleton.MiniDraw();
                }

                player.Position = player.StartingPosition;
                player.Update();
                player.Speed = Vector2.Zero;

                while (size < oSize)
                {
                    size += speed;
                    size = size > oSize ? oSize : size;
                    States.GameState.LightLevel = size;
                    System.Windows.Forms.Application.DoEvents();
                    Main.Singleton.MiniUpdate();
                    Main.Singleton.MiniDraw();
                }
            }

            return false;
        }
    }
}
