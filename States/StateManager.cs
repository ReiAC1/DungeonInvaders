using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DungeonInvaders.States
{
    public static class StateManager
    {
        static State _State = null;

        public static State CurrentState
        {
            get { return _State; }

            set
            {
                if (_State != null) { _State.Stop(); }

                _State = value;

                if (_State != null) { _State.Start(); }
            }
        }

        public static void Update(GameTime gameTime)
        {
            if (_State != null) { _State.Update(gameTime); }
        }

        public static void Draw(GameTime gameTime)
        {
            if (_State != null) { _State.Draw(gameTime); }
        }
    }
}
