using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonInvaders.Util.Entities
{
    public class PlayerEntity : LiveEntity
    {
        #region Fields

        public static short HeadGear = -1, BodyGear = -1, LegGear = -1, Ring = -1, Necklace = -1, Arrow = -1, Spell = -1,
            Weapon = 0, Item = 1;

        int PreviousHealth = 0, HealthRecharge = 0;

        public int MagicDefense, ArrowDefense, MeleeDefense;

        public int DamageImmuneFrames = 0;

        public Vector2 StartingPosition;

        public bool ShouldAttack = true;
        byte AtkHold = 0;

        #endregion

        #region Properties

        int MaxDamageImmune { get { return Main.Graphics.GraphicsDevice.DisplayMode.RefreshRate; } }

        public int Attack
        {
            get
            {
                return User.StatLevels[0];
            }
        }

        public int Defense
        {
            get
            {
                return User.StatLevels[1];
            }
        }

        public int Strength
        {
            get
            {
                return User.StatLevels[2];
            }
        }

        public int Magic
        {
            get
            {
                return User.StatLevels[3];
            }
        }

        public int Archery
        {
            get
            {
                return User.StatLevels[4];
            }
        }

        public int Mining
        {
            get
            {
                return User.StatLevels[6];
            }
        }

        public int Smithing
        {
            get
            {
                return User.StatLevels[7];
            }
        }

        public int Fishing
        {
            get
            {
                return User.StatLevels[8];
            }
        }

        public int Cooking
        {
            get
            {
                return User.StatLevels[9];
            }
        }

        public int Crafting
        {
            get
            {
                return User.StatLevels[10];
            }
        }

        public bool DamageImmune { get { return DamageImmuneFrames > 0; } }

        int HealthRechargeRate
        {
            get
            {
                return User.MaxStatLevel + 30 - (User.StatLevels[5] / 2);
            }
        }

        #endregion

        #region Methods

        public PlayerEntity()
        {
            MaxHealth = 15 + (User.StatLevels[5] * 10);
            Health = MaxHealth;

            Type = EntityType.Ally;

            Texture = Main.ContentManager.Load<Texture2D>("player_" + (User.IsMale ? "m" : "f"));
        }

        public override void Update()
        {
            //IsPhysical = !InputManager.IsPerformingAction(InputManager.Action.InversePrimarySecondary);

            DamageImmuneFrames -= 1;
            KnockbackFrames -= 1;

            //DrawColor.A = (!DamageImmune || (DamageImmuneFrames % 10 <= 4)) ? (byte)255 : (byte)0;

            Lighting.LightManager.Lights[0].Rotation = Rotation;

            if (StartingPosition.X == 0 && StartingPosition.Y == 0)
            {
                StartingPosition = Position;
            }

            if (KnockbackFrames < 1)
            {
                Speed = new Vector2(InputManager.IsMoving(InputManager.Movement.Sideways),
                InputManager.IsMoving(InputManager.Movement.Straight)) * 2;

            }
            else
            {
                Speed = new Vector2((float)Math.Sin(ThrowRot) * Throwback,
                (float)Math.Cos(ThrowRot) * Throwback);
            }

            MaxHealth = 15 + (User.StatLevels[5] * 10);

            if (Health == PreviousHealth)
            {
                HealthRecharge--;
                if (HealthRecharge < 1)
                {
                    HealthRecharge = HealthRechargeRate;
                    if (Health < MaxHealth) { Health++; }
                }
            }
            else
            {
                PreviousHealth = Health;
                HealthRecharge = HealthRechargeRate;
            }

            Rotation = (float)Math.Atan2((InputManager.CursorPosition - Position).X, (-InputManager.CursorPosition - -Position).Y);

            base.Update();

            Camera.Position = Position - new Vector2(Main.Graphics.PreferredBackBufferWidth / 2,
                Main.Graphics.PreferredBackBufferHeight / 2) / Camera.Scale;

            Lighting.LightManager.Lights[0].Position = Position;

            byte holdTime = 3;

            if (InputManager.IsPerformingAction(InputManager.Action.PrimaryTrigger) &&
                !((States.GameState)States.StateManager.CurrentState).HUD.IsOver() && ShouldAttack && AtkHold > holdTime)
            {
                if (Weapon == -1) { return; }
                if (User.Items[Weapon, 1] < 1) { return; }
                Items.Item item = Items.Item.Get(User.Items[Weapon, 0]);

                DamageImmuneFrames = 0;
                DrawColor = OriginColor;

                if (item.OnUse(this))
                {

                    User.Items[Weapon, 1]--;
                }
            }
            else if (InputManager.IsPerformingAction(InputManager.Action.SecondaryTrigger, true) &&
                !((States.GameState)States.StateManager.CurrentState).HUD.IsOver())
            {
                if (Item == -1) { return; }
                if (User.Items[Item, 1] < 1) { return; }
                Items.Item item = Items.Item.Get(User.Items[Item, 0]);

                if (item.OnUse(this))
                {
                    User.Items[Item, 1]--;
                }
            }

            {
                Items.Item item = Items.Item.Get(User.Items[Weapon, 0]);
                item.CooldownTimer -= 1;

                item = Items.Item.Get(User.Items[Weapon, 1]);
                item.CooldownTimer -= 1;
            }

            if (!InputManager.IsPerformingAction(InputManager.Action.PrimaryTrigger)) { AtkHold = 0; ShouldAttack = true; }
            else { AtkHold += 1; }
        }

        public override void TakeDamage(int amount, Entity e, LiveEntity.DamageType type)
        {
            if (DamageImmune) { return; }
            DamageImmuneFrames = MaxDamageImmune;

            base.TakeDamage(amount, e, type);
            
            if (type == DamageType.Melee)
            {
                amount -= Random.Next(MeleeDefense, Defense);
            }
            else if (type == DamageType.Magic)
            {
                amount -= Random.Next(MagicDefense, Defense);
            }
            else if (type == DamageType.Arrow)
            {
                amount -= Random.Next(ArrowDefense, Defense);
            }
            else
            {
                amount -= Defense;
            }

            if (amount < 1)
            {
                amount = 1;
            }

            Health -= amount;
        }

        #endregion
    }
}
