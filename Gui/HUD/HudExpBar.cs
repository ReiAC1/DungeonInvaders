using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonInvaders.Util;

namespace DungeonInvaders.Gui.HUD
{
    public class HudExpBar : GuiObject
    {
        #region Fields

        static Texture2D Pixel;
        public Texture2D Frame, Bar;

        SpriteFont Font;

        #endregion

        #region Methods

        static HudExpBar()
        {
            Pixel = new Texture2D(Main.Graphics.GraphicsDevice, 1, 1);
            Pixel.SetData<Color>(new Color[] { Color.White });
        }

        public HudExpBar(SpriteFont font)
        {
            Font = font;
        }

        public override void Draw()
        {
            Rectangle bounds = new Rectangle(2, 2, 224, 14);
            Rectangle exp = new Rectangle(4, 4,
                (int)(((User.EXP - User.LevelEXP) * 220.0f) / (User.NextLevelEXP - User.LevelEXP)), 10);

            if (Frame != null)
            {
                Main.SpriteBatch.Draw(Frame, bounds, Color.White);
            }
            else
            {
                Main.SpriteBatch.Draw(Pixel, bounds, Color.DarkGray * 0.2f);
            }

            if (Bar != null)
            {
                Main.SpriteBatch.Draw(Bar, exp, new Rectangle(0, 0, exp.Width, exp.Height), Color.White);
            }
            else
            {
                Main.SpriteBatch.Draw(Pixel, exp, Color.DarkCyan * 0.5f);
            }

            TextHelper.DrawShadowString(Font, String.Format("Lv: {0} EXP: {1}/{2}", User.Level,
                User.EXP, User.NextLevelEXP), new Vector2(4, 2), Color.Black, new Color(50, 50, 50));
        }

        #endregion
    }
}
