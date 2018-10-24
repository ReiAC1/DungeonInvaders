using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonInvaders.Gui;
using DungeonInvaders.Util;

namespace DungeonInvaders.States
{
    public class DungeonSettingsState : State
    {
        #region Fields

        GuiButton Back, StartDungeon;

        GuiButton Easy, Normal, Hard, Master;

        GuiButton Tiny, Small, Medium, Large, Huge, Massive;

        GuiButton AreaSelect;

        public static Map.Size Size = Map.Size.Medium;
        public static int Difficulty = 0;
        public static int Area = 0;

        string[] DifficultyString = new string[] { "Easy", "Normal", "Hard", "Master" };

        GuiTitleBackground Background;

        State NextState;

        #endregion

        #region Methods

        public DungeonSettingsState(State next)
        {
            NextState = next;
        }

        public override void Start()
        {
            Background = new GuiTitleBackground();

            Back = new GuiButton("Back", "menu font", new Vector2(32, 16), Color.Black, new Vector2(5, 5), "button frame");

            StartDungeon = new GuiButton("Invade Dungeon", "menu font",
                new Vector2(Main.Graphics.PreferredBackBufferWidth / 2, (Main.Graphics.PreferredBackBufferHeight / 4) * 3),
                Color.Black, new Vector2(5, 5), "button frame");

            Easy = new GuiButton("Easy", "menu font",
                new Vector2((Main.Graphics.PreferredBackBufferWidth / 4) * 1.25f, (Main.Graphics.PreferredBackBufferHeight / 4)),
                Color.Black, new Vector2(5, 5), "button frame");

            Normal = new GuiButton("Normal", "menu font",
                new Vector2((Main.Graphics.PreferredBackBufferWidth / 4) * 1.75f, (Main.Graphics.PreferredBackBufferHeight / 4)),
                Color.Black, new Vector2(5, 5), "button frame");

            Hard = new GuiButton("Hard", "menu font",
                new Vector2((Main.Graphics.PreferredBackBufferWidth / 4) * 2.25f, (Main.Graphics.PreferredBackBufferHeight / 4)),
                Color.Black, new Vector2(5, 5), "button frame");

            Master = new GuiButton("Master", "menu font",
                new Vector2((Main.Graphics.PreferredBackBufferWidth / 4) * 2.75f, (Main.Graphics.PreferredBackBufferHeight / 4)),
                Color.Black, new Vector2(5, 5), "button frame");

            Tiny = new GuiButton("Tiny", "menu font",
                new Vector2((Main.Graphics.PreferredBackBufferWidth / 4) * 0.75f, (Main.Graphics.PreferredBackBufferHeight / 4) * 2),
                Color.Black, new Vector2(5, 5), "button frame");

            Small = new GuiButton("Small", "menu font",
                new Vector2((Main.Graphics.PreferredBackBufferWidth / 4) * 1.25f, (Main.Graphics.PreferredBackBufferHeight / 4) * 2),
                Color.Black, new Vector2(5, 5), "button frame");

            Medium = new GuiButton("Medium", "menu font",
                new Vector2((Main.Graphics.PreferredBackBufferWidth / 4) * 1.75f, (Main.Graphics.PreferredBackBufferHeight / 4) * 2),
                Color.Black, new Vector2(5, 5), "button frame");

            Large = new GuiButton("Large", "menu font",
                new Vector2((Main.Graphics.PreferredBackBufferWidth / 4) * 2.25f, (Main.Graphics.PreferredBackBufferHeight / 4) * 2),
                Color.Black, new Vector2(5, 5), "button frame");

            Huge = new GuiButton("Huge", "menu font",
                new Vector2((Main.Graphics.PreferredBackBufferWidth / 4) * 2.75f, (Main.Graphics.PreferredBackBufferHeight / 4) * 2),
                Color.Black, new Vector2(5, 5), "button frame");

            Massive = new GuiButton("Massive", "menu font",
                new Vector2((Main.Graphics.PreferredBackBufferWidth / 4) * 3.25f, (Main.Graphics.PreferredBackBufferHeight / 4) * 2),
                Color.Black, new Vector2(5, 5), "button frame");
        }

        public override void Update(GameTime gameTime)
        {
            Background.Update();

            if (Back.IsPressed())
            {
                StateManager.CurrentState = new MainMenuState();
            }
            else if (StartDungeon.IsPressed())
            {
                StateManager.CurrentState = NextState;
            }
            else if (Easy.IsPressed())
            {
                Difficulty = 0;
            }
            else if (Normal.IsPressed())
            {
                Difficulty = 1;
            }
            else if (Hard.IsPressed())
            {
                Difficulty = 2;
            }
            else if (Master.IsPressed())
            {
                Difficulty = 3;
            }
            else if (Tiny.IsPressed())
            {
                Size = Map.Size.Tiny;
            }
            else if (Small.IsPressed())
            {
                Size = Map.Size.Small;
            }
            else if (Medium.IsPressed())
            {
                Size = Map.Size.Medium;
            }
            else if (Large.IsPressed())
            {
                Size = Map.Size.Large;
            }
            else if (Huge.IsPressed())
            {
                Size = Map.Size.Huge;
            }
            else if (Massive.IsPressed())
            {
                Size = Map.Size.Massive;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();

            Background.Draw();

            Back.Draw();
            StartDungeon.Draw();

            Easy.Draw();
            Normal.Draw();
            Hard.Draw();
            Master.Draw();

            Tiny.Draw();
            Small.Draw();
            Medium.Draw();
            Large.Draw();
            Huge.Draw();
            Massive.Draw();

            Color fore = Color.White;
            Color shadow = Color.Black;

            TextHelper.DrawShadowString(StartDungeon.Font, "Difficulty: " + DifficultyString[Difficulty],
                new Vector2(Main.Graphics.PreferredBackBufferWidth / 2, (Main.Graphics.PreferredBackBufferHeight / 4) * 1.5f),
                fore, shadow, 1);

            TextHelper.DrawShadowString(StartDungeon.Font, "Size: " + Size.ToString(),
                new Vector2(Main.Graphics.PreferredBackBufferWidth / 2, (Main.Graphics.PreferredBackBufferHeight / 4) * 2.5f),
                fore, shadow, 1);

            InputManager.DrawCursor();

            SpriteBatch.End();
        }

        #endregion
    }
}
