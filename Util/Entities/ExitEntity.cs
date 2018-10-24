using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonInvaders.Util.Entities
{
    public class ExitEntity : Entity
    {
        #region Methods

        public ExitEntity()
        {
            Texture = null;
            IsPhysical = false;
        }

        public override void Update()
        {
            
        }

        public override void OnCollide(Entity entity)
        {
            if (entity is PlayerEntity && Texture != null)
            {
                entity.CanDispose = true;
            }
        }

        #endregion
    }
}
