using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace DungeonInvaders.Util.Entities.Weapons
{
    public class SwordEntity : WeaponEntity
    {
        public int Life = 16;
        public float SwingSpeed = MathHelper.ToRadians(4);
        float rotAdd = 0;

        public static SoundEffect Swing;
        public SoundEffectInstance SwingInstance;

        public override void OnStart()
        {
            Life = (int)((Life * 60f) / Main.Graphics.GraphicsDevice.DisplayMode.RefreshRate);
            SwingSpeed = (SwingSpeed * 60f) / Main.Graphics.GraphicsDevice.DisplayMode.RefreshRate;

            if (Swing == null)
            {
                Swing = Main.ContentManager.Load<SoundEffect>("sword-swing");
            }

            Swing.Play(Settings.SFXVolume, 0, 0);
        }

        public override void Update()
        {

            Position = User.Position +
                new Vector2((float)Math.Sin(User.Rotation) * Offset,
                (float)Math.Cos(User.Rotation) * -Offset);

            if (Life <= 1)
            {
                CanDispose = true;
            }

            Life--;

            rotAdd += SwingSpeed;

            Rotation = User.Rotation + rotAdd;

            User.Speed = Vector2.Zero;

            base.Update();
        }
    }
}
