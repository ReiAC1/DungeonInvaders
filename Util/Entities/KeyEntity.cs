using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Util.Entities
{
    public class KeyEntity : ItemEntity
    {

        #region Fields

        public static readonly Color[] KeyColors = new Color[]
        {
            Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Orange, Color.Purple, Color.Brown, Color.Gray,
            Color.Pink, Color.DarkGreen, Color.CornflowerBlue, Color.Cyan, Color.Black, Color.White, Color.Teal,
            Color.SteelBlue, Color.PaleVioletRed, Color.PaleGoldenrod, Color.Gold, Color.Plum, Color.Violet,
            Color.Turquoise, Color.OrangeRed, Color.Olive, Color.Navy, Color.Beige, Color.DarkMagenta, Color.DarkRed,
            Color.DarkSeaGreen
        };

        #endregion

        #region Methods

        public KeyEntity(int keyID)
            : base(keyID, 0)
        {
            ItemID = keyID + 1000000;
            
            if (keyID < KeyColors.Length)
            {
                DrawColor = KeyColors[keyID];
            }

            Texture = Main.ContentManager.Load<Texture2D>("Key");
        }

        #endregion
    }
}
