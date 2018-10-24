using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using DungeonInvaders.Gui;
using DungeonInvaders.Util;
using DungeonInvaders.Util.Entities;

namespace DungeonInvaders.States
{
    public abstract class GameState : State
    {
        #region Fields

        protected Map Map;

        protected bool MenuShown = false, PauseWithMenu = true;
        public bool GameOver = false, Quit = false;

        protected List<GuiObject> MenuGuiObjects = new List<GuiObject>();
        public GuiHud HUD;

        GuiButton MGReturn, MGQuit;

        int FramesBlack = 60;

        public static List<SoundEffect> AmbientNoises = new List<SoundEffect>();
        public static List<SoundEffectInstance> AmbientNoiseInstances = new List<SoundEffectInstance>();

        public static List<SoundEffect> AmbientBackgrounds = new List<SoundEffect>();
        public static List<SoundEffectInstance> AmbientBackgroundInstances = new List<SoundEffectInstance>();

        Random Random = new Random();

        #endregion

        #region Properties

        public static float LightLevel
        {
            get
            {
                return Lighting.LightManager.Lights[0].Range;
            }

            set
            {
                Lighting.LightManager.Lights[0].Range = value;
            }
        }

        public bool IsAmbientNoisePlaying
        {
            get
            {
                foreach(var effect in AmbientNoiseInstances)
                {
                    if (effect.State == SoundState.Playing)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsAmbientBackgroundPlaying
        {
            get
            {
                foreach (var effect in AmbientBackgroundInstances)
                {
                    if (effect.State == SoundState.Playing)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion

        #region Methods

        public override void Start()
        {
            FramesBlack = GraphicsDevice.DisplayMode.RefreshRate;
            Camera.Scale = Vector2.One * 2;
            if (AmbientNoises.Count == 0)
            {
                AmbientNoises.Add(Content.Load<SoundEffect>("ambience-1"));
                AmbientNoises.Add(Content.Load<SoundEffect>("ambience-2"));
                AmbientNoises.Add(Content.Load<SoundEffect>("ambience-3"));
                AmbientNoises.Add(Content.Load<SoundEffect>("ambience-4"));
                AmbientNoises.Add(Content.Load<SoundEffect>("ambience-5"));

                foreach(SoundEffect effect in AmbientNoises)
                {
                    AmbientNoiseInstances.Add(effect.CreateInstance());
                    AmbientNoiseInstances[AmbientNoiseInstances.Count - 1].Volume = Settings.SFXVolume;
                }

                AmbientBackgrounds.Add(Content.Load<SoundEffect>("bg01"));
                AmbientBackgrounds.Add(Content.Load<SoundEffect>("bg02"));
                AmbientBackgrounds.Add(Content.Load<SoundEffect>("bg03"));
                AmbientBackgrounds.Add(Content.Load<SoundEffect>("bg04"));
                AmbientBackgrounds.Add(Content.Load<SoundEffect>("bg05"));

                foreach (SoundEffect effect in AmbientBackgrounds)
                {
                    AmbientBackgroundInstances.Add(effect.CreateInstance());
                    AmbientBackgroundInstances[AmbientBackgroundInstances.Count - 1].Volume = Settings.MusicVolume;
                }
            }

            MainMenuState.MenuSoundInstance.Stop();

            Lighting.LightManager.Startup();

            User.StartDungeon();

            MGReturn = new GuiButton("Back to Game", "menu font", new Vector2(Main.Graphics.PreferredBackBufferWidth,
                Main.Graphics.PreferredBackBufferHeight / 4) / 2,
                Color.Black, new Vector2(5, 5), "button frame");

            MenuGuiObjects.Add(MGReturn);

            MGQuit = new GuiButton("Quit", "menu font", new Vector2(Main.Graphics.PreferredBackBufferWidth / 2,
                ((Main.Graphics.PreferredBackBufferHeight / 4) / 2) + 48),
                Color.Black, new Vector2(5, 5), "button frame");

            MenuGuiObjects.Add(MGQuit);

            Map = new Map(DungeonSettingsState.Difficulty, DungeonSettingsState.Area, DungeonSettingsState.Size);

            HUD = new GuiHud();
        }

        public override void Stop()
        {
            foreach(Room r in Map.Current.Rooms)
            {
                if (r == null) { continue; }

                for (int i = 0; i < r.Entities.Count; i++)
                {
                    var e = r.Entities[i];
                    e.OnDestroy(true);
                }
            }

            foreach(var v in AmbientNoiseInstances)
            {
                v.Stop();
            }

            foreach(var v in AmbientBackgroundInstances)
            {
                v.Stop();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (GameOver)
            {
                if (LightLevel > 0)
                {
                    float val = LightLevel - 1;
                    val = val < 0 ? 0 : val;
                    LightLevel = val;
                }
                else
                {
                    if (FramesBlack > 0)
                    {
                        FramesBlack--;
                    }
                    else
                    {
                        int gained = (int)((User.EXP - Map.StartEXP) * (DungeonSettingsState.Difficulty + 1 +
                            (DungeonSettingsState.Difficulty * 0.25f)));

                        if (Quit) { gained /= 2; }

                        if (Map.Current.Player.Health < 1 || Quit)
                        {
                            StateManager.CurrentState = new MainMenuState();
                        }
                        else
                        {
                            User.EXP += gained;

                            StateManager.CurrentState = new MainMenuState();
                        }

                        User.SaveSettings();
                    }
                }

                return;
            }
            else
            {
                if (!IsAmbientNoisePlaying && Random.Next(750) == 0)
                {
                    AmbientNoiseInstances[Random.Next(AmbientNoiseInstances.Count)].Play();
                }

                if (!IsAmbientBackgroundPlaying)
                {
                    AmbientBackgroundInstances[Random.Next(AmbientBackgrounds.Count)].Play();
                }
            }

            if (InputManager.IsPerformingAction(InputManager.Action.Pause))
            {
                MenuShown = !MenuShown;
            }

            if (MenuShown)
            {
                foreach (GuiObject obj in MenuGuiObjects)
                {
                    obj.Update();
                }

                if (MGReturn.IsPressed()) { MenuShown = false; }
                else if (MGQuit.IsPressed()) { GameOver = true; Quit = true; MenuShown = false; }

                if (MenuShown && PauseWithMenu) { return; }
            }
            else
            {
                HUD.Update();
            }

            Map.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Lighting.LightManager.Draw();

            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp,
                DepthStencilState.DepthRead, RasterizerState.CullCounterClockwise, Lighting.LightManager.LightEffect, Camera.View);

            Map.Draw();

            SpriteBatch.End();

            if (GameOver)
            {
                return;
            }

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (MenuShown)
            {

                foreach (GuiObject obj in MenuGuiObjects)
                {
                    obj.Draw();
                }
            }
            else
            {
                HUD.Draw();
            }

            SpriteBatch.End();
        }

        #endregion
    }
}
