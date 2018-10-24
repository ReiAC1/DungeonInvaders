using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Util.Entities
{
    public class LiveEntity : Entity
    {
        #region Types

        public enum DamageType : byte
        {
            Melee,
            Magic,
            Arrow,
            Misc
        }

        #endregion

        #region Fields

        public int Health = int.MaxValue;

        public SoundEffect OnHitSoundEffect, OnDiedSoundEffect;

        static Texture2D HealthBar;

        public Color HitColor = Color.Red;
        public int HitFrames = 0;

        protected Color OriginColor = Color.White;

        #endregion

        #region Properties

        public int MaxHealth { get; set; }
        public int MaxHitFrames { get { return Main.Graphics.GraphicsDevice.DisplayMode.RefreshRate; } }

        #endregion

        #region Methods

        static LiveEntity()
        {
            HealthBar = new Texture2D(Main.Graphics.GraphicsDevice, 32, 1);

            for (int x = 0; x < HealthBar.Width; x++)
            {
                Color color = Color.Lerp(Color.Red, Color.Green, 1.25f / ((float)HealthBar.Width / x));
                HealthBar.SetData<Color>(0, new Rectangle(x, 0, 1, HealthBar.Height), new Color[] { color }, 0, 1);
            }
        }

        public override void Update()
        {
            if (HitFrames > 0)
            {
                DrawColor = (HitFrames % 10 <= 4 ? OriginColor : HitColor);
            }
            else if (HitFrames <= 0)
            {
                DrawColor = OriginColor;
            }

            --HitFrames;

            if (Health <= 0)
            {
                CanDispose = true;
            }
            else if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }

            base.Update();
        }

        public override void OnDestroy(bool destroyState = false)
        {
        }

        public virtual void TakeDamage(int amount, Entity e, DamageType type)
        {
            ThrowRot = (float)Math.Atan2(Position.X - e.Position.X, Position.Y - e.Position.Y);

            if (KnockbackFrames < 1)
            {
                KnockbackFrames = e.MaxKnockbackFrames;
                Throwback = e.Knockback;
                HitFrames = MaxHitFrames;
                OriginColor = DrawColor;
            }

            if (OnHitSoundEffect != null && (Health - amount > 0 || OnDiedSoundEffect == null))
            { OnHitSoundEffect.Play(Settings.SFXVolume, 0, 0); }
            else if (OnDiedSoundEffect != null && Health - amount <= 0)
            { OnDiedSoundEffect.Play(Settings.SFXVolume, 0, 0); }
        }

        protected override void OnDraw()
        {
            Rectangle source = new Rectangle(0, 0, (Health * HealthBar.Width) / MaxHealth, 4);

            Main.SpriteBatch.Draw(HealthBar, Position + Origin, source, Color.White, 0,
                new Vector2((source.Width / 2) + Origin.X, 0), 1, SpriteEffects.None, 0);
        }

        #endregion
    }
}
