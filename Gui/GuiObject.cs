using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonInvaders.Gui
{
    public abstract class GuiObject
    {
        public virtual void Update() { }

        public abstract void Draw();
    }
}
