using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DungeonInvaders.Util
{
    public struct Path
    {
        #region Fields

        public int CurrentNode;
        public List<Vector2> PathNodes;

        public bool TurnObject;

        public Entities.Entity Parent;

        #endregion

        #region Methods

        public Path(List<Vector2> Points = null, bool Turn = true)
        {
            PathNodes = Points == null ? new List<Vector2>() : Points;
            TurnObject = Turn;
            CurrentNode = 0;
            Parent = null;
        }

        public void Step(float Speed = 3, float MinError = 2)
        {
            if (IsFinished())
            {
                Parent.Speed = Vector2.Zero;
                return;
            }

            Parent.Speed = Vector2.Zero;

            if (Vector2.Distance(Parent.Position, PathNodes[CurrentNode]) <= MinError)
            {
                CurrentNode++;
                if (CurrentNode >= PathNodes.Count)
                {
                    return;
                }
            }

            float rotation = (float)Math.Atan2((PathNodes[CurrentNode] - Parent.Position).X,
                    (-PathNodes[CurrentNode] - -Parent.Position).Y);

            if (TurnObject)
            {
                Parent.Rotation = rotation;
            }

            Parent.Speed = new Vector2((float)Math.Sin(rotation) * Speed,
                (float)Math.Cos(rotation) * -Speed);
        }

        public bool IsFinished()
        {
            return CurrentNode >= PathNodes.Count || !(PathNodes.Count > 0);
        }

        public void Restart()
        {
            CurrentNode = 0;
        }

        public void Clear()
        {
            CurrentNode = 0;
            PathNodes = new List<Vector2>();
        }

        #endregion
    }
}
