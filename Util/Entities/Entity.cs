using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonInvaders.Plugins;

namespace DungeonInvaders.Util.Entities
{
    public enum EntityType
    {
        Ally,
        Enemy,
        NPC,
        Item,
        Door,
        Misc
    }

    public class Entity
    {
        #region Fields

        protected static Random Random = new Random();

        public Vector2 Position = Vector2.Zero, Size = new Vector2(32, 32), Origin = new Vector2(16, 16);
        public Vector2 PreviousPosition = Vector2.Zero, Speed = Vector2.Zero;
        public float Rotation = 0;

        public string Name = "";

        public EntityType Type = EntityType.Misc;

        public Texture2D Texture = null;
        public Rectangle? Source = null;

        public Color DrawColor = Color.White;

        public int Frame;
        public float FrameTransition = 0.3f;
        float FrameInc = 0;

        public int Knockback = 4;
        public int Throwback = 0;
        public int MaxKnockbackFrames = 8;
        public int KnockbackFrames = 0;

        public float ThrowRot = 0;

        bool IsPlugin = false;
        #endregion

        #region Properties
        public bool CanDispose { get; set; }

        public bool IsPhysical { get; protected set; }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X - (int)Origin.X, (int)Position.Y - (int)Origin.Y, (int)Size.X, (int)Size.Y);
            }
        }

        #endregion

        #region Methods

        public Entity()
        {
            IsPhysical = true;

            if (GetType().FullName.Contains("Ruby"))
            {
                object o;
                if (PluginManager.Engine.Operations.TryGetMember(this, "on_create", out o))
                {
                    PluginManager.Engine.Operations.Invoke(o);
                }
            }
        }

        public bool CheckColliding(Entity entity)
        {
            Rectangle a, b;

            Vector2 offset = new Vector2((float)Math.Sin(Rotation) * ((Size.X / 2) - Origin.X),
                (float)Math.Cos(Rotation) * -((Size.Y / 2) - Origin.Y));

            Vector2 offset2 = new Vector2((float)Math.Sin(entity.Rotation) * ((entity.Size.X / 2) - entity.Origin.X),
                (float)Math.Cos(entity.Rotation) * -((entity.Size.Y / 2) - entity.Origin.Y));

            a = Bounds;
            b = entity.Bounds;

            a.X += (int)offset.X;
            a.Y += (int)offset.Y;

            b.X += (int)offset2.X;
            b.Y += (int)offset2.Y;

            if (a.Intersects(b))
            {
                OnCollide(entity);
                entity.OnCollide(this);
            }

            return a.Intersects(b);
        }

        public virtual void OnDestroy(bool destroyState = false)
        {
            if (GetType().FullName.Contains("Ruby"))
            {
                object o;
                if (PluginManager.Engine.Operations.TryGetMember(this, "on_destroy", out o))
                {
                    PluginManager.Engine.Operations.Invoke(o);
                }
            }
        }

        public virtual void Update()
        {
            if (GetType().FullName.Contains("Ruby"))
            {
                object o;
                if (PluginManager.Engine.Operations.TryGetMember(this, "on_update", out o))
                {
                    PluginManager.Engine.Operations.Invoke(o);
                }
            }

            if (Texture == null) { CanDispose = true; }

            if (IsPhysical)
            {
                Rectangle bound2;

                if (Speed.X != 0)
                {
                    Position.X += Speed.X;
                    if (!Map.Current.CanMoveToPosition(this, out bound2))
                    {
                        Rectangle intersection = Rectangle.Intersect(Bounds, bound2);

                        //if (Speed.X > 0) { Position.X -= intersection.Width + 1; }
                        //else if (Speed.X < 0) { Position.X += intersection.Width + 1; }

                        //Speed.X = 0;

                        Position.X -= Speed.X;
                    }
                }

                if (Speed.Y != 0)
                {
                    Position.Y += Speed.Y;
                    if (!Map.Current.CanMoveToPosition(this, out bound2))
                    {
                        Rectangle intersection = Rectangle.Intersect(Bounds, bound2);

                        //if (Speed.Y > 0) { Position.Y -= intersection.Height + 1; }
                        //else if (Speed.Y < 0) { Position.Y += intersection.Height + 1; }

                        //Speed.Y = 0;

                        Position.Y -= Speed.Y;
                    }
                }
            }
            else
            {
                Position += Speed;
            }

            PreviousPosition = Position;
        }

        public virtual void OnCollide(Entity entity)
        {
            if (GetType().FullName.Contains("Ruby"))
            {
                object o;
                if (PluginManager.Engine.Operations.TryGetMember(this, "on_collide", out o))
                {
                    PluginManager.Engine.Operations.Invoke(o, new object[] { entity });
                }
            }
        }

        public void Draw()
        {
            OnDraw();

            if (Texture == null) { return; }

            Rectangle? src = Source;

            if (Source == null)
            {
                if (Speed.X != 0 || Speed.Y != 0)
                {
                    int xSize = Texture.Width / (int)Size.X;
                    int ySize = Texture.Height / (int)Size.Y;

                    int x = (int)Math.Floor((float)Frame % xSize);
                    int y = (int)Math.Floor((float)Frame / xSize);

                    src = new Rectangle(x * (int)Size.X, y * (int)Size.Y, (int)Size.X, (int)Size.Y);

                    FrameInc += FrameTransition;

                    if (FrameInc >= 1)
                    {
                        Frame = (int)((float)(Frame + 1) % (xSize * ySize));
                        FrameInc = 0;
                    }
                }
                else
                {
                    src = new Rectangle(0, 0, (int)Size.X, (int)Size.Y);
                }
            }

            Rectangle bnds = Bounds;
            bnds.X += (int)Origin.X;
            bnds.Y += (int)Origin.Y;

            Main.SpriteBatch.Draw(Texture, bnds, src, DrawColor,
                Rotation, Origin, SpriteEffects.None, 0);
        }

        protected virtual void OnDraw()
        {
            if (GetType().FullName.Contains("Ruby"))
            {
                object o;
                if (PluginManager.Engine.Operations.TryGetMember(this, "on_draw", out o))
                {
                    PluginManager.Engine.Operations.Invoke(o);
                }
            }
        }

        public static Entity CreateMonster(string ID, int maxLevel, int x, int y, int difficulty)
        {
            ID = ID.ToLower();
            if (ID == "random")
            {
                List<string> applicableIDs = new List<string>();

                int i = 0;

                int playerLevel = ((((User.StatLevels[0] + User.StatLevels[2]) / 2) + User.StatLevels[3] + User.StatLevels[4]) / 3) +
                        User.StatLevels[1] + User.StatLevels[5];

                int range = (int)Math.Round(playerLevel / (1 + (maxLevel * 0.75f)));

                foreach(MonsterEntity monster in MonsterEntity.TypeCache)
                {
                    int level = (((monster.MaxHealth - 15) / 10) + monster.Damage + monster.Defense);

                    if (Math.Abs(playerLevel - level) <= range)
                    {
                        applicableIDs.Add(monster.Name.ToLower());
                    }

                    i++;
                }

                foreach(Entity entity in PluginManager.Entities)
                {
                    if (!(entity is MonsterEntity)) { i++; continue; }

                    MonsterEntity monster = entity as MonsterEntity;

                    int level = (((monster.MaxHealth - 15) / 10) + monster.Damage + monster.Defense);

                    if (Math.Abs(playerLevel - level) <= range)
                    {
                        applicableIDs.Add(monster.Name.ToLower());
                    }

                    i++;
                }

                if (applicableIDs.Count == 0)
                {
                    return CreateMonster("1", -1, x, y, difficulty);
                }

                ID = applicableIDs[Random.Next(applicableIDs.Count)];
            }

            MonsterEntity en = null;

            int id = -100;

            int.TryParse(ID, out id);

            if (id != 0 || (id == 0 && ID.Length == 1))
            {
                if (id >= MonsterEntity.TypeCache.Count)
                {
                    en = (MonsterEntity)PluginManager.Entities[id - MonsterEntity.TypeCache.Count].MemberwiseClone();
                }
                else
                {
                    en = MonsterEntity.Get(id);
                }
            }

            if (en == null)
            {
                foreach (MonsterEntity mn in MonsterEntity.TypeCache)
                {
                    if (mn.Name.ToLower() == ID)
                    {
                        en = (MonsterEntity)mn.MemberwiseClone();
                        break;
                    }
                }
            }

            if (en == null)
            {
                foreach (Entity e in PluginManager.Entities)
                {
                    if (e == null) { continue; }
                    if (!(e is MonsterEntity)) { continue; }

                    MonsterEntity mn = (MonsterEntity)e;

                    if (mn.Name.ToLower() == ID)
                    {
                        en = (MonsterEntity)mn.MemberwiseClone();
                    }
                }
            }

            if (en == null)
            {
                return null;
            }

            en.MaxHealth *= (int)((difficulty + 1) * 1.5f);
            en.Damage += difficulty;
            en.Defense += difficulty;
            en.MeleeDefense += difficulty;
            en.MagicDefense += difficulty;
            en.ArrowDefense += difficulty;

            if (en == null) { return null; }

            en.Position = new Vector2(x, y);

            return en;
        }

        public static Entity CreateNPC(int ID, int x, int y)
        {
            return null;
        }

        public static Entity CreateItem(int ID, int x, int y)
        {
            var e = new ItemEntity(ID, 1);
            e.Position = new Vector2(x, y);
            return e;
        }

        public static Entity Get(int ID)
        {
            return (Entity)PluginManager.Entities[ID].MemberwiseClone();
        }

        #endregion
    }
}
