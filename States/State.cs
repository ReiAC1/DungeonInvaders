using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.States
{
    public abstract class State
    {
        #region Properties

        protected GraphicsDevice GraphicsDevice { get { return Main.Graphics.GraphicsDevice; } }
        protected SpriteBatch SpriteBatch { get { return Main.SpriteBatch; } }
        protected ContentManager Content { get { return Main.ContentManager; } }

        #endregion

        #region Methods

        public virtual void Start() { }
        public virtual void Stop() { }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }

        #endregion
    }
}
