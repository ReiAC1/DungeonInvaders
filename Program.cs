#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace DungeonInvaders
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        public static bool IsExited = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (!Steamworks.SteamAPI.Init())
            {
                //return;
            }

            using (var game = new Main())
                game.Run();

            IsExited = true;

            Steamworks.SteamAPI.Shutdown();
        }
    }
#endif
}
