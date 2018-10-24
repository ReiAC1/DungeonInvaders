using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Lighting
{
    public static class LightManager
    {
        #region Fields

        public static Effect LightEffect;

        public static RenderTarget2D RenderTarget;

        public static List<LightSource> Lights = new List<LightSource>();

        static BlendState BlendState;

        #endregion

        #region Properties

        static GraphicsDevice GraphicsDevice { get { return Main.Graphics.GraphicsDevice; } }

        #endregion

        public static void Startup()
        {
            Lights.Clear();

            if (RenderTarget == null)
            {
                RenderTarget = new RenderTarget2D(Main.Graphics.GraphicsDevice, Main.Graphics.PreferredBackBufferWidth,
                Main.Graphics.PreferredBackBufferHeight);

                LightEffect = Main.ContentManager.Load<Effect>("FX/Lighting");

                BlendState = new BlendState()
                {
                    ColorSourceBlend = Blend.SourceAlpha,
                    AlphaSourceBlend = Blend.SourceAlpha,
                    ColorDestinationBlend = Blend.InverseSourceAlpha,
                    AlphaDestinationBlend = Blend.InverseSourceAlpha,
                    ColorBlendFunction = BlendFunction.Max
                };
            }

            int degrees = 360;

            if (States.DungeonSettingsState.Difficulty == 2)
            {
                degrees = 40;
            }
            else if (States.DungeonSettingsState.Difficulty >= 3)
            {
                degrees = 30;
            }

            LightSource Source = new LightSource(Color.White, ((12 - States.DungeonSettingsState.Difficulty) * 32), MathHelper.ToRadians(degrees), new Vector2(200, 200));
            Lights.Add(Source);
        }

        public static void Draw()
        {
            GraphicsDevice.SetRenderTarget(RenderTarget);

            GraphicsDevice.Clear(Color.TransparentBlack);

            Main.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Util.Camera.View);

            foreach(LightSource light in Lights)
            {
                light.Draw(Main.SpriteBatch);
            }

            Main.SpriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            LightEffect.Parameters["LightTexture"].SetValue(RenderTarget);
            LightEffect.Parameters["ScreenWidth"].SetValue(Main.Graphics.PreferredBackBufferWidth);
            LightEffect.Parameters["ScreenHeight"].SetValue(Main.Graphics.PreferredBackBufferHeight);
        }

        public static void FormatLights()
        {
            Lights.RemoveAll(FormatLight);
        }


        static bool FormatLight(LightSource l)
        {
            return l == null;
        }
    }
}
