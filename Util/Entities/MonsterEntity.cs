using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Util.Entities
{
    public class MonsterEntity : LiveEntity
    {
        #region Fields

        public static List<MonsterEntity> TypeCache = new List<MonsterEntity>();

        public PathFindingParams PathParams = new PathFindingParams();

        int AttackCooldownTimer = 0;

        public short[] DropItems = null;
        public short[] DropItemAmounts = null;
        public byte ItemDropChance = 30;

        int _AttackCooldown = 0;


        #endregion

        #region Properties

        public Point Goal
        {
            get
            {
                return PathParams.Goal;
            }

            set
            {
                PathParams.Goal = value;
            }
        }

        public int Damage       { get; set; }

        public int Defense      { get; set; }
        public int MeleeDefense { get; set; }
        public int MagicDefense { get; set; }
        public int ArrowDefense { get; set; }

        public int AttackCooldown
        {
            get { return _AttackCooldown; }

            set
            {
                float cd = value / 60f;

                _AttackCooldown = (int)(cd * Main.Graphics.GraphicsDevice.DisplayMode.RefreshRate);

            }
        }

        #endregion

        #region Methods

        internal MonsterEntity()
        {
            PathParams.Speed = 2;

            Map.Paths.Add(PathParams);
        }

        public override void OnDestroy(bool destroyState=false)
        {
            base.OnDestroy(false);

            if (destroyState) { return; }

            int xp = ((MaxHealth / 2) + Damage + ((Defense + MeleeDefense + MagicDefense + ArrowDefense) / 4)) / 3;

            User.AddEXP(xp);

            if (Random.Next(100) < ItemDropChance && DropItems != null && DropItemAmounts != null)
            {
                int id = Random.Next(DropItems.Length);
                short item = DropItems[id];
                short amt = (short)Random.Next(1, DropItemAmounts[id]);

                ItemEntity itm = new ItemEntity(item, amt);
                itm.Position = Position;

                Map.Current.GetRoomFromPosition(Position).Entities.Add(itm);
            }
        }

        public override void Update()
        {
            PathParams.Path.Parent = this;
            AttackCooldownTimer--;

            base.Update();
        }

        public override void OnCollide(Entity entity)
        {
            if (AttackCooldownTimer > 0)
            {
                return;
            }

            if (entity.Type == EntityType.Ally)
            {
                LiveEntity live = (LiveEntity)entity;

                live.TakeDamage(Damage, this, DamageType.Melee);

                AttackCooldownTimer = AttackCooldown;
            }
        }

        public override void TakeDamage(int amount, Entity e, LiveEntity.DamageType type)
        {
            base.TakeDamage(amount, e, type);
            if (type == DamageType.Melee)
            {
                amount -= MeleeDefense;
            }
            else if (type == DamageType.Magic)
            {
                amount -= MagicDefense;
            }
            else if (type == DamageType.Arrow)
            {
                amount -= ArrowDefense;
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

        public static new MonsterEntity Get(int id)
        {
            if (id < 0 || id >= TypeCache.Count)
            {
                return null;
            }

            return (MonsterEntity)TypeCache[id].MemberwiseClone();
        }

        public static int Register(MonsterEntity monster)
        {
            TypeCache.Add(monster);

            return TypeCache.Count - 1;
        }

        #endregion
    }
}
