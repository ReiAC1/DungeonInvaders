using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Util
{
    public static class Settings
    {
        static short _MusicVolume = 10000, _SFXVolume = 10000;

        static short _ScreenSize = 0;

        public static float MusicVolume
        {
            get
            {
                return _MusicVolume / 10000.0f;
            }

            set
            {
                _MusicVolume = (short)(value * 10000);

                States.MainMenuState.MenuSoundInstance.Volume = value;
            }
        }

        public static float SFXVolume
        {
            get
            {
                return _SFXVolume / 10000.0f;
            }

            set
            {
                _SFXVolume = (short)(value * 10000);

                foreach(var snd in States.GameState.AmbientNoiseInstances)
                {
                    snd.Volume = value;
                }
            }
        }

        public static short ScreenSize
        {
            get
            {
                return _ScreenSize;
            }

            set
            {
                //if (value == _ScreenSize) { return; }

                DisplayMode[] modes = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.ToArray();

                Console.WriteLine(value);

                DisplayMode mode = modes[value];

                int cnt = modes.Length;

                Console.WriteLine(modes.Length);

                while (mode.Width < 800 || mode.Height < 800 ||
                    (mode.Width == Main.Graphics.PreferredBackBufferWidth && mode.Height == Main.Graphics.PreferredBackBufferHeight))
                {
                    value = (short)((value + 1) % (modes.Length - 1));
                    
                    if (cnt <= 0) { throw new Exception("Cannot create a large enough screen"); }

                    mode = modes[value];

                    cnt--;
                }

                _ScreenSize = value;

                Main.Graphics.PreferredBackBufferWidth = mode.Width;
                Main.Graphics.PreferredBackBufferHeight = mode.Height;
                Main.Graphics.PreferredBackBufferFormat = mode.Format;

                Console.WriteLine(mode);

                Main.Graphics.ApplyChanges();
            }
        }

        public static bool Fullscreen
        {
            get { return Main.Graphics.IsFullScreen; }
            set { Main.Graphics.IsFullScreen = value; Main.Graphics.ApplyChanges(); }
        }

        static Settings()
        {
            if (!File.Exists(User.BaseFolder + "settings.dat"))
            {
                Save();
            }
            Load();
        }

        public static void Save()
        {

            try
            {
                StreamWriter writer = new StreamWriter(User.BaseFolder + "settings.dat");

                writer.Write((char)_MusicVolume);
                writer.Write((char)_SFXVolume);
                writer.Write((char)_ScreenSize);
                writer.Write(Fullscreen ? 'T' : 'F');

                writer.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void Load()
        {
            StreamReader reader = new StreamReader(User.BaseFolder + "settings.dat");

            _MusicVolume = (short)reader.Read();
            _SFXVolume = (short)reader.Read();

            short s = (short)reader.Read();
            if (s <= -1) { s = 0; }

            ScreenSize = s;
            Fullscreen = reader.Read() == 'T';

            reader.Close();
        }

    }
}
