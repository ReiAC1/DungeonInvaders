using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Gui
{
    public class GuiTitleBackground : GuiObject
    {
        #region Fields

        static Texture2D Background;
        static Vector2 BackgroundPos = Vector2.Zero;

        #endregion

        #region Methods

        public GuiTitleBackground()
        {
            if (Background == null)
            {
                Background = Main.ContentManager.Load<Texture2D>("Titlescreen");
            }
        }

        public override void Update()
        {
            float speed = 0.5f;

            float maxX = 0, minX = -(Background.Width - Main.Graphics.PreferredBackBufferWidth);
            float maxY = 0, minY = -(Background.Height - Main.Graphics.PreferredBackBufferHeight);

            if (BackgroundPos.Y >= maxY && BackgroundPos.X > minX)
            {
                BackgroundPos.Y = maxY;
                BackgroundPos.X -= speed;
            }
            else if (BackgroundPos.X <= minX && BackgroundPos.Y > minY)
            {
                BackgroundPos.X = minX;
                BackgroundPos.Y -= speed;
            }
            else if (BackgroundPos.Y <= minY && BackgroundPos.X < maxX)
            {
                BackgroundPos.Y = minY;
                BackgroundPos.X += speed;
            }
            else
            {
                BackgroundPos.X = maxX;
                BackgroundPos.Y += speed;
            }
        }

        public override void Draw()
        {
            Main.SpriteBatch.Draw(Background, BackgroundPos, Color.White);
        }

        #endregion
    }
}
