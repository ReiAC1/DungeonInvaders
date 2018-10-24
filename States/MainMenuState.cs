using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using DungeonInvaders.Gui;
using DungeonInvaders.Util;

namespace DungeonInvaders.States
{
    public class MainMenuState : State
    {
        #region Fields

        GuiButton SinglePlayer, MultiPlayer, Options;

        GuiTitleBackground Background;

        public static SoundEffect MenuSound;
        public static SoundEffectInstance MenuSoundInstance;

        #endregion

        #region Methods

        public override void Start()
        {
            Camera.Scale = Vector2.One;

            if (MenuSound == null)
            {
                MenuSound = Content.Load<SoundEffect>("MenuSound");
                MenuSoundInstance = MenuSound.CreateInstance();
                MenuSoundInstance.Volume = Settings.MusicVolume;
            }

            Background = new GuiTitleBackground();

            SinglePlayer = new GuiButton("Solo Invasion", "Menu Font", new Vector2(Main.Graphics.PreferredBackBufferWidth,
                Main.Graphics.PreferredBackBufferHeight / 4) / 2, Color.Black, new Vector2(5, 5), "button frame");

            MultiPlayer = new GuiButton("Team Invasion", "Menu Font", new Vector2(Main.Graphics.PreferredBackBufferWidth,
                Main.Graphics.PreferredBackBufferHeight / 2) / 2, Color.Black, new Vector2(5, 5), "button frame");

            Options = new GuiButton("Options", "Menu Font", new Vector2(Main.Graphics.PreferredBackBufferWidth,
                Main.Graphics.PreferredBackBufferHeight / 1.35f) / 2, Color.Black, new Vector2(5, 5), "button frame");
        }

        public override void Update(GameTime gameTime)
        {
            Background.Update();

            if (SinglePlayer.IsPressed())
            {
                StateManager.CurrentState = new DungeonSettingsState(new SinglePlayerGameState());
            }
            else if (Options.IsPressed())
            {
                StateManager.CurrentState = new OptionsMenuState();
            }

            if (MenuSoundInstance.State == SoundState.Stopped)
            {
                MenuSoundInstance.Play();
                MenuSoundInstance.IsLooped = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();

            Background.Draw();

            SinglePlayer.Draw();
            MultiPlayer.Draw();
            Options.Draw();

            Util.InputManager.DrawCursor();

            SpriteBatch.End();
        }

        #endregion
    }
}
