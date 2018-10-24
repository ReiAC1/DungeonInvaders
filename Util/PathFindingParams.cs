using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DungeonInvaders.Util
{
    public class PathFindingParams
    {
        public Path Path = new Path(null);
        public Point Goal;
        public bool LastResult = false, NeedsRefresh = true;
        public int Speed = 3, MinError = 6;
    }
}
