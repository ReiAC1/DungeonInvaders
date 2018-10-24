using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Util.Entities
{
    public class ItemEntity : Entity
    {
        #region Fields

        public int ItemID, Amount = 1;
        public SoundEffect PickupEffect;

        #endregion

        #region Methods

        public ItemEntity(int itemID, int amount)
        {
            ItemID = itemID;
            Amount = amount;

            if (ItemID > 0 && ItemID < Items.Item.TypeCache.Count)
            {
                Texture = Items.Item.Get(itemID).Texture;
            }

            IsPhysical = false;

            PickupEffect = Main.ContentManager.Load<SoundEffect>("item-pickup");
        }

        public override void Update()
        {
            Rectangle Bounds = this.Bounds;
            Bounds.X -= 5;
            Bounds.Y -= 5;
            Bounds.Width += 10;
            Bounds.Height += 10;

            if (Bounds.Contains(InputManager.CursorPosition) &&
                InputManager.IsPerformingAction(InputManager.Action.PrimaryTrigger, true) &&
                Vector2.Distance(Position, Map.Current.Player.Position) <= Size.Length())
            {
                if (User.AddItem(ItemID, Amount))
                {
                    CanDispose = true;
                    Map.Current.Player.ShouldAttack = false;
                    PickupEffect.Play(Settings.SFXVolume, 0, 0);
                }
            }

            if (Texture != null)
            {
                Size = new Vector2(Texture.Width, Texture.Height);
            }
        }

        #endregion
    }
}
