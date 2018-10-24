using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Util.Entities
{
    public class FurnaceEntity : Entity
    {
        #region Fields

        public static List<int> SmeltingOutputs = new List<int>();

        public static readonly int[,] SmeltingRecipes = new int[,]
        {
            // Ore then Amount loop until output
            {1, 1, 2, 1, 0, -1, -1},
            {3, 1, 0, 2, 1, -1, -1},
            {4, 1, 0, 4, 2, -1, -1},
            {5, 1, 0, 8, 3, -1, -1},
            {6, 2, 5, 4, 0, 10, 4},
            {7, 2, 5, 4, 0, 12, 5}
        };

        public static readonly int[] SmeltingLevels = new int[]
        {
            1,
            10,
            30,
            40,
            60,
            80
        };

        public Lighting.LightSource Light;

        public static float Distance = 99999;

        #endregion

        #region Methods

        public FurnaceEntity()
        {
            Light = new Lighting.LightSource(Color.OrangeRed, 64, MathHelper.ToRadians(360), Vector2.Zero);
            Lighting.LightManager.Lights.Add(Light);

            Texture = Main.ContentManager.Load<Texture2D>("Furnace");
            IsPhysical = true;

            Size = new Vector2(Texture.Width, Texture.Height);
            Origin = Size / 2;
        }

        public override void OnDestroy(bool destroyState = false)
        {
            Lighting.LightManager.Lights.Remove(Light);
        }

        public override void Update()
        {
            Light.Position = Position;

            if (Vector2.Distance(Position, Map.Current.Player.Position) <= Distance)
            {
                Distance = Vector2.Distance(Position, Map.Current.Player.Position);
            }

            if (Distance > 72) { return; }

            if (Bounds.Contains(InputManager.CursorPosition) && InputManager.IsPerformingAction(InputManager.Action.PrimaryTrigger, true))
            {
                ((States.GameState)States.StateManager.CurrentState).HUD.InventoryMenu.ShowSmeltingTab();
                Map.Current.Player.ShouldAttack = false;
            }
        }

        #endregion
    }
}
