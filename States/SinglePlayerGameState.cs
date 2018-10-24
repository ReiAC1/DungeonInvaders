using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonInvaders.Gui;
using DungeonInvaders.Util;
using DungeonInvaders.Plugins;

namespace DungeonInvaders.States
{
    public class SinglePlayerGameState : GameState
    {
        #region Methods

        public override void Start()
        {
            PluginManager.Initialize();
            PluginManager.Load();
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            PluginManager.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);


        }

        #endregion
    }
}
