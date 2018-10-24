using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DungeonInvaders.Util
{
    public static class Camera
    {
        #region Fields

        static Vector2 _Position = Vector2.Zero, _Scale = Vector2.One * 2;
        static float _Rotation = 0;

        #endregion

        #region Properties

        public static Matrix View { get; private set; }

        public static Vector2 Position
        {
            get
            {
                return _Position;
            }

            set
            {
                _Position = value;
                RebuildMatrix();
            }
        }

        public static Vector2 Scale
        {
            get
            {
                return _Scale;
            }

            set
            {
                _Scale = value;
                RebuildMatrix();
            }
        }

        public static float Rotation
        {
            get
            {
                return _Rotation;
            }

            set
            {
                _Rotation = value;
                RebuildMatrix();
            }
        }

        #endregion


        #region Methods

        public static void AlignWithEntity(Entities.Entity entity)
        {
            Position = entity.Position;
        }

        static void RebuildMatrix()
        {
            View = Matrix.CreateTranslation(new Vector3(-_Position.X, -Position.Y, 0)) * Matrix.CreateRotationZ(_Rotation)
                    * Matrix.CreateScale(_Scale.X, _Scale.Y, 1);
        }

        #endregion
    }
}
