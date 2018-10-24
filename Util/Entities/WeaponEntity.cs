using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Util.Entities
{
    public class WeaponEntity : Entity
    {
        #region Fields

        static List<WeaponEntity> TypeCache = new List<WeaponEntity>();

        public LiveEntity.DamageType DamageType = LiveEntity.DamageType.Melee;

        public int Power = 0;

        public int LiveHits = 1;

        List<LiveEntity> HitEntities = new List<LiveEntity>();

        public Entity User = null;
        public float Offset;

        #endregion

        #region Methods

        public WeaponEntity()
        {
            IsPhysical = false;
        }

        public virtual void OnStart() { }

        public override void OnCollide(Entity entity)
        {
            if (entity.Type == EntityType.Enemy)
            {
                MonsterEntity live = (MonsterEntity)entity;
                if (!HitEntities.Contains(live))
                {
                    int pwr = Power;

                    if (User is PlayerEntity)
                    {
                        PlayerEntity player = (PlayerEntity)User;

                        if (DamageType == LiveEntity.DamageType.Melee)
                        {
                            if (player.Strength > player.Attack)
                            {
                                pwr += Random.Next(player.Attack, player.Strength);
                            }
                            else
                            {
                                pwr += player.Strength;
                            }
                        }
                        else if (DamageType == LiveEntity.DamageType.Arrow)
                        {
                            pwr += Random.Next(player.Archery / 4, player.Archery);
                        }
                        else if (DamageType == LiveEntity.DamageType.Magic)
                        {
                            pwr += Random.Next(player.Magic / 4, player.Magic);
                        }
                    }

                    live.TakeDamage(pwr, this, DamageType);
                    HitEntities.Add(live);
                    LiveHits--;
                }
            }

            if (LiveHits < 1)
            {
                CanDispose = true;
            }
        }

        public static int Register(WeaponEntity weapon)
        {
            TypeCache.Add(weapon);

            return TypeCache.Count - 1;
        }

        public static WeaponEntity Get(int id)
        {
            if (id < 0 || id >= TypeCache.Count)
            {
                return null;
            }

            var w = (WeaponEntity)TypeCache[id].MemberwiseClone();
            w.HitEntities = new List<LiveEntity>();

            return w;
        }

        #endregion
    }
}
