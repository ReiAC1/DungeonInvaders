using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonInvaders.Util
{
    public static class TextHelper
    {
        #region Fields

        static int RepeatRate = 15;
        static int[] RepeatTime = new int[256];
        static bool[] RepeatUpdate = new bool[256];

        #endregion

        #region Properties

        public static KeyboardState PreviousState
        {
            get
            {
                return InputManager.PreviousKeyState;
            }
        }

        public static KeyboardState CurrentState
        {
            get
            {
                return InputManager.CurrentKeyState;
            }
        }

        #endregion

        #region Methods

        public static void UpdateStates()
        {
            for (int i = 0; i < RepeatUpdate.Length; i++)
            {

                if (RepeatUpdate[i])
                {
                    RepeatTime[i] += 1;
                }

                RepeatUpdate[i] = false;
            }
        }

        public static char GetCharacter()
        {
            foreach (Keys keyK in CurrentState.GetPressedKeys())
            {
                bool Caps = Console.CapsLock, Shift = CurrentState.IsKeyDown(Keys.LeftShift) || CurrentState.IsKeyDown(Keys.RightShift);

                if (!ProcessKey(keyK))
                    continue;
                char key = (char)0;
                switch (keyK)
                {
                    //Alphabet keys
                    case Keys.A: if (Shift || Caps && !(Shift && Caps)) { key = 'A'; } else { key = 'a'; } break;
                    case Keys.B: if (Shift || Caps && !(Shift && Caps)) { key = 'B'; } else { key = 'b'; } break;
                    case Keys.C: if (Shift || Caps && !(Shift && Caps)) { key = 'C'; } else { key = 'c'; } break;
                    case Keys.D: if (Shift || Caps && !(Shift && Caps)) { key = 'D'; } else { key = 'd'; } break;
                    case Keys.E: if (Shift || Caps && !(Shift && Caps)) { key = 'E'; } else { key = 'e'; } break;
                    case Keys.F: if (Shift || Caps && !(Shift && Caps)) { key = 'F'; } else { key = 'f'; } break;
                    case Keys.G: if (Shift || Caps && !(Shift && Caps)) { key = 'G'; } else { key = 'g'; } break;
                    case Keys.H: if (Shift || Caps && !(Shift && Caps)) { key = 'H'; } else { key = 'h'; } break;
                    case Keys.I: if (Shift || Caps && !(Shift && Caps)) { key = 'I'; } else { key = 'i'; } break;
                    case Keys.J: if (Shift || Caps && !(Shift && Caps)) { key = 'J'; } else { key = 'j'; } break;
                    case Keys.K: if (Shift || Caps && !(Shift && Caps)) { key = 'K'; } else { key = 'k'; } break;
                    case Keys.L: if (Shift || Caps && !(Shift && Caps)) { key = 'L'; } else { key = 'l'; } break;
                    case Keys.M: if (Shift || Caps && !(Shift && Caps)) { key = 'M'; } else { key = 'm'; } break;
                    case Keys.N: if (Shift || Caps && !(Shift && Caps)) { key = 'N'; } else { key = 'n'; } break;
                    case Keys.O: if (Shift || Caps && !(Shift && Caps)) { key = 'O'; } else { key = 'o'; } break;
                    case Keys.P: if (Shift || Caps && !(Shift && Caps)) { key = 'P'; } else { key = 'p'; } break;
                    case Keys.Q: if (Shift || Caps && !(Shift && Caps)) { key = 'Q'; } else { key = 'q'; } break;
                    case Keys.R: if (Shift || Caps && !(Shift && Caps)) { key = 'R'; } else { key = 'r'; } break;
                    case Keys.S: if (Shift || Caps && !(Shift && Caps)) { key = 'S'; } else { key = 's'; } break;
                    case Keys.T: if (Shift || Caps && !(Shift && Caps)) { key = 'T'; } else { key = 't'; } break;
                    case Keys.U: if (Shift || Caps && !(Shift && Caps)) { key = 'U'; } else { key = 'u'; } break;
                    case Keys.V: if (Shift || Caps && !(Shift && Caps)) { key = 'V'; } else { key = 'v'; } break;
                    case Keys.W: if (Shift || Caps && !(Shift && Caps)) { key = 'W'; } else { key = 'w'; } break;
                    case Keys.X: if (Shift || Caps && !(Shift && Caps)) { key = 'X'; } else { key = 'x'; } break;
                    case Keys.Y: if (Shift || Caps && !(Shift && Caps)) { key = 'Y'; } else { key = 'y'; } break;
                    case Keys.Z: if (Shift || Caps && !(Shift && Caps)) { key = 'Z'; } else { key = 'z'; } break;

                    //Decimal keys
                    case Keys.D0: if (Shift) { key = ')'; } else { key = '0'; } break;
                    case Keys.D1: if (Shift) { key = '!'; } else { key = '1'; } break;
                    case Keys.D2: if (Shift) { key = '@'; } else { key = '2'; } break;
                    case Keys.D3: if (Shift) { key = '#'; } else { key = '3'; } break;
                    case Keys.D4: if (Shift) { key = '$'; } else { key = '4'; } break;
                    case Keys.D5: if (Shift) { key = '%'; } else { key = '5'; } break;
                    case Keys.D6: if (Shift) { key = '^'; } else { key = '6'; } break;
                    case Keys.D7: if (Shift) { key = '&'; } else { key = '7'; } break;
                    case Keys.D8: if (Shift) { key = '*'; } else { key = '8'; } break;
                    case Keys.D9: if (Shift) { key = '('; } else { key = '9'; } break;

                    //Decimal numpad keys
                    case Keys.NumPad0: key = '0'; break;
                    case Keys.NumPad1: key = '1'; break;
                    case Keys.NumPad2: key = '2'; break;
                    case Keys.NumPad3: key = '3'; break;
                    case Keys.NumPad4: key = '4'; break;
                    case Keys.NumPad5: key = '5'; break;
                    case Keys.NumPad6: key = '6'; break;
                    case Keys.NumPad7: key = '7'; break;
                    case Keys.NumPad8: key = '8'; break;
                    case Keys.NumPad9: key = '9'; break;

                    //Special keys
                    case Keys.OemTilde: if (Shift) { key = '~'; } else { key = '`'; } break;
                    case Keys.OemSemicolon: if (Shift) { key = ':'; } else { key = ';'; } break;
                    case Keys.OemQuotes: if (Shift) { key = '"'; } else { key = '\''; } break;
                    case Keys.OemQuestion: if (Shift) { key = '?'; } else { key = '/'; } break;
                    case Keys.OemPlus: if (Shift) { key = '+'; } else { key = '='; } break;
                    case Keys.OemPipe: if (Shift) { key = '|'; } else { key = '\\'; } break;
                    case Keys.OemPeriod: if (Shift) { key = '>'; } else { key = '.'; } break;
                    case Keys.OemOpenBrackets: if (Shift) { key = '{'; } else { key = '['; } break;
                    case Keys.OemCloseBrackets: if (Shift) { key = '}'; } else { key = ']'; } break;
                    case Keys.OemMinus: if (Shift) { key = '_'; } else { key = '-'; } break;
                    case Keys.OemComma: if (Shift) { key = '<'; } else { key = ','; } break;
                    case Keys.Space: key = ' '; break;
                }

                return key;

            }

            return (char)0;

        }

        public static bool ProcessKey(Keys key)
        {
            if (CurrentState.IsKeyDown(key))
            {
                RepeatUpdate[(int)key] = true;
                if ((!PreviousState.IsKeyDown(key) || RepeatTime[(int)key] >= RepeatRate))
                {
                    RepeatTime[(int)key] = 0;
                    return true;
                }
            }
            return false;
        }

        public static void DrawShadowString(SpriteFont font, String str, Vector2 pos, Color color, Color shadow)
        {
            Main.SpriteBatch.DrawString(font, str, pos + new Vector2(0, 1), shadow);
            Main.SpriteBatch.DrawString(font, str, pos + new Vector2(1, 0), shadow);
            Main.SpriteBatch.DrawString(font, str, pos + new Vector2(1, 1), shadow);
            Main.SpriteBatch.DrawString(font, str, pos, color);
        }

        public static void DrawShadowString(SpriteFont font, String str, Vector2 pos, Color color, Color shadow, int align)
        {
            Vector2 origin = Vector2.Zero;

            if (align == 1)
            {
                origin = font.MeasureString(str) / 2;
            }
            else if (align == 2)
            {
                origin.X = font.MeasureString(str).X;
            }

            Main.SpriteBatch.DrawString(font, str, pos + new Vector2(0, 1), shadow, 0, origin, 1, SpriteEffects.None, 0);
            Main.SpriteBatch.DrawString(font, str, pos + new Vector2(1, 0), shadow, 0, origin, 1, SpriteEffects.None, 0);
            Main.SpriteBatch.DrawString(font, str, pos + new Vector2(1, 1), shadow, 0, origin, 1, SpriteEffects.None, 0);
            Main.SpriteBatch.DrawString(font, str, pos, color, 0, origin, 1, SpriteEffects.None, 0);
        }

        #endregion
    }
}
