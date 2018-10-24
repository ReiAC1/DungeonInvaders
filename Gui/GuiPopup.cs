using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonInvaders.Util;

namespace DungeonInvaders.Gui
{
    public class GuiPopup : GuiObject
    {
        static Texture2D Pixel;

        public Color Color = new Color(128, 128, 128, 128);
        public string[] Options;
        public SpriteFont Font;
        public Vector2 Position;

        public bool ShouldDispose = false;

        public int Width, Height, ObjectID = -1;

        public int SelectedItem
        {
            get
            {
                Vector2 relative = InputManager.CursorScreenPosition - Position;

                if (InputManager.IsPerformingAction(InputManager.Action.PrimaryTrigger, true))
                {
                    ShouldDispose = true;
                    if (!(relative.X < 0 || relative.Y < 0 || relative.X > Width || relative.Y > Height))
                    {
                        return (int)(relative.Y / 24);
                    }
                }

                return -1;
            }
        }

        public bool IsOver()
        {
            return InputManager.CursorScreenPosition.X >= Position.X && InputManager.CursorScreenPosition.Y >=
                Position.Y && InputManager.CursorScreenPosition.X <= Position.X + Width &&
                InputManager.CursorScreenPosition.Y <= Position.Y + Height;
        }

        static GuiPopup()
        {
            Pixel = new Texture2D(Main.Graphics.GraphicsDevice, 1, 1);
            Pixel.SetData<Color>(new Color[] { Color.White });
        }

        public override void Draw()
        {
            Height = Options.Length * 24;

            Main.SpriteBatch.Draw(Pixel, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color);

            for (int i = 0; i < Options.Length; i++)
            {
                if (Width < Font.MeasureString(Options[i]).X + 16)
                {
                    Width = (int)Font.MeasureString(Options[i]).X + 16;
                }

                Vector2 relative = InputManager.CursorScreenPosition - Position;
                int id = -1;

                if (!(relative.X < 0 || relative.Y < 0 || relative.X >= Width || relative.Y >= Height))
                {
                    id = (int)(relative.Y / 24);
                }

                Color color = Color.DarkGray;

                if (id == i)
                {
                    color = new Color(color.ToVector3() * 0.5f);
                }

                TextHelper.DrawShadowString(Font, Options[i], Position + new Vector2(8, (i * 24) + 4), color,
                    Color.Black);
            }
        }
    }
}
