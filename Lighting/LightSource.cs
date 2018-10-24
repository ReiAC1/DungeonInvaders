using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Lighting
{
    public class LightSource
    {
        public Vector2 Position
        {
            get;
            set;
        }

        public float Range
        {
            get;
            set;
        }

        public Color Color
        {
            get;
            set;
        }

        public Texture2D LightTexture
        {
            get;
            set;
        }

        public float Rotation
        {
            get;
            set;
        }

        public LightSource(Color color, float range, float FOV, Vector2 position)
        {
            int half = (int)range / 2;
            LightTexture = new Texture2D(Main.Graphics.GraphicsDevice, (int)range, (int)range);

            Color[] Colors = new Color[(int)range * (int)range];

            for (int x = 0; x < range; x++)
            {
                for (int y = 0; y < range; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(half, half));

                    if (distance < half && ((float)Math.Abs(Math.Atan2(x - half, -y - -half)) <= FOV || distance <= 16))
                    {
                        float amt = 1 - (distance / half);
                        float alpha = distance / half;

                        if ((float)Math.Abs(Math.Atan2(x - half, -y - -half)) > FOV && distance <= 16)
                        {
                            amt = 1 - (distance / 16);
                            alpha = distance / 16;
                        }

                        Colors[x + (y * (int)range)] = new Color(amt, amt, amt, 1);
                    }
                    else
                    {
                        Colors[x + (y * (int)range)] = Color.TransparentBlack;
                    }
                }
            }

            LightTexture.SetData<Color>(Colors);

            Color = color;
            Range = range;
            Position = position;
        }

        ~LightSource()
        {
            LightTexture.Dispose();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 center = new Vector2(LightTexture.Width / 2, LightTexture.Height / 2);
            float scale = Range / ((float)LightTexture.Width / 2.0f);
            spriteBatch.Draw(LightTexture, Position, null, Color, Rotation, center, scale, SpriteEffects.None, 1);
        }


    }
}
