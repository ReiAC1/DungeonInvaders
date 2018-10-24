using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DungeonInvaders.Util
{
    public class Pathfinder
    {

        public enum TileType
        {
            Floor,
            Wall,
            Path
        }

        private List<Node> openList = new List<Node>();
        private List<Node> closedList = new List<Node>();
        private List<Tile> tiles = new List<Tile>();

        public bool Diagonal = true;

        private Node start;
        private Node end;
        private Node current;
        private Vector2 mapSize;
        private Point MapPos;

        public Pathfinder(List<Tile> t, Vector2 mapSize)
        {
            this.mapSize = mapSize;
            tiles = t;
        }

        public Pathfinder(List<Tile> t,Vector2 mapSize, bool diagonal)
        {
            this.mapSize = mapSize;
            tiles = t;
            Diagonal = diagonal;
        }

        public bool SearchPath(ref Path p, Point offset, Point Start, Point End)
        {
            MapPos = offset;

            p.Clear();

            openList.Clear();
            closedList.Clear();

            Start -= MapPos;

            End -= MapPos;

            foreach (Tile t in tiles)
            {
                if (t.Type == TileType.Path)
                    t.Type = TileType.Floor;
            }

            start = new Node(Start.Y + (Start.X * (int)mapSize.Y), null);
            end = new Node(End.Y + (End.X * (int)mapSize.Y), null);

            if (start.CellIndex == end.CellIndex) { return true; }

            if (end.CellIndex >= tiles.Count || end.CellIndex < 0) { return false; }
            if (start.CellIndex >= tiles.Count || start.CellIndex < 0) { return false; }

            if (tiles[start.CellIndex].Type == TileType.Wall || tiles[end.CellIndex].Type == TileType.Wall)
                return false;

            openList.Add(start);
            current = start;

            int amt = 10000;
            while (amt > 0)
            {
                amt--;
                if (openList.Count == 0 || closedList.Count == tiles.Count)
                    break;

                current = FindSmallestF();

                if (current.CellIndex == end.CellIndex)
                    break;

                openList.Remove(current);
                closedList.Add(current);

                if (Diagonal)
                {
                    //Diagonal
                    AddAdjacentCellToOpenList(current, 1, -1, 14);
                    AddAdjacentCellToOpenList(current, -1, -1, 14);
                    AddAdjacentCellToOpenList(current, -1, 1, 14);
                    AddAdjacentCellToOpenList(current, 1, 1, 14);
                }
                //Straight
                AddAdjacentCellToOpenList(current, 0, -1, 10);
                AddAdjacentCellToOpenList(current, -1, 0, 10);
                AddAdjacentCellToOpenList(current, 1, 0, 10);
                AddAdjacentCellToOpenList(current, 0, 1, 10);

            }
            while (current != null)
            {
                bool endOnClosed = false;
                for (int v = 0; v < openList.Count; v++)
                    if (openList[v].CellIndex == end.CellIndex)
                        endOnClosed = true;
                if (endOnClosed &&
                    !(tiles[current.CellIndex].TilePos.X == Start.X && tiles[current.CellIndex].TilePos.Y == Start.Y) &&
                    !(tiles[current.CellIndex].TilePos.X == End.X && tiles[current.CellIndex].TilePos.Y == End.Y)
                )
                {
                    tiles[current.CellIndex].Type = TileType.Path;
                    p.PathNodes.Insert(0, ((tiles[current.CellIndex].TilePos + new Vector2(MapPos.X, MapPos.Y)) *
                        Tile.tilesize) + new Vector2(Tile.tilesize / 2));
                }
                current = current.Parent;
            }

            return amt > 0;
        }

        private Node FindSmallestF()
        {
            var smallestF = int.MaxValue;
            Node selectedNode = null;

            foreach (var node in openList)
            {
                if (node.F < smallestF)
                {
                    selectedNode = node;
                    smallestF = node.F;
                }
            }

            return selectedNode;
        }


        private void AddAdjacentCellToOpenList(Node parentNode, int columnOffset, int rowOffset, int gCost)
        {
            var adjacentCellIndex = GetAdjacentCellIndex(parentNode.CellIndex, columnOffset, rowOffset);

            // ignore unwalkable nodes (or nodes outside the grid)
            if (adjacentCellIndex == -1)
                return;

            // ignore nodes on the closed list
            if (closedList.Any(n => n.CellIndex == adjacentCellIndex))
                return;

            var adjacentNode = openList.SingleOrDefault(n => n.CellIndex == adjacentCellIndex);
            if (adjacentNode != null)
            {
                if (parentNode.G + gCost < adjacentNode.G)
                {
                    adjacentNode.Parent = parentNode;
                    adjacentNode.G = parentNode.G + gCost;
                    adjacentNode.F = adjacentNode.G + adjacentNode.H;
                }

                return;
            }

            var node = new Node(adjacentCellIndex, parentNode) { G = gCost, H = GetDistance(adjacentCellIndex, end.CellIndex) };
            node.F = node.G + node.H;
            openList.Add(node);
        }

        public int GetAdjacentCellIndex(int cellIndex, int columnOffset, int rowOffset)
        {
            var x = cellIndex % (int)mapSize.X;
            var y = cellIndex / (int)mapSize.X;

            if ((x + columnOffset < 0 || x + columnOffset > mapSize.X - 1) ||
                (y + rowOffset < 0 || y + rowOffset > mapSize.Y - 1))
                return -1;

            if (tiles[((y + rowOffset) * (int)mapSize.X) + (x + columnOffset)].Type == TileType.Wall)
                return -1;

            return cellIndex + columnOffset + (rowOffset * (int)mapSize.X);
        }

        public int GetDistance(int startTileID, int endTileID)
        {
            var startX = (int)tiles[startTileID].TilePos.X / (int)mapSize.X;
            var startY = (int)tiles[startTileID].TilePos.Y / (int)mapSize.Y;

            var endX = (int)tiles[endTileID].TilePos.X / (int)mapSize.X;
            var endY = (int)tiles[endTileID].TilePos.Y / (int)mapSize.Y;

            return Math.Abs(startX - endX) + Math.Abs(startY - endY);
        }


        public TileType this[int x, int y]
        {
            get
            {
                return tiles[y + (x * (int)mapSize.Y)].Type;
            }

            set
            {

                tiles[y + (x * (int)mapSize.Y)].Type = value;

            }
        }

        public List<Tile> GetTiles()
        {
            return tiles;
        }
    }
}
