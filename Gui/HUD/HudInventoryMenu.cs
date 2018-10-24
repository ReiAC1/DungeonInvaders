using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using DungeonInvaders.Util;
using DungeonInvaders.Util.Entities;
using DungeonInvaders.Util.Items;

namespace DungeonInvaders.Gui.HUD
{
    public class HudInventoryMenu : GuiObject
    {
        #region Fields

        static Texture2D Pixel;

        Vector2 ItemPos,
                HelmPos,
                BodyPos,
                LegPos,
                WeaponPos,
                ArrowPos,
                SpellPos,
                NecklacePos,
                RingPos;

        public SpriteFont Font, SmallFont;

        Texture2D Texture, Items, MinMax, Arrow, Skills, EquipmentSlots, Gold;

        int Page = 0, TabID = 0;

        Vector2 Position = Vector2.Zero;

        Item Display;

        bool IsClosed = false, DrawLeftArrow = false, DrawRightArrow = true;

        GuiPopup Popup = null;

        SoundEffect PressSoundEffect, HoverSoundEffect;

        #endregion

        #region Properties

        int ItemsPerPage
        {
            get
            {
                return ((Texture.Height - 68) / 24) - 1;
            }
        }

        #endregion

        #region Methods

        static HudInventoryMenu()
        {
            Pixel = new Texture2D(Main.Graphics.GraphicsDevice, 1, 1);
            Pixel.SetData<Color>(new Color[]{ new Color(0, 0, 128, 128) });
        }

        public HudInventoryMenu(SpriteFont font, SpriteFont smallFont)
        {
            Texture = Main.ContentManager.Load<Texture2D>("ItemsMenu");
            Items = Main.ContentManager.Load<Texture2D>("ItemMenu_Items");
            MinMax = Main.ContentManager.Load<Texture2D>("ItemsMenu_MinMax");
            Arrow = Main.ContentManager.Load<Texture2D>("Arrow");
            Skills = Main.ContentManager.Load<Texture2D>("ItemMenu_Skills");
            EquipmentSlots = Main.ContentManager.Load<Texture2D>("ItemsMenu_EquipmentSlots");
            Gold = Main.ContentManager.Load<Texture2D>("Gold");
            Font = font;
            SmallFont = smallFont;
            PressSoundEffect = Main.ContentManager.Load<SoundEffect>("interface1");
            HoverSoundEffect = Main.ContentManager.Load<SoundEffect>("interface2");
        }

        public override void Update()
        {
            ItemPos = Position + new Vector2((Texture.Width / 2) - 32, 86);
            HelmPos = Position + new Vector2((Texture.Width / 2) - 32, 128);
            BodyPos = Position + new Vector2((Texture.Width / 2) - 32, 170);
            LegPos = Position + new Vector2((Texture.Width / 2) - 32, 212);
            WeaponPos = Position + new Vector2((Texture.Width / 2) - 74, 170);
            ArrowPos = Position + new Vector2((Texture.Width / 2) + 10, 170);
            SpellPos = Position + new Vector2((Texture.Width / 2) + 10, 128);
            NecklacePos = Position + new Vector2((Texture.Width / 2) - 74, 128);
            RingPos = Position + new Vector2((Texture.Width / 2) + 52, 170);

            if (!IsClosed)
            {
                Position.Y = Main.Graphics.PreferredBackBufferHeight - Texture.Height;
                Position.X = Main.Graphics.PreferredBackBufferWidth - 2 - Texture.Width;
            }

            int prevTabID = TabID;

            if (IsOver() && (Popup == null || !Popup.IsOver()))
            {
                Vector2 RelativePos = InputManager.CursorScreenPosition - Position;

                if (RelativePos.Y <= 32)
                {
                    if (((InputManager.CurrentMouseState.LeftButton == ButtonState.Released &&
                InputManager.PreviousMouseState.LeftButton == ButtonState.Pressed) ||
                InputManager.CurrentPadState.Buttons.A == ButtonState.Pressed))
                    {
                        PressSoundEffect.Play(Settings.SFXVolume, 0, 0);
                        TabID = (int)Math.Floor(RelativePos.X / 32);
                        Page = 0;

                        if ((Position.Y + Texture.Height > Main.Graphics.PreferredBackBufferHeight))
                        {
                            if (TabID == (int)Math.Floor(Texture.Width / 32.0f) - 1)
                            {
                                TabID = prevTabID;
                            }
                            Position.Y -= Texture.Height - 38;
                            IsClosed = false;
                        }
                    }
                }

                if (TabID == 0)
                {
                    int mouseItem = (int)Math.Floor((RelativePos.Y - 66) / 24.0f);

                    int id = mouseItem + (ItemsPerPage * Page);

                    if (id >= 0 && id < User.Items.GetLength(0) && User.Items[id, 1] > 0 &&
                        InputManager.IsPerformingAction(InputManager.Action.PrimaryTrigger, true))
                    {
                        Item item = Item.Get(User.Items[id, 0]);

                        if (item.ClassType == Item.Class.Weapon && item.RequiredLevel <= Map.Current.Player.Attack)
                        {
                            PlayerEntity.Weapon = (short)id;
                        }
                        else if (item.ClassType == Item.Class.Spell && item.RequiredLevel <= Map.Current.Player.Magic)
                        {
                            PlayerEntity.Spell = (short)id;
                        }
                        else if (item.ClassType == Item.Class.Ring)
                        {
                            PlayerEntity.Ring = (short)id;
                        }
                        else if (item.ClassType == Item.Class.Necklace)
                        {
                            PlayerEntity.Necklace = (short)id;
                        }
                        else if (item.ClassType == Item.Class.LegGear && item.RequiredLevel <= Map.Current.Player.Defense)
                        {
                            if ((item.SecondaryClass == Item.Class.Spell && item.SecondaryRequiredLevel <= Map.Current.Player.Magic) ||
                                (item.SecondaryClass == Item.Class.Arrow && item.SecondaryRequiredLevel <= Map.Current.Player.Archery) ||
                                (item.SecondaryClass != Item.Class.Arrow && item.SecondaryClass != Item.Class.Spell))
                            {
                                PlayerEntity.LegGear = (short)id;
                            }
                        }
                        else if (item.ClassType == Item.Class.HeadGear && item.RequiredLevel <= Map.Current.Player.Defense)
                        {
                            if ((item.SecondaryClass == Item.Class.Spell && item.SecondaryRequiredLevel <= Map.Current.Player.Magic) ||
                               (item.SecondaryClass == Item.Class.Arrow && item.SecondaryRequiredLevel <= Map.Current.Player.Archery) ||
                                (item.SecondaryClass != Item.Class.Arrow && item.SecondaryClass != Item.Class.Spell))
                            {
                                PlayerEntity.HeadGear = (short)id;
                            }
                        }
                        else if (item.ClassType == Item.Class.BodyGear && item.RequiredLevel <= Map.Current.Player.Defense)
                        {
                            if ((item.SecondaryClass == Item.Class.Spell && item.SecondaryRequiredLevel <= Map.Current.Player.Magic) ||
                               (item.SecondaryClass == Item.Class.Arrow && item.SecondaryRequiredLevel <= Map.Current.Player.Archery) ||
                                (item.SecondaryClass != Item.Class.Arrow && item.SecondaryClass != Item.Class.Spell))
                            {
                                PlayerEntity.BodyGear = (short)id;
                            }
                        }
                        else if (item.ClassType == Item.Class.Arrow && item.RequiredLevel <= Map.Current.Player.Archery)
                        {
                            PlayerEntity.Arrow = (short)id;
                        }
                        else if (item.ClassType == Item.Class.Item)
                        {
                            if (item.OnUse(Map.Current.Player))
                            {
                                User.Items[id, 1]--;
                            }
                        }
                    }
                    else if (id >= 0 && id < User.Items.GetLength(0) && User.Items[id, 1] > 0 &&
                        InputManager.IsPerformingAction(InputManager.Action.SecondaryTrigger, true))
                    {
                        Vector2 pos = InputManager.CursorScreenPosition;

                        if (pos.X + 48 >= Main.Graphics.PreferredBackBufferWidth)
                        {
                            pos.X -= 48;
                        }

                        if (pos.Y + 72 >= Main.Graphics.PreferredBackBufferHeight)
                        {
                            pos.Y -= 72;
                        }

                        Popup = new GuiPopup();
                        Popup.Position = pos;
                        Popup.Font = Font;
                        Popup.ObjectID = id;
                        Popup.Options = new string[]
                        {
                            "Use",
                            "Equip",
                            "Drop"
                        };
                    }
                }
                else if (TabID == 1)
                {
                    int mouseItem = (int)Math.Floor((RelativePos.Y - 66) / 24.0f);

                    int id = mouseItem + (ItemsPerPage * Page);

                    if (InputManager.IsPerformingAction(InputManager.Action.PrimaryTrigger, true))
                    {
                        if (id >= 0 && id < User.StatLevels.Length && User.AvailableStatPoints > 0)
                        {
                            User.StatLevels[id]++;
                        }
                    }
                    else if (InputManager.IsPerformingAction(InputManager.Action.SecondaryTrigger, true))
                    {
                        if (id >= 0 && id < User.StatLevels.Length && User.StatLevels[id] > 1)
                        {
                            User.StatLevels[id]--;
                        }
                    }
                }
                else if (TabID == 2 && InputManager.IsPerformingAction(InputManager.Action.SecondaryTrigger, true))
                {
                    int popupID = -1;
                    Vector2 pos = InputManager.CursorScreenPosition;

                    if (pos.X >= WeaponPos.X && pos.Y <= WeaponPos.X + 32 && pos.Y >= WeaponPos.Y && pos.Y <= WeaponPos.Y + 32 &&
                        pos.X <= WeaponPos.X + 32 && PlayerEntity.Weapon != -1 && User.Items[PlayerEntity.Weapon, 1] > 0)
                    {
                        popupID = 0;
                    }
                    else if (pos.X >= HelmPos.X && pos.Y <= HelmPos.X + 32 && pos.Y >= HelmPos.Y && pos.Y <= HelmPos.Y + 32 &&
                        pos.X <= HelmPos.X + 32 && PlayerEntity.HeadGear != -1 && User.Items[PlayerEntity.HeadGear, 1] > 0)
                    {
                        popupID = 1;
                    }
                    else if (pos.X >= BodyPos.X && pos.Y <= BodyPos.X + 32 && pos.Y >= BodyPos.Y && pos.Y <= BodyPos.Y + 32 &&
                        pos.X <= BodyPos.X + 32 && PlayerEntity.BodyGear != -1 && User.Items[PlayerEntity.BodyGear, 1] > 0)
                    {
                        popupID = 2;
                    }
                    else if (pos.X >= LegPos.X && pos.Y <= LegPos.X + 32 && pos.Y >= LegPos.Y && pos.Y <= LegPos.Y + 32 &&
                        pos.X <= LegPos.X + 32 && PlayerEntity.LegGear != -1 && User.Items[PlayerEntity.LegGear, 1] > 0)
                    {
                        popupID = 3;
                    }
                    else if (pos.X >= ArrowPos.X && pos.Y <= ArrowPos.X + 32 && pos.Y >= ArrowPos.Y && pos.Y <= ArrowPos.Y + 32 &&
                        pos.X <= ArrowPos.X + 32 && PlayerEntity.Arrow != -1 && User.Items[PlayerEntity.Arrow, 1] > 0)
                    {
                        popupID = 4;
                    }
                    else if (pos.X >= SpellPos.X && pos.Y <= SpellPos.X + 32 && pos.Y >= SpellPos.Y && pos.Y <= SpellPos.Y + 32 &&
                        pos.X <= SpellPos.X + 32 && PlayerEntity.Spell != -1 && User.Items[PlayerEntity.Spell, 1] > 0)
                    {
                        popupID = 5;
                    }
                    else if (pos.X >= NecklacePos.X && pos.Y <= NecklacePos.X + 32 && pos.Y >= NecklacePos.Y && pos.Y <= NecklacePos.Y + 32 &&
                        pos.X <= NecklacePos.X + 32 && PlayerEntity.Necklace != -1 && User.Items[PlayerEntity.Necklace, 1] > 0)
                    {
                        popupID = 6;
                    }
                    else if (pos.X >= RingPos.X && pos.Y <= RingPos.X + 32 && pos.Y >= RingPos.Y && pos.Y <= RingPos.Y + 32 &&
                        pos.X <= RingPos.X + 32 && PlayerEntity.Ring != -1 && User.Items[PlayerEntity.Ring, 1] > 0)
                    {
                        popupID = 7;
                    }
                    else if (pos.X >= ItemPos.X && pos.Y <= ItemPos.X + 32 && pos.Y >= ItemPos.Y && pos.Y <= ItemPos.Y + 32 &&
                        pos.X <= ItemPos.X + 32 && PlayerEntity.Item != -1 && User.Items[PlayerEntity.Item, 1] > 0)
                    {
                        popupID = 8;
                    }

                    if (popupID != -1)
                    {
                        Popup = new GuiPopup();
                        Popup.Position = pos;
                        Popup.Font = Font;
                        Popup.ObjectID = popupID;
                        Popup.Options = new string[]
                        {
                            "Remove",
                            "Drop"
                        };
                    }
                }
                else if (TabID == 100 && InputManager.IsPerformingAction(InputManager.Action.PrimaryTrigger, true))
                {
                    int mouseItem = (int)Math.Floor((RelativePos.Y - 66) / 24.0f);

                    int id = mouseItem + (ItemsPerPage * Page);

                    if (id >= 0 && id < FurnaceEntity.SmeltingRecipes.GetLength(0))
                    {
                        Smelt(id);
                    }
                }

                if (DrawRightArrow && InputManager.IsPerformingAction(InputManager.Action.PrimaryTrigger, true))
                {
                    if (RelativePos.X >= Texture.Width - 18 && RelativePos.Y >= Texture.Height - 24)
                    {
                        Page++;
                    }
                }

                if (DrawLeftArrow && InputManager.IsPerformingAction(InputManager.Action.PrimaryTrigger, true))
                {
                    if (RelativePos.X >= 8 && RelativePos.X <= 18 && RelativePos.Y >= Texture.Height - 24)
                    {
                        Page--;
                    }
                }

                if (TabID == (int)Math.Floor(Texture.Width / 32.0f) - 1)
                {
                    Position.Y += Texture.Height - 38;
                    TabID = prevTabID;
                    IsClosed = true;
                }
            }

            if (Popup != null)
            {
                int id = Popup.SelectedItem;

                if (TabID == 0)
                {
                    if (id == 0)
                    {
                        if (User.Items[Popup.ObjectID, 1] > 0)
                        {
                            Item item = Item.Get(User.Items[Popup.ObjectID, 0]);
                            if (item.OnUse(Map.Current.Player))
                            {
                                User.Items[Popup.ObjectID, 1]--;
                            }
                        }
                    }
                    else if (id == 1)
                    {
                        if (User.Items[Popup.ObjectID, 1] > 0)
                        {
                            Item item = Item.Get(User.Items[Popup.ObjectID, 0]);
                            id = Popup.ObjectID;

                            if (item.ClassType == Item.Class.Weapon && item.RequiredLevel <= Map.Current.Player.Attack)
                            {
                                PlayerEntity.Weapon = (short)id;
                            }
                            else if (item.ClassType == Item.Class.Spell && item.RequiredLevel <= Map.Current.Player.Magic)
                            {
                                PlayerEntity.Spell = (short)id;
                            }
                            else if (item.ClassType == Item.Class.Ring)
                            {
                                PlayerEntity.Ring = (short)id;
                            }
                            else if (item.ClassType == Item.Class.Necklace)
                            {
                                PlayerEntity.Necklace = (short)id;
                            }
                            else if (item.ClassType == Item.Class.LegGear && item.RequiredLevel <= Map.Current.Player.Defense)
                            {
                                if ((item.SecondaryClass == Item.Class.Spell && item.SecondaryRequiredLevel <= Map.Current.Player.Magic) ||
                                    (item.SecondaryClass == Item.Class.Arrow && item.SecondaryRequiredLevel <= Map.Current.Player.Archery) ||
                                    (item.SecondaryClass != Item.Class.Arrow && item.SecondaryClass != Item.Class.Spell))
                                {
                                    PlayerEntity.LegGear = (short)id;
                                }
                            }
                            else if (item.ClassType == Item.Class.HeadGear && item.RequiredLevel <= Map.Current.Player.Defense)
                            {
                                if ((item.SecondaryClass == Item.Class.Spell && item.SecondaryRequiredLevel <= Map.Current.Player.Magic) ||
                                   (item.SecondaryClass == Item.Class.Arrow && item.SecondaryRequiredLevel <= Map.Current.Player.Archery) ||
                                    (item.SecondaryClass != Item.Class.Arrow && item.SecondaryClass != Item.Class.Spell))
                                {
                                    PlayerEntity.HeadGear = (short)id;
                                }
                            }
                            else if (item.ClassType == Item.Class.BodyGear && item.RequiredLevel <= Map.Current.Player.Defense)
                            {
                                if ((item.SecondaryClass == Item.Class.Spell && item.SecondaryRequiredLevel <= Map.Current.Player.Magic) ||
                                   (item.SecondaryClass == Item.Class.Arrow && item.SecondaryRequiredLevel <= Map.Current.Player.Archery) ||
                                    (item.SecondaryClass != Item.Class.Arrow && item.SecondaryClass != Item.Class.Spell))
                                {
                                    PlayerEntity.BodyGear = (short)id;
                                }
                            }
                            else if (item.ClassType == Item.Class.Arrow && item.RequiredLevel <= Map.Current.Player.Archery)
                            {
                                PlayerEntity.Arrow = (short)id;
                            }
                            else if (item.ClassType == Item.Class.Item)
                            {
                                PlayerEntity.Item = (short)id;
                            }
                        }
                    }
                    else if (id == 2)
                    {
                        int amt = User.Items[Popup.ObjectID, 1];
                        if (amt > 0)
                        {
                            User.Items[Popup.ObjectID, 1] -= (short)amt;
                            Room room = Map.Current.GetRoomFromPosition(Map.Current.Player.Position);

                            Util.Entities.ItemEntity itm = new Util.Entities.ItemEntity(User.Items[Popup.ObjectID, 0], amt);
                            itm.Position = Map.Current.Player.Position;

                            room.Entities.Add(itm);
                        }
                    }
                }
                else if (TabID == 2)
                {
                    if (id == 0)
                    {
                        if (Popup.ObjectID == 0)
                        {
                            PlayerEntity.Weapon = -1;
                        }
                        else if (Popup.ObjectID == 1)
                        {
                            PlayerEntity.HeadGear = -1;
                        }
                        else if (Popup.ObjectID == 2)
                        {
                            PlayerEntity.BodyGear = -1;
                        }
                        else if (Popup.ObjectID == 3)
                        {
                            PlayerEntity.LegGear = -1;
                        }
                        else if (Popup.ObjectID == 4)
                        {
                            PlayerEntity.Arrow = -1;
                        }
                        else if (Popup.ObjectID == 5)
                        {
                            PlayerEntity.Spell = -1;
                        }
                        else if (Popup.ObjectID == 6)
                        {
                            PlayerEntity.Necklace = -1;
                        }
                        else if (Popup.ObjectID == 7)
                        {
                            PlayerEntity.Ring = -1;
                        }
                        else if (Popup.ObjectID == 8)
                        {
                            PlayerEntity.Item = -1;
                        }
                    }
                    else
                    {
                        Item item;

                        if (Popup.ObjectID == 0)
                        {
                            item = Item.Get(User.Items[PlayerEntity.Weapon, 0]);
                        }
                    }
                }

                if (Popup.ShouldDispose)
                {
                    Popup = null;
                }
            }
        }

        public void ShowSmeltingTab()
        {
            TabID = 100;
            IsClosed = false;

            PressSoundEffect.Play(Settings.SFXVolume, 0, 0);
        }

        public bool IsOver()
        {
            return InputManager.CursorScreenPosition.X >= Position.X && InputManager.CursorScreenPosition.Y >=
                Position.Y && InputManager.CursorScreenPosition.X <= Position.X + Texture.Width &&
                InputManager.CursorScreenPosition.Y <= Position.Y + Texture.Height;
        }

        public override void Draw()
        {
            Main.SpriteBatch.Draw(Texture, Position, Color.White);
            Main.SpriteBatch.Draw(Items, Position + new Vector2(6, 3), Color.White);
            Main.SpriteBatch.Draw(Skills, Position + new Vector2(34, 0), Color.White);
            Main.SpriteBatch.Draw(EquipmentSlots, Position + new Vector2(68, 1), new Rectangle(0, 0, 32, 32), Color.White);
            Main.SpriteBatch.Draw(Gold, Position + new Vector2((Texture.Width / 2) - 17, Texture.Height - 34), Color.White);

            string gold = "";
            Color color = Color.DarkBlue;

            if (User.Gold < 10000)
            {
                gold = User.Gold.ToString();
            }
            else if (User.Gold < 1000000)
            {
                string temp = (User.Gold / 1000.0f).ToString();
                if (temp.Length > 3) { temp = temp.Substring(0, 3); }
                gold = temp + "k";
                color = Color.White;
            }
            else
            {
                string temp = (User.Gold / 1000000.0f).ToString();
                if (temp.Length > 3) { temp = temp.Substring(0, 3); }
                gold = temp + "m";
                color = Color.DarkGreen;
            }

            TextHelper.DrawShadowString(SmallFont, gold,
                Position + new Vector2((Texture.Width / 2), Texture.Height - 16), color, Color.Black, 1);

            Rectangle source = new Rectangle(0, 0, 10, 10);
            source.X = (Position.Y + Texture.Height > Main.Graphics.PreferredBackBufferHeight) ? 10 : 0;

            Main.SpriteBatch.Draw(MinMax, Position + new Vector2(Texture.Width - 24, 14), source, Color.White);

            if (TabID == 0)
            {
                DrawItems();
            }
            else if (TabID == 1)
            {
                DrawStats();
            }
            else if (TabID == 2)
            {
                DrawEquipment();
            }
            else if (TabID == 100)
            {
                DrawSmelting();
            }

            if (DrawLeftArrow)
            {
                Main.SpriteBatch.Draw(Arrow, Position + new Vector2(8, Texture.Height - 24), Color.White);
            }

            if (DrawRightArrow)
            {
                Main.SpriteBatch.Draw(Arrow, Position + new Vector2(Texture.Width - 18, Texture.Height - 24), null, Color.White,
                    0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
            }

            if (Popup != null) { Popup.Draw(); }

            DrawLeftArrow = Page > 0;
        }

        void DrawItems()
        {
            int amt;

            DrawRightArrow = !(((Page + 1) * ItemsPerPage) >= User.Items.GetLength(0));

            for (int i = 0; i < ItemsPerPage; i++)
            {

                int id = i + (Page * ItemsPerPage);

                if (id >= User.Items.GetLength(0)) { break; }

                Item item = GetItem(i, out amt);

                string name = "Empty";

                if (item != null)
                {
                    name = item.Name;
                }

                TextHelper.DrawShadowString(Font, name, Position +
                    new Vector2(36, 68 + (i * 25)), Color.DarkGray, Color.Black);

                if (item != null)
                {
                    Main.SpriteBatch.Draw(item.Texture, Position + new Vector2(4, 66 + (i * 25)), Color.White);

                    TextHelper.DrawShadowString(SmallFont, amt.ToString(), Position +
                        new Vector2(34, 67 + (i * 25) + (item.Texture.Height / 2)), Color.DarkGray, Color.Black,
                        2);
                }

            }


            if (Popup != null && Popup.IsOver()) { return; }

            int mouseItem = (int)Math.Floor((InputManager.CursorScreenPosition.Y - Position.Y - 66) / 24.0f);
            int prevMouseItem = (int)Math.Floor((InputManager.PreviousCursorScreenPosition.Y - Position.Y - 66) / 24.0f);

            if (mouseItem != prevMouseItem && mouseItem >= 0 && prevMouseItem >= 0 && mouseItem < ItemsPerPage &&
                prevMouseItem < ItemsPerPage && IsOver())
            {
                HoverSoundEffect.Play(Settings.SFXVolume, 0, 0);
            }

            if (mouseItem < 0 || !IsOver() || mouseItem >= ItemsPerPage) { return; }

            Main.SpriteBatch.Draw(Pixel, new Rectangle((int)Position.X + 2, (int)Position.Y + 66 + (mouseItem * 25),
                Texture.Width - 4, 24), Color.White);

            Display = GetItem(mouseItem, out amt);

            if (Display != null)
            {
                string a, b;
                a = Display.Description;
                b = "";

                while (SmallFont.MeasureString(a).X > Texture.Width - 8)
                {
                    b = b.Insert(0, "" + a[a.Length - 1]);
                    a = a.Substring(0, a.Length - 1);
                }

                TextHelper.DrawShadowString(SmallFont, a, Position + new Vector2(4, 36), Color.DarkGray,
                    Color.Black);

                if (!String.IsNullOrEmpty(b))
                {
                    TextHelper.DrawShadowString(SmallFont, b, Position + new Vector2(4, 48), Color.DarkGray,
                    Color.Black);
                }
            }
        }

        void DrawStats()
        {
            int id;

            DrawRightArrow = !(((Page + 1) * ItemsPerPage) >= User.StatNames.GetLength(0));

            for (int i = 0; i < ItemsPerPage; i++)
            {

                id = i + (Page * ItemsPerPage);

                if (id < 0 || id >= User.StatNames.GetLength(0)) { continue; }

                TextHelper.DrawShadowString(Font, User.StatNames[id], Position + new Vector2(12, 68 + (i * 25)),
                    Color.DarkGray, Color.Black);

                TextHelper.DrawShadowString(Font, String.Format("Level: {0}", User.StatLevels[id]),
                    Position + new Vector2(Texture.Width - 34, 68 + (i * 25)), Color.DarkGray, Color.Black, 2);
            }

            string stats = String.Format("Stat Points: {0}", User.AvailableStatPoints);

            TextHelper.DrawShadowString(Font, stats, Position + new Vector2(4, 38), Color.DarkGray,
                    Color.Black);

            int mouseItem = (int)Math.Floor((InputManager.CursorScreenPosition.Y - Position.Y - 66) / 24.0f);
            int prevMouseItem = (int)Math.Floor((InputManager.PreviousCursorScreenPosition.Y - Position.Y - 66) / 24.0f);

            if (mouseItem != prevMouseItem && mouseItem >= 0 && prevMouseItem >= 0 && mouseItem < ItemsPerPage &&
                prevMouseItem < ItemsPerPage && IsOver())
            {
                HoverSoundEffect.Play(Settings.SFXVolume, 0, 0);
            }

            if (mouseItem < 0 || !IsOver() || mouseItem >= ItemsPerPage) { return; }

            Main.SpriteBatch.Draw(Pixel, new Rectangle((int)Position.X + 2, (int)Position.Y + 66 + (mouseItem * 25),
                Texture.Width - 4, 24), Color.White);
        }

        void DrawEquipment()
        {
            DrawRightArrow = false;

            Main.SpriteBatch.Draw(EquipmentSlots, ItemPos,
               new Rectangle(32, 0, 32, 32), Color.White);
            Main.SpriteBatch.Draw(EquipmentSlots, HelmPos,
                new Rectangle(32, 0, 32, 32), Color.White);
            Main.SpriteBatch.Draw(EquipmentSlots, BodyPos,
                new Rectangle(32, 0, 32, 32), Color.White);
            Main.SpriteBatch.Draw(EquipmentSlots, LegPos,
                new Rectangle(32, 0, 32, 32), Color.White);
            Main.SpriteBatch.Draw(EquipmentSlots, WeaponPos,
                new Rectangle(32, 0, 32, 32), Color.White);
            Main.SpriteBatch.Draw(EquipmentSlots, ArrowPos,
                new Rectangle(32, 0, 32, 32), Color.White);

            Main.SpriteBatch.Draw(EquipmentSlots, SpellPos,
                new Rectangle(32, 0, 32, 32), Color.White);
            Main.SpriteBatch.Draw(EquipmentSlots, NecklacePos,
                new Rectangle(32, 0, 32, 32), Color.White);

            Main.SpriteBatch.Draw(EquipmentSlots, RingPos,
                new Rectangle(32, 0, 32, 32), Color.White);

            if (PlayerEntity.Item >= 0 && User.Items[PlayerEntity.Item, 1] > 0)
            {
                Item item = Item.Get(User.Items[PlayerEntity.Item, 0]);

                Main.SpriteBatch.Draw(item.Texture, Position + new Vector2((Texture.Width / 2) - 28, 90), Color.White);
            }

            if (PlayerEntity.Weapon >= 0 && User.Items[PlayerEntity.Weapon, 1] > 0)
            {
                Item item = Item.Get(User.Items[PlayerEntity.Weapon, 0]);

                Main.SpriteBatch.Draw(item.Texture, Position + new Vector2((item.Texture.Width / 2) + 46, 174), Color.White);
            }

            if (PlayerEntity.Spell >= 0 && User.Items[PlayerEntity.Spell, 1] > 0)
            {
                Item item = Item.Get(User.Items[PlayerEntity.Spell, 0]);

                Main.SpriteBatch.Draw(item.Texture, Position + new Vector2((Texture.Width / 2) + 14, 132), Color.White);
            }

            if (PlayerEntity.Arrow >= 0 && User.Items[PlayerEntity.Arrow, 1] > 0)
            {
                Item item = Item.Get(User.Items[PlayerEntity.Arrow, 0]);

                Main.SpriteBatch.Draw(item.Texture, Position + new Vector2((Texture.Width / 2) + 14, 174), Color.White);
            }

            if (PlayerEntity.HeadGear >= 0 && User.Items[PlayerEntity.HeadGear, 1] > 0)
            {
                Item item = Item.Get(User.Items[PlayerEntity.HeadGear, 0]);

                Main.SpriteBatch.Draw(item.Texture, Position + new Vector2((Texture.Width / 2) - 28, 132), Color.White);
            }

            if (PlayerEntity.BodyGear >= 0 && User.Items[PlayerEntity.BodyGear, 1] > 0)
            {
                Item item = Item.Get(User.Items[PlayerEntity.BodyGear, 0]);

                Main.SpriteBatch.Draw(item.Texture, Position + new Vector2((Texture.Width / 2) - 28, 174), Color.White);
            }

            if (PlayerEntity.LegGear >= 0 && User.Items[PlayerEntity.LegGear, 1] > 0)
            {
                Item item = Item.Get(User.Items[PlayerEntity.LegGear, 0]);

                Main.SpriteBatch.Draw(item.Texture, Position + new Vector2((Texture.Width / 2) - 28, 216), Color.White);
            }

            if (PlayerEntity.Necklace >= 0 && User.Items[PlayerEntity.Necklace, 1] > 0)
            {
                Item item = Item.Get(User.Items[PlayerEntity.Necklace, 0]);

                Main.SpriteBatch.Draw(item.Texture, Position + new Vector2((item.Texture.Width / 2) + 46, 132), Color.White);
            }

            if (PlayerEntity.Ring >= 0 && User.Items[PlayerEntity.Ring, 1] > 0)
            {
                Item item = Item.Get(User.Items[PlayerEntity.Ring, 0]);

                Main.SpriteBatch.Draw(item.Texture, Position + new Vector2((Texture.Width / 2) + 56, 174), Color.White);
            }

            Vector2 pos = InputManager.CursorScreenPosition;
            string text = "";

            if (pos.X >= WeaponPos.X && pos.Y <= WeaponPos.X + 32 && pos.Y >= WeaponPos.Y && pos.Y <= WeaponPos.Y + 32 &&
                       pos.X <= WeaponPos.X + 32 && PlayerEntity.Weapon != -1 && User.Items[PlayerEntity.Weapon, 1] > 0)
            {
                text = Item.Get(User.Items[PlayerEntity.Weapon, 0]).Description;
            }
            else if (pos.X >= HelmPos.X && pos.Y <= HelmPos.X + 32 && pos.Y >= HelmPos.Y && pos.Y <= HelmPos.Y + 32 &&
                pos.X <= HelmPos.X + 32 && PlayerEntity.HeadGear != -1 && User.Items[PlayerEntity.HeadGear, 1] > 0)
            {
                text = Item.Get(User.Items[PlayerEntity.HeadGear, 0]).Description;
            }
            else if (pos.X >= BodyPos.X && pos.Y <= BodyPos.X + 32 && pos.Y >= BodyPos.Y && pos.Y <= BodyPos.Y + 32 &&
                pos.X <= BodyPos.X + 32 && PlayerEntity.BodyGear != -1 && User.Items[PlayerEntity.BodyGear, 1] > 0)
            {
                text = Item.Get(User.Items[PlayerEntity.BodyGear, 0]).Description;
            }
            else if (pos.X >= LegPos.X && pos.Y <= LegPos.X + 32 && pos.Y >= LegPos.Y && pos.Y <= LegPos.Y + 32 &&
                pos.X <= LegPos.X + 32 && PlayerEntity.LegGear != -1 && User.Items[PlayerEntity.LegGear, 1] > 0)
            {
                text = Item.Get(User.Items[PlayerEntity.LegGear, 0]).Description;
            }
            else if (pos.X >= ArrowPos.X && pos.Y <= ArrowPos.X + 32 && pos.Y >= ArrowPos.Y && pos.Y <= ArrowPos.Y + 32 &&
                pos.X <= ArrowPos.X + 32 && PlayerEntity.Arrow != -1 && User.Items[PlayerEntity.Arrow, 1] > 0)
            {
                text = Item.Get(User.Items[PlayerEntity.Arrow, 0]).Description;
            }
            else if (pos.X >= SpellPos.X && pos.Y <= SpellPos.X + 32 && pos.Y >= SpellPos.Y && pos.Y <= SpellPos.Y + 32 &&
                pos.X <= SpellPos.X + 32 && PlayerEntity.Spell != -1 && User.Items[PlayerEntity.Spell, 1] > 0)
            {
                text = Item.Get(User.Items[PlayerEntity.Spell, 0]).Description;
            }
            else if (pos.X >= NecklacePos.X && pos.Y <= NecklacePos.X + 32 && pos.Y >= NecklacePos.Y && pos.Y <= NecklacePos.Y + 32 &&
                pos.X <= NecklacePos.X + 32 && PlayerEntity.Necklace != -1 && User.Items[PlayerEntity.Necklace, 1] > 0)
            {
                text = Item.Get(User.Items[PlayerEntity.Necklace, 0]).Description;
            }
            else if (pos.X >= RingPos.X && pos.Y <= RingPos.X + 32 && pos.Y >= RingPos.Y && pos.Y <= RingPos.Y + 32 &&
                pos.X <= RingPos.X + 32 && PlayerEntity.Ring != -1 && User.Items[PlayerEntity.Ring, 1] > 0)
            {
                text = Item.Get(User.Items[PlayerEntity.Ring, 0]).Description;
            }
            else if (pos.X >= ItemPos.X && pos.Y <= ItemPos.X + 32 && pos.Y >= ItemPos.Y && pos.Y <= ItemPos.Y + 32 &&
                pos.X <= ItemPos.X + 32 && PlayerEntity.Item != -1 && User.Items[PlayerEntity.Item, 1] > 0)
            {
                text = Item.Get(User.Items[PlayerEntity.Item, 0]).Description;
            }

            if (text != "")
            {
                string a, b;
                a = text;
                b = "";

                while (SmallFont.MeasureString(a).X > Texture.Width - 8)
                {
                    b = b.Insert(0, "" + a[a.Length - 1]);
                    a = a.Substring(0, a.Length - 1);
                }

                TextHelper.DrawShadowString(SmallFont, a, Position + new Vector2(4, 36), Color.DarkGray,
                    Color.Black);

                if (!String.IsNullOrEmpty(b))
                {
                    TextHelper.DrawShadowString(SmallFont, b, Position + new Vector2(4, 48), Color.DarkGray,
                    Color.Black);
                }
            }
        }

        void DrawSmelting()
        {
            if (FurnaceEntity.Distance > 72)
            {
                TabID = 0;
                if (!IsClosed)
                {
                    PressSoundEffect.Play(Settings.SFXVolume, 0, 0);
                }
                return;
            }
            DrawRightArrow = !(((Page + 1) * ItemsPerPage) >= FurnaceEntity.SmeltingRecipes.GetLength(0));

            for (int i = 0; i < ItemsPerPage; i++)
            {

                int id = i + (Page * ItemsPerPage);

                if (id >= FurnaceEntity.SmeltingRecipes.GetLength(0)) { break; }

                Item item = Item.Get(GetSmeltingOutput(id));

                string name = "???";

                if (item != null)
                {
                    name = item.Name;
                }

                Color main = CanSmelt(id) ? Color.DarkGray : Color.Maroon;

                TextHelper.DrawShadowString(Font, name, Position +
                    new Vector2(36, 68 + (i * 25)), main, Color.Black);

                if (item != null && item.Texture != null)
                {
                    Main.SpriteBatch.Draw(item.Texture, Position + new Vector2(4, 66 + (i * 25)), Color.White);
                }
            }


            if (Popup != null && Popup.IsOver()) { return; }

            int mouseItem = (int)Math.Floor((InputManager.CursorScreenPosition.Y - Position.Y - 66) / 24.0f);
            int prevMouseItem = (int)Math.Floor((InputManager.PreviousCursorScreenPosition.Y - Position.Y - 66) / 24.0f);

            if (mouseItem != prevMouseItem && mouseItem >= 0 && prevMouseItem >= 0 && mouseItem < ItemsPerPage &&
                prevMouseItem < ItemsPerPage && IsOver() &&
                mouseItem + (Page * ItemsPerPage) < FurnaceEntity.SmeltingRecipes.GetLength(0))
            {
                HoverSoundEffect.Play(Settings.SFXVolume, 0, 0);
            }

            if (mouseItem < 0 || !IsOver() || mouseItem >= ItemsPerPage ||
                mouseItem + (Page * ItemsPerPage) >= FurnaceEntity.SmeltingRecipes.GetLength(0)) { return; }

            Main.SpriteBatch.Draw(Pixel, new Rectangle((int)Position.X + 2, (int)Position.Y + 66 + (mouseItem * 25),
                Texture.Width - 4, 24), Color.White);

            string a, b;
            a = SmeltInputToString(mouseItem + (Page * ItemsPerPage));
            b = "";

            if (a.Contains('\n'))
            {
                string[] t = a.Split('\n');
                a = t[0];
                b = t[1];
            }
            else
            {
                while (SmallFont.MeasureString(a).X > Texture.Width - 8)
                {
                    b = b.Insert(0, "" + a[a.Length - 1]);
                    a = a.Substring(0, a.Length - 1);
                }
            }

            TextHelper.DrawShadowString(SmallFont, a, Position + new Vector2(4, 36), Color.DarkGray,
                Color.Black);

            if (!String.IsNullOrEmpty(b))
            {
                TextHelper.DrawShadowString(SmallFont, b, Position + new Vector2(4, 48), Color.DarkGray,
                Color.Black);
            }
        }

        Item GetItem(int id, out int amt)
        {
            amt = 0;
            id += Page * ItemsPerPage;

            if (id >= User.Items.GetLength(0)) { return null; }

            if (User.Items[id, 1] < 1) { return null; }

            amt = User.Items[id, 1];

            return Item.Get(User.Items[id, 0]);
        }

        void Smelt(int recipeID)
        {
            if (recipeID < 0 || recipeID >= FurnaceEntity.SmeltingRecipes.GetLength(0)) { return; }

            if (Map.Current.Player.Smithing < FurnaceEntity.SmeltingLevels[recipeID])
            {
                return;
            }

            List<int> tempIDs = new List<int>();
            int output = -1;
            for (int i = 0; i < FurnaceEntity.SmeltingRecipes.GetLength(1) - 1; i += 2)
            {
                if (FurnaceEntity.SmeltingRecipes[recipeID, i + 1] == -1) { output = FurnaceEntity.SmeltingOutputs[FurnaceEntity.SmeltingRecipes[recipeID, i]]; break; }

                int oreID = OreRockEntity.Ores[FurnaceEntity.SmeltingRecipes[recipeID, i]];
                int amount = FurnaceEntity.SmeltingRecipes[recipeID, i + 1];

                for (int j = 0; j < User.Items.GetLength(0); j++)
                {
                    if (oreID == User.Items[j, 0])
                    {
                        if (User.Items[j, 1] < amount) { return; }
                        else { tempIDs.Add(j); continue; }
                    }

                    return;
                }
            }

            if (output == -1)
            {
                output = FurnaceEntity.SmeltingOutputs[FurnaceEntity.SmeltingRecipes[recipeID, FurnaceEntity.SmeltingRecipes.GetLength(1) - 1]];
            }

            User.AddItem(output, 1);

            for (int i = 0; i < tempIDs.Count; i++)
            {
                User.Items[tempIDs[i], 1] -= (short)FurnaceEntity.SmeltingRecipes[recipeID, (i * 2) + 1];
            }
        }

        bool CanSmelt(int recipeID)
        {
            if (recipeID < 0 || recipeID >= FurnaceEntity.SmeltingRecipes.GetLength(0)) { return false; }

            if (Map.Current.Player.Smithing < FurnaceEntity.SmeltingLevels[recipeID])
            {
                return false;
            }

            for (int i = 0; i < FurnaceEntity.SmeltingRecipes.GetLength(1) - 1; i += 2)
            {
                if (FurnaceEntity.SmeltingRecipes[recipeID, i + 1] == -1) { break; }

                int oreID = OreRockEntity.Ores[FurnaceEntity.SmeltingRecipes[recipeID, i]];
                int amount = FurnaceEntity.SmeltingRecipes[recipeID, i + 1];

                for (int j = 0; j < User.Items.GetLength(0); j++)
                {
                    if (oreID == User.Items[j, 0])
                    {
                        if (User.Items[j, 1] < amount) { return false; }
                    }

                    return false;
                }
            }

            return true;
        }

        int GetSmeltingOutput(int recipeID)
        {
            for (int i = 0; i < FurnaceEntity.SmeltingRecipes.GetLength(1); i++)
            {
                if (FurnaceEntity.SmeltingRecipes[recipeID, i] == -1)
                { return FurnaceEntity.SmeltingOutputs[FurnaceEntity.SmeltingRecipes[recipeID, i - 1]]; }
            }

            return FurnaceEntity.SmeltingOutputs[FurnaceEntity.SmeltingRecipes[recipeID, FurnaceEntity.SmeltingRecipes.GetLength(1) - 1]];
        }

        string SmeltInputToString(int recipeID)
        {
            string str = "";

            for (int i = 0; i < FurnaceEntity.SmeltingRecipes.GetLength(1) - 1; i += 2)
            {
                if (FurnaceEntity.SmeltingRecipes[recipeID, i + 1] == -1) { break; }

                int oreID = OreRockEntity.Ores[FurnaceEntity.SmeltingRecipes[recipeID, i]];
                int amount = FurnaceEntity.SmeltingRecipes[recipeID, i + 1];

                Item itm = Item.Get(oreID);

                if (itm == null) { return str; }

                if (i == 4)
                {
                    str += "\n";
                }

                str += String.Format("{0}: {1} ", itm.Name, amount);
            }


            return str;
        }

        #endregion
    }
}
