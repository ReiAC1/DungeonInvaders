using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DungeonInvaders.Util;

namespace DungeonInvaders.Gui
{
    public class GuiButton : GuiObject
    {

        #region Fields

        public String Text;
        public Color TextColor = Color.Black;

        public Texture2D Texture;
        public Vector2 TextOffset;
        public SpriteFont Font;

        Vector2 _Position;

        public static SoundEffect HoverSound;

        #endregion

        #region Properties

        public Vector2 Position
        {
            get
            {
                return _Position + (Font.MeasureString(Text) / 2);
            }

            set
            {
                _Position = value - (Font.MeasureString(Text) / 2);
            }
        }

        #endregion

        #region Methods

        public GuiButton(String text, String font, Vector2 position, Color textColor, Vector2 textOffset, String texture = null)
        {
            Text = text;
            Font = Main.ContentManager.Load<SpriteFont>(font);
            Position = position;
            TextColor = textColor;
            TextOffset = textOffset;
            if (texture != null) { Texture = Main.ContentManager.Load<Texture2D>(texture); }

        }

        public bool IsPressed()
        {
            return IsOver() && ((InputManager.CurrentMouseState.LeftButton == ButtonState.Released &&
                InputManager.PreviousMouseState.LeftButton == ButtonState.Pressed) ||
                InputManager.CurrentPadState.Buttons.A == ButtonState.Pressed);
        }

        bool IsOver()
        {
            Vector2 state = InputManager.CursorScreenPosition;

            int width, height;

            if (Texture != null)
            {
                Vector2 w = Font.MeasureString(Text) + (2 * TextOffset);
                width = (int)w.X;
                height = (int)w.Y;
            }
            else
            {
                Vector2 vec = Font.MeasureString(Text);
                width = (int)vec.X;
                height = (int)vec.Y;
            }

            return state.X >= _Position.X && state.X <= _Position.X + width && state.Y >= _Position.Y &&
                state.Y <= _Position.Y + height;
        }

        bool IsOverPrevious()
        {
            Vector2 state = InputManager.PreviousCursorScreenPosition;

            int width, height;

            if (Texture != null)
            {
                Vector2 w = Font.MeasureString(Text) + (2 * TextOffset);
                width = (int)w.X;
                height = (int)w.Y;
            }
            else
            {
                Vector2 vec = Font.MeasureString(Text);
                width = (int)vec.X;
                height = (int)vec.Y;
            }

            return state.X >= _Position.X && state.X <= _Position.X + width && state.Y >= _Position.Y &&
                state.Y <= _Position.Y + height;
        }

        public override void Draw()
        {
            if (IsOver() && !IsOverPrevious())
            {
                if (HoverSound == null)
                {
                    HoverSound = Main.ContentManager.Load<SoundEffect>("interface2");
                }

                HoverSound.Play(Settings.SFXVolume, 0, 0);
            }

            bool over = IsOver();

            if (Texture != null)
            {
                Vector2 w = Font.MeasureString(Text) + (2 * TextOffset);
                w.X += TextOffset.X * 2;

                Rectangle rect = new Rectangle((int)(_Position.X - TextOffset.X), (int)_Position.Y, (int)w.X, (int)w.Y);
                Main.SpriteBatch.Draw(Texture, rect, null, (over ? Color.Gray : Color.White));
            }


            Color color = TextColor;
            Color shadow = Color.Gray;

            if (over)
            {
                color = new Color(color.ToVector3() / 2);
                shadow = new Color(shadow.ToVector3() / 2);
            }

            TextHelper.DrawShadowString(Font, Text, _Position + TextOffset, color, shadow, 0);
        }

        #endregion
    }
}
