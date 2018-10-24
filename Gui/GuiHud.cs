using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonInvaders.Gui.HUD;

namespace DungeonInvaders.Gui
{
    public class GuiHud : GuiObject
    {
        #region Fields

        public HudInventoryMenu InventoryMenu;
        public HudExpBar ExpBar;

        #endregion

        #region Methods

        public GuiHud()
        {
            InventoryMenu = new HudInventoryMenu(Main.ContentManager.Load<SpriteFont>("Menu font"),
                Main.ContentManager.Load<SpriteFont>("Menu font small"));
            ExpBar = new HudExpBar(Main.ContentManager.Load<SpriteFont>("Menu font small"));
            ExpBar.Frame = Main.ContentManager.Load<Texture2D>("exp frame");
            ExpBar.Bar = Main.ContentManager.Load<Texture2D>("exp bar");
        }

        public bool IsOver()
        {
            return InventoryMenu.IsOver();
        }

        public override void Update()
        {
            InventoryMenu.Update();
        }

        public override void Draw()
        {
            InventoryMenu.Draw();
            ExpBar.Draw();
        }

        #endregion
    }
}
