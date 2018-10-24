using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Util.Entities
{
    public class TileEntity : Entity
    {
        static SoundEffect UnlockEffect;

        public int StartGID = 0, InitialGID = -1;

        public short Data = 0, Value = -1;

        public TileEntity(int startGID, int gid, Texture2D texture, int width, int height)
        {
            if (UnlockEffect == null)
            {
                UnlockEffect = Main.ContentManager.Load<SoundEffect>("door-unlock");
            }

            StartGID = startGID;
            Texture = texture;

            IsPhysical = false;

            SetGID(gid, width, height);
        }

        public void SetGID(int gid, int width, int height)
        {
            if (InitialGID == -1) { InitialGID = gid; }

            if (Texture.Width == 32 && Texture.Height == 32)
            {
                Source = new Rectangle(0, 0, 31, 31);
                return;
            }

            DrawColor = Color.White;

            Rectangle source = new Rectangle(0, 0, width - 1, height - 1);

            int xAmount = (int)Math.Ceiling((float)Texture.Width / width);

            source.X = (gid - StartGID) % xAmount;
            source.Y = (gid - StartGID) / xAmount;

            source.X *= width;
            source.Y *= height;

            Source = source;
        }

        public override void OnCollide(Entity entity)
        {
            if ((entity is PlayerEntity) && Data > 3 && Value > -1)
            {
                if (User.Keys[Value])
                {
                    Value = -1;
                    Data -= 4;
                    UnlockEffect.Play(Settings.SFXVolume, 0, 0);
                }
            }
            else if ((entity is PlayerEntity) && Data > 7 && Value == -1)
            {
                Room r = Map.Current.GetRoomFromPosition(Position);
                Data = (short)(Data % 4);
                Texture = r.Tilesets[0].Texture;
                SetGID(InitialGID, r.TileWidth, r.TileHeight);
                UnlockEffect.Play(Settings.SFXVolume, 0, 0);
            }
        }

        public override void Update()
        {
            base.Update();
        }

    }
}
