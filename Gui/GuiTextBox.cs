using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DungeonInvaders.Util;

namespace DungeonInvaders.Gui
{
    public class GuiTextBox : GuiObject
    {
        #region Fields

        public string Text = "";

        public SpriteFont Font;
        public Texture2D Texture;

        public Vector2 TextOffset = new Vector2(8, 5);

        public Color TextColor;

        public int MaxText = 32;

        public bool OnlyNumbers = false;

        public bool Focused = true;

        Vector2 _Position;

        int WaitUntilDrawCursor, CursorIndex;
        bool DrawCursor = true;

        const int CursorMax = 25;

        #endregion

        #region Properties

        public Vector2 Position
        {
            get
            {
                return _Position + new Vector2(Texture.Width / 2, Texture.Height / 2);
            }

            set
            {
                _Position = value - new Vector2(Texture.Width / 2, Texture.Height / 2);
            }
        }

        #endregion

        #region Methods

        public GuiTextBox(String text, String font, String texture, Vector2 position, Color textColor)
        {
            Text = text;
            Font = Main.ContentManager.Load<SpriteFont>(font);
            Texture = Main.ContentManager.Load<Texture2D>(texture);
            TextColor = textColor;

            WaitUntilDrawCursor = CursorMax;

            CursorIndex = Text.Length;
        }

        public override void Update()
        {

            if (!Focused)
            {
                return;
            }

            CursorIndex = (int)Math.Min(CursorIndex, Text.Length);
            CursorIndex = (int)Math.Max(CursorIndex, 0);

            if (TextHelper.ProcessKey(Keys.Back) && CursorIndex > 0)
            {
                Text = Text.Remove(CursorIndex - 1, 1);
                CursorIndex--;
            }

            if (TextHelper.ProcessKey(Keys.Delete) && CursorIndex < Text.Length)
            {
                Text = Text.Remove(CursorIndex, 1);
            }

            if (TextHelper.ProcessKey(Keys.Left) && CursorIndex > 0)
            {
                CursorIndex--;
            }

            if (TextHelper.ProcessKey(Keys.Right) && CursorIndex < Text.Length)
            {
                CursorIndex++;
            }

            char chr = TextHelper.GetCharacter();

            if (OnlyNumbers && !(chr == '0' || chr == '1' || chr == '2' || chr == '3' || chr == '4' || chr == '5' || chr == '6' ||
                chr == '7' || chr == '8' || chr == '9'))
            {
                chr = (char)0;
            }

            if (chr != (char)0 && (Text.Length < MaxText || MaxText < 0))
            {
                Text = Text.Insert(CursorIndex, chr.ToString());
                CursorIndex++;
            }

            WaitUntilDrawCursor++;
            if (WaitUntilDrawCursor >= CursorMax)
            {
                DrawCursor = !DrawCursor;
                WaitUntilDrawCursor = 0;
            }
        }

        public bool IsPressed()
        {
            return IsOver() && InputManager.IsPerformingAction(InputManager.Action.PrimaryTrigger);
        }

        bool IsOver()
        {

            Vector2 state = InputManager.CursorScreenPosition;

            int width, height;

            Vector2 w = new Vector2(Texture.Width, Texture.Height) + (2 * TextOffset);
            width = (int)w.X;
            height = (int)w.Y;

            return state.X >= _Position.X && state.X <= _Position.X + width && state.Y >= _Position.Y &&
                state.Y <= _Position.Y + height;
        }

        public override void Draw()
        {
            Main.SpriteBatch.Draw(Texture, _Position, Color.White);

            TextHelper.DrawShadowString(Font, Text, _Position + TextOffset, TextColor, Color.Gray, 0);

            if (DrawCursor && Focused)
            {
                int size = (int)Font.MeasureString(" ").X;
                Main.SpriteBatch.DrawString(Font, "|", _Position + TextOffset + new Vector2((CursorIndex * size) - (size / 2), 0), Color.Black);
            }
        }

        #endregion
    }
}
