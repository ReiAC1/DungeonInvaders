using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using DungeonInvaders.Util;

namespace DungeonInvaders.Gui
{
    public class GuiSlider : GuiObject
    {

        #region Fields

        static Texture2D PixelTexture;

        Texture2D ButtonTexture;

        public Vector2 Position;
        public SpriteFont Font;
        public String Text;

        public int Length;
        public float Value;

        public static SoundEffect HoverSound;

        bool PreviousPressed = false;

        #endregion

        #region Methods

        public GuiSlider(Vector2 position, int length, float value, string text, string font, string texture)
        {

            if (PixelTexture == null)
            {
                PixelTexture = new Texture2D(Main.Graphics.GraphicsDevice, 1, 1);
                PixelTexture.SetData<Color>(
                    new Color[] { Color.White });// fill the texture with white
            }

            Position = position;
            Length = length;
            Value = value;
            Text = text;
            Font = Main.ContentManager.Load<SpriteFont>(font);
            ButtonTexture = Main.ContentManager.Load<Texture2D>(texture);
        }

        public override void Update()
        {
            Value = (float)Math.Max(Value, 0);
            Value = (float)Math.Min(Value, 1 - (ButtonTexture.Width / Length));

            if (IsPressed())
            {
                Value = (float)Math.Abs((InputManager.CurrentMouseState.X) - Position.X) / (Length - (ButtonTexture.Width / Length));
            }

            if (PreviousPressed && !IsPressed())
            {
                if (HoverSound == null)
                {
                    HoverSound = Main.ContentManager.Load<SoundEffect>("interface2");
                }

                HoverSound.Play(Settings.SFXVolume, 0, 0);
            }

            PreviousPressed = IsPressed();
        }

        public bool IsPressed()
        {
            return InputManager.IsPerformingAction(InputManager.Action.PrimaryTrigger) && IsOver();
        }

        bool IsOver()
        {
            return InputManager.CursorScreenPosition.X >= Position.X &&
                InputManager.CursorScreenPosition.X <= Position.X + Length &&
                InputManager.CursorScreenPosition.Y >= Position.Y - (ButtonTexture.Height) &&
                InputManager.CursorScreenPosition.Y <= Position.Y + (ButtonTexture.Height);
        }

        bool IsOverPrevious()
        {
            return InputManager.PreviousCursorScreenPosition.X >= Position.X &&
                InputManager.PreviousCursorScreenPosition.X <= Position.X + Length &&
                InputManager.PreviousCursorScreenPosition.Y >= Position.Y - (ButtonTexture.Height) &&
                InputManager.PreviousCursorScreenPosition.Y <= Position.Y + (ButtonTexture.Height);
        }

        public override void Draw()
        {
            Vector2 sliderPos = Position + new Vector2(Length * Value, 0);
            sliderPos.X -= ButtonTexture.Width / 2;
            sliderPos.Y -= ButtonTexture.Height / 2;

            DrawLine(Position, new Vector2(Position.X + Length, Position.Y), 3, Color.Black);

            TextHelper.DrawShadowString(Font, Text, Position + new Vector2(Length / 2, -24), Color.DarkGray, Color.Black, 1);

            Main.SpriteBatch.Draw(ButtonTexture, sliderPos, Color.White);
        }

        void DrawLine(Vector2 start, Vector2 end, int scale, Color color)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            Main.SpriteBatch.Draw(PixelTexture,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1 * scale), //width of line, change this to make thicker line
                null,
                color, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }

        #endregion

    }
}
