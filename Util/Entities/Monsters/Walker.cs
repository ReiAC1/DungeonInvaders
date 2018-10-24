using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Util.Entities.Monsters
{
    public class Walker : MonsterEntity
    {

        #region Methods

        public Walker(string texture)
        {
            Type = EntityType.Enemy;

            Texture = Main.LoadImage(texture);
            
            IsPhysical = false;
        }

        public override void Update()
        {
            Vector2 prev = PreviousPosition;

            base.Update();

            PathParams.Goal = Tile.PosToPoint(Map.Current.Player.Position);

            Room r = Map.Current.GetRoomFromPosition(Position);

            if (r == null) { return; }

            if (r.Position != Map.Current.GetRoomFromPosition(Map.Current.Player.Position).Position)
            {
                return;
            }

            Point offset = new Point();
            offset.X = (int)Math.Floor(r.Position.X * r.Width);
            offset.Y = (int)Math.Floor(r.Position.Y * r.Height);

            PathParams.LastResult = r.Pathfinder.SearchPath(ref PathParams.Path,
                offset, Tile.PosToPoint(Position), PathParams.Goal);

            if (KnockbackFrames < 1)
            {
                IsPhysical = false;
                if (PathParams.LastResult)
                {
                    PathParams.Path.PathNodes.Add(Map.Current.Player.Position);
                    PathParams.Path.Step(PathParams.Speed, PathParams.MinError);
                }
                else
                {
                    Position = prev;
                    Speed = Vector2.Zero;
                }
            }
            else
            {
                IsPhysical = true;
                KnockbackFrames -= 1;

                Speed = new Vector2((float)Math.Sin(ThrowRot) * Throwback,
                (float)Math.Cos(ThrowRot) * Throwback);
            }
        }

        #endregion
    }
}
