using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Util
{
    public static class InputManager
    {

        #region Enums

        public enum Movement
        {
            Straight,
            Sideways
        }

        public enum Action
        {
            Pause,
            PrimaryTrigger,
            SecondaryTrigger,
            HotKeyForward,
            HotKeyBackward,
            InversePrimarySecondary
        }

        #endregion

        #region Fields

        public static Color CursorColor = Color.Red;

#if WINDOWS || LINUX || OSX
        public static MouseState CurrentMouseState, PreviousMouseState;

        public const int KeyMapMoveLeft = 0, KeyMapMoveRight = 1, KeyMapMoveUp = 2, KeyMapMoveDown = 3, KeyMapPause = 4;

        public static Keys[] KeyMap = new Keys[]
        {
            Keys.A,
            Keys.D,
            Keys.W,
            Keys.S,
            Keys.Escape,
        };
#else
        static Vector2 CursorPos = Vector2.Zero, PreviousCursorPos = Vector2.Zero;
#endif

        public static KeyboardState CurrentKeyState, PreviousKeyState;

        public static GamePadState CurrentPadState, PreviousPadState;

        static Texture2D Texture = null;

        #endregion

        #region Properties

        public static Vector2 CursorPosition
        {
            get
            {
#if WINDOWS || LINUX || OSX
                return Vector2.Transform(new Vector2(CurrentMouseState.X, CurrentMouseState.Y), Matrix.Invert(Camera.View));
#else
                return Vector2.Transform(CursorPos, Matrix.Invert(Camera.View));
#endif
            }
        }

        public static Vector2 CursorScreenPosition
        {
            get
            {
#if WINDOWS || LINUX || OSX
                return new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
#else
                return CursorPos;
#endif
            }
        }

        public static Vector2 PreviousCursorPosition
        {
            get
            {
#if WINDOWS || LINUX || OSX
                return Vector2.Transform(new Vector2(PreviousMouseState.X, PreviousMouseState.Y), Matrix.Invert(Camera.View));
#else
                return Vector2.Transform(PreviousCursorPos, Matrix.Invert(Camera.View));
#endif
            }
        }

        public static Vector2 PreviousCursorScreenPosition
        {
            get
            {
#if WINDOWS || LINUX || OSX
                return new Vector2(PreviousMouseState.X, PreviousMouseState.Y);
#else
                return PreviousCursorPos;
#endif
            }
        }


        #endregion

        #region Methods

        public static void Update()
        {
#if WINDOWS || LINUX || OSX
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
#else
            PreviousCursorPos = CursorPos;
            CursorPos += new Vector2(CurrentPadState.ThumbSticks.Right.X, CurrentPadState.ThumbSticks.Right.Y) * 2;
#endif

            PreviousKeyState = CurrentKeyState;
            CurrentKeyState = Keyboard.GetState();

            PreviousPadState = CurrentPadState;
            CurrentPadState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
        }

        public static int IsMoving(Movement movement)
        {

            if (movement == Movement.Straight)
            {
#if WINDOWS || LINUX || OSX
                int speed = 0;
                if (CurrentKeyState.IsKeyDown(KeyMap[KeyMapMoveUp]))
                {
                    speed -= 1;
                }

                if (CurrentKeyState.IsKeyDown(KeyMap[KeyMapMoveDown]))
                {
                    speed += 1;
                }

                if (speed != 0) { return speed; }
#endif

                return (int)Math.Round(CurrentPadState.ThumbSticks.Left.Y);

            }
            else if (movement == Movement.Sideways)
            {
#if WINDOWS || LINUX || OSX
                int speed = 0;
                if (CurrentKeyState.IsKeyDown(KeyMap[KeyMapMoveLeft]))
                {
                    speed -= 1;
                }

                if (CurrentKeyState.IsKeyDown(KeyMap[KeyMapMoveRight]))
                {
                    speed += 1;
                }

                if (speed != 0) { return speed; }
#endif

                return (int)Math.Round(CurrentPadState.ThumbSticks.Left.X);
            }

            return 0;
        }

        public static bool IsPerformingAction(Action action, bool onPress = false)
        {
            if (action == Action.Pause)
            {
#if WINDOWS || LINUX || OSX
                if (CurrentKeyState.IsKeyDown(KeyMap[KeyMapPause]) && PreviousKeyState.IsKeyUp(KeyMap[KeyMapPause]))
                { return true; }
#endif
                return CurrentPadState.Buttons.Start == ButtonState.Pressed && PreviousPadState.Buttons.Start == ButtonState.Released;
            }
            else if (action == Action.PrimaryTrigger)
            {
#if WINDOWS || LINUX || OSX
                if (CurrentMouseState.LeftButton == ButtonState.Pressed &&
                    (PreviousMouseState.LeftButton == ButtonState.Released || !onPress))
                { return true; }
#endif
                return CurrentPadState.Triggers.Right > 0.5f && (PreviousPadState.Triggers.Right < 0.5f || !onPress);
            }
            else if (action == Action.SecondaryTrigger)
            {
#if WINDOWS || LINUX || OSX
                if (CurrentMouseState.RightButton == ButtonState.Pressed &&
                    (PreviousMouseState.RightButton == ButtonState.Released || !onPress))
                { return true; }
#endif
                return CurrentPadState.Triggers.Left > 0.5f && (PreviousPadState.Triggers.Left < 0.5f || !onPress);
            }
            else if (action == Action.HotKeyForward)
            {
#if WINDOWS || LINUX || OSX
                if (CurrentMouseState.ScrollWheelValue < PreviousMouseState.ScrollWheelValue)
                { return true; }
#endif
                return CurrentPadState.Buttons.RightShoulder == ButtonState.Pressed;
            }
            else if (action == Action.HotKeyBackward)
            {
#if WINDOWS || LINUX || OSX
                if (CurrentMouseState.ScrollWheelValue > PreviousMouseState.ScrollWheelValue)
                { return true; }
#endif
                return CurrentPadState.Buttons.LeftShoulder == ButtonState.Pressed;
            }
            else if (action == Action.InversePrimarySecondary)
            {
#if WINDOWS || LINUX || OSX
                if (CurrentKeyState.IsKeyDown(Keys.LeftShift) || CurrentKeyState.IsKeyDown(Keys.RightShift))
                { return true; }
#endif
                return CurrentPadState.Buttons.RightStick == ButtonState.Pressed || CurrentPadState.Buttons.LeftStick == ButtonState.Pressed;
            }

            return false;
        }

        public static void DrawCursor()
        {

            if (Texture == null)
            {
                Texture = Main.ContentManager.Load<Texture2D>("cursor");
            }

            Color color = CursorColor;

            if (IsPerformingAction(Action.PrimaryTrigger) || IsPerformingAction(Action.SecondaryTrigger))
            {
                color = new Color(color.ToVector3() / 2);
            }

            Main.SpriteBatch.Draw(Texture, CursorPosition, null, color,
                0, new Vector2(Texture.Width / 2, Texture.Height / 2), 1, SpriteEffects.None, 0);
        }

        #endregion
    }
}
