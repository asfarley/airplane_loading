using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AStarSharp;

namespace AirplaneLoadingSimulation
{
    public class Passenger
    {
        public Seat seat;
        public int locationX;
        public int locationY;
        public int radius;
        public int speed;
        private int pathIndex = 0;
        private Stack<Node> path;

        public Astar Navigation;

        public void Move(List<Passenger> passengers)
        {
            var newPosition = path.ToArray()[pathIndex];

            if (PositionIsOccupied(passengers, (int) newPosition.Center.X, (int) newPosition.Center.Y))
            {
                return;
            }

            locationX = (int) newPosition.Center.X;
            locationY = (int) newPosition.Center.Y;
            if (pathIndex < path.Count - 1)
            {
                pathIndex++;
            }
        }

        public bool PositionIsOccupied(List<Passenger> passengers, int x, int y)
        {
            return passengers.Any(p => Math.Abs(p.locationX - x) < 10 && Math.Abs(p.locationY - y) < 10);
        }

        public void UpdateNavigation(Bitmap im)
        {
            var grid = BuildGrid(im);
            Navigation = new Astar(grid);
            pathIndex = 0;
            var start = new Vector2(locationX, locationY);
            var end = new Vector2(seat.TargetLocationX, seat.TargetLocationY);

            Trace.WriteLine("Starting path-planning.");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            path = Navigation.FindPath(start, end);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Trace.WriteLine("Path-planning took " + elapsedMs + " ms.");
        }

        public List<List<Node>> BuildGrid(Bitmap im)
        {
            List<List<Node>> grid = new List<List<Node>>();

            //Build base grid (no connections, one node-list per pixel)
            for (int i = 0; i < im.Size.Width; i++)
            {
                var col = new List<Node>();
                for (int j = 0; j < im.Size.Height; j++)
                {
                    var pos = new Vector2(i, j);
                    var walkable = isNotBlack(im, i, j);
                    float weight = walkable ? 1.0f : 0.0f;
                    var pixel = im.GetPixel(i, j);

                    var hint = HintWeight(pixel.R, pixel.G, pixel.B);
                    if (walkable && hint != 0.0f)
                    {
                        weight = hint;
                    }

                    var pixelNode = new Node(pos, walkable, weight);
                    col.Add(pixelNode);
                }

                grid.Add(col);
            }

            return grid;
        }

        private bool isWhite(Bitmap im, int x, int y)
        {
            var pixel = im.GetPixel(x, y);
            var isWhite = (pixel.R == 255 && pixel.G == 255 && pixel.B == 255);
            return isWhite;
        }

        private bool isNotBlack(Bitmap im, int x, int y)
        {
            var pixel = im.GetPixel(x, y);
            var isNotBlack = !(pixel.R == 0 && pixel.G == 0 && pixel.B == 0);
            return isNotBlack;
        }

        private float HintWeight(byte R, byte G, byte B)
        {
            var redIntensity = RedIntensity(R, G, B);
            var greenIntensity = GreenIntensity(R, G, B);

            if (redIntensity == 0.0 && greenIntensity == 0.0)
            {
                return 0.0f;
            }

            if (redIntensity > 0.0f && greenIntensity == 0.0f)
            {
                return 1.0f + redIntensity;
            }

            if (greenIntensity > 0.0f && greenIntensity == 0.0f)
            {
                return 1.0f/greenIntensity;
            }

            return 0.0f;
        }

        private float RedIntensity(byte R, byte G, byte B)
        {
            if (R == 255 && G == 255 && B == 255)
            {
                return 0.0f;
            }

            if (R == 0)
            {
                return 0.0f;
            }

            if (G == 0 && B == 0)
            {
                return R;
            }

            return 2 * R / (G + B);
        }

        private float GreenIntensity(byte R, byte G, byte B)
        {
            if (R == 255 && G == 255 && B == 255)
            {
                return 0.0f;
            }

            if (G == 0)
            {
                return 0.0f;
            }

            if (R == 0 && B == 0)
            {
                return G;
            }

            return 2 * G / (R + B);
        }
    }
}
