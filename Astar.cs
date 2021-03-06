using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Windows.Forms;

namespace AStarSharp
{
    public class Node
    {
        // Change this depending on what the desired size is for each element in the grid
        public static int NODE_SIZE = 2;
        public Node Parent;
        public Vector2 Position;
        public Vector2 Center
        {
            get
            {
                return new Vector2(Position.X + NODE_SIZE / 2, Position.Y + NODE_SIZE / 2);
            }
        }
        public float DistanceToTarget;
        public float Cost;
        public float Weight;
        public float F
        {
            get
            {
                if (DistanceToTarget != -1 && Cost != -1)
                    return DistanceToTarget + Cost;
                else
                    return -1;
            }
        }
        public bool Walkable;

        public Node(Vector2 pos, bool walkable, float weight = 1)
        {
            Parent = null;
            Position = pos;
            DistanceToTarget = -1;
            Cost = 1;
            Weight = weight;
            Walkable = walkable;
        }
    }

    public class Astar
    {
        public static bool EIGHTWAY = true;
        List<List<Node>> Grid;
        int GridRows
        {
            get
            {
               return Grid[0].Count;
            }
        }
        int GridCols
        {
            get
            {
                return Grid.Count;
            }
        }

        public Astar(List<List<Node>> grid)
        {
            Grid = grid;
        }

        public Stack<Node> FindPath(Vector2 Start, Vector2 End, PictureBox pic, Graphics im, Bitmap bmp)
        {
            Node start = new Node(new Vector2((int)(Start.X/Node.NODE_SIZE), (int) (Start.Y/Node.NODE_SIZE)), true);
            Node end = new Node(new Vector2((int)(End.X / Node.NODE_SIZE), (int)(End.Y / Node.NODE_SIZE)), true);

            Stack<Node> Path = new Stack<Node>();
            List<Node> OpenList = new List<Node>();
            List<Node> ClosedList = new List<Node>();
            List<Tuple<Node,bool>> adjacencies;
            Node current = start;
           
            // add start node to Open List
            OpenList.Add(start);

            while(OpenList.Count != 0 && !ClosedList.Exists(x => x.Position == end.Position))
            {
                current = OpenList[0];
                OpenList.Remove(current);
                ClosedList.Add(current);
                adjacencies = GetAdjacentNodes(current);

                foreach((Node n,bool f) in adjacencies)
                {
                    if (!ClosedList.Contains(n) && n.Walkable)
                    {
                        if (!OpenList.Contains(n))
                        {
                            n.Parent = current;
                            n.DistanceToTarget = Math.Abs(n.Position.X - end.Position.X) + Math.Abs(n.Position.Y - end.Position.Y);
                            n.Cost = n.Weight + n.Parent.Cost;
                            if (!f)
                            {
                                //n.Cost += 0.41421f; //Pythagoras
                                n.Cost += 0.75f; // "pyTHAgoRAS"
                            }

                            OpenList.Add(n);
                            OpenList = OpenList.OrderBy(node => node.F).ToList<Node>();
                            //DrawPath(OpenList, im);
                            //pic.Image = bmp;
                            //pic.Refresh();
                        }
                    }
                }
            }
            
            // construct path, if end was not closed return null
            if(!ClosedList.Exists(x => x.Position == end.Position))
            {
                return null;
            }

            // if all good, return path
            Node temp = ClosedList[ClosedList.IndexOf(current)];
            if (temp == null) return null;
            do
            {
                Path.Push(temp);
                temp = temp.Parent;
            } while (temp != start && temp != null) ;
            return Path;
        }

        private void DrawPath(List<Node> path, Graphics g)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var n = path[i];
                g.DrawRectangle(Pens.DarkOrchid, n.Center.X, n.Center.Y, 1, 1);
            }
        }
		
        private List<Tuple<Node,bool>> GetAdjacentNodes(Node n)
        {
            List<Tuple<Node,bool>> temp = new List<Tuple<Node, bool>>();

            int row = (int)n.Position.Y;
            int col = (int)n.Position.X;

            if(row + 1 < GridRows)
            {
                var node = Grid[col][row + 1];
                var b = true;
                temp.Add(Tuple.Create(node,b));
            }
            if(row - 1 >= 0)
            {
                var node = Grid[col][row - 1];
                var b = true;
                temp.Add(Tuple.Create(node, b));
            }
            if(col - 1 >= 0)
            {
                var node = Grid[col - 1][row];
                var b = true;
                temp.Add(Tuple.Create(node, b));
            }
            if(col + 1 < GridCols)
            {
                var node = Grid[col + 1][row];
                var b = true;
                temp.Add(Tuple.Create(node, b));
            }

            if (EIGHTWAY)
            {
                if (row + 1 < GridRows && col + 1 < GridCols)
                {
                    var node = Grid[col + 1][row + 1];
                    var b = false;
                    temp.Add(Tuple.Create(node, b));
                }
                if (row + 1 < GridRows && col - 1 >= 0)
                {
                    var node = Grid[col - 1][row + 1];
                    var b = false;
                    temp.Add(Tuple.Create(node, b));
                }
                if (col + 1 < GridCols && row - 1 >= 0)
                {
                    var node = Grid[col + 1][row - 1];
                    var b = false;
                    temp.Add(Tuple.Create(node, b));
                }
                if (col - 1 >= 0 && row - 1 >= 0)
                {
                    var node = Grid[col - 1][row - 1];
                    var b = false;
                    temp.Add(Tuple.Create(node, b));
                }
            }

            return temp;
        }
    }
}
