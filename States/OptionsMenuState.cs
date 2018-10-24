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
    public class OptionsMenuState : State
    {
        #region Fields

        GuiTitleBackground Background;

        GuiSlider MusicVolume, SFXVolume;

        GuiButton Fullscreen, Resolution, Cancel, Accept;

        float MusicVolumeVal, SFXVolumeVal;

        bool FullscreenVal;
        short ResolutionVal;

        #endregion

        #region Methods

        public OptionsMenuState()
        {
            MusicVolumeVal = Settings.MusicVolume;
            SFXVolumeVal = Settings.SFXVolume;
            FullscreenVal = Settings.Fullscreen;
            ResolutionVal = Settings.ScreenSize;
        }

        public override void Start()
        {
            Background = new GuiTitleBackground();

            MusicVolume = new GuiSlider(new Vector2((GraphicsDevice.Viewport.Width / 2) - 64, 64), 128, Settings.MusicVolume, "Music Volume",
                "menu font", "Slider");
            SFXVolume = new GuiSlider(new Vector2((GraphicsDevice.Viewport.Width / 2) - 64, 128), 128, Settings.SFXVolume, "SFX Volume",
                "menu font", "Slider");

            Fullscreen = new GuiButton("Toggle Fullscreen", "Menu Font",
                new Vector2((GraphicsDevice.Viewport.Width / 2), 180), Color.Black, new Vector2(5, 5), "button frame");

            Cancel = new GuiButton("Cancel", "Menu Font",
               new Vector2((GraphicsDevice.Viewport.Width / 2) - 48, 308), Color.Black, new Vector2(5, 5), "button frame");

            Accept = new GuiButton("Accept", "Menu Font",
               new Vector2((GraphicsDevice.Viewport.Width / 2) + 48, 308), Color.Black, new Vector2(5, 5), "button frame");

            CreateResolutionButton();
        }

        public override void Update(GameTime gameTime)
        {
            Background.Update();
            MusicVolume.Update();
            SFXVolume.Update();

            //if (Fullscreen.IsPressed())
            //{
            //    Settings.Fullscreen = !Settings.Fullscreen;
            //}

            //if (Resolution.IsPressed())
            //{
            //    Settings.ScreenSize = (short)((Settings.ScreenSize + 1) %
            //        GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.ToArray().Length);
            //    Start();
            //}

            if (Accept.IsPressed())
            {
                StateManager.CurrentState = new MainMenuState();
                Settings.Save();

                return;
            }
            else if (Cancel.IsPressed())
            {
                Settings.MusicVolume = MusicVolumeVal;
                Settings.SFXVolume = SFXVolumeVal;
                Settings.ScreenSize = ResolutionVal;
                Settings.Fullscreen = FullscreenVal;

                StateManager.CurrentState = new MainMenuState();

                return;
            }

            Settings.MusicVolume = MusicVolume.Value;
            Settings.SFXVolume = SFXVolume.Value;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();

            Background.Draw();
            MusicVolume.Draw();
            SFXVolume.Draw();
            //Fullscreen.Draw();
            //Resolution.Draw();
            Cancel.Draw();
            Accept.Draw();

            InputManager.DrawCursor();

            SpriteBatch.End();
        }

        void CreateResolutionButton()
        {
            Resolution = new GuiButton(String.Format("Resolution: {0}x{1}", Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight),
                "Menu Font", new Vector2((GraphicsDevice.Viewport.Width / 2), 244), Color.Black, new Vector2(5, 5), "button frame");
        }

        #endregion
    }
}
