using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AStarSharp;

namespace AirplaneLoadingSimulation
{
    public class Passenger
    {
        public Seat seat;
        public bool seated = false;
        public int locationX;
        public int locationY;
        public int radius;
        public double speed;
        public bool isSlow = false;
        private double pathIndex = 0.0;
        private Stack<Node> path;

        public Astar Navigation;

        private int impatience = 0;
        private int impatience_threshold = 20;

        private Random rnd = new Random();

        public void Move(List<Passenger> passengers)
        {
            var integerIndex = (int) Math.Floor(pathIndex);
            var newPosition = path.ToArray()[integerIndex];

            if (PositionIsOccupied(passengers, (int) newPosition.Center.X, (int) newPosition.Center.Y))
            {
                impatience += rnd.Next(0, 3);

                if (impatience < impatience_threshold)
                {
                    return;
                }
            }
            else
            {
                impatience = 0;
            }

            if (pathIndex < path.Count - 1.0 - speed)
            {
                pathIndex += speed;
            }
            else
            {
                seated = true;
            }

            locationX = (int) newPosition.Center.X;
            locationY = (int) newPosition.Center.Y;
        }

        public bool PositionIsOccupied(List<Passenger> passengers, int x, int y)
        {
            return passengers.Any(p => !p.seated && Math.Sqrt(Math.Pow(p.locationX - x, 2) + Math.Pow(p.locationY - y, 2)) < 10);
        }

        public void UpdateNavigation(Bitmap im, Graphics g, PictureBox pic)
        {
            var grid = BuildGrid(im);
            Navigation = new Astar(grid);
            pathIndex = 0;
            var start = new Vector2(2 * locationX, 2 * locationY);
            var end = new Vector2(2 * seat.TargetLocationX, 2 * seat.TargetLocationY);

            Trace.WriteLine("Starting path-planning.");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            path = Navigation.FindPath(start, end, pic, g, im);
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
                    float weight = 1.0f;
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

            if (redIntensity > greenIntensity)
            {
                return 1.0f + 100.0f*redIntensity;
            }

            if (greenIntensity > redIntensity)
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

            return 2.0f * R / (G + B);
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

            return 2.0f * G / (R + B);
        }
    }
}
