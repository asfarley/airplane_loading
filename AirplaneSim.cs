using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AirplaneLoadingSimulation
{
    public enum BoardingStrategy
    {
        SLOW_FIRST,
        FAST_FIRST,
        NONE
    }
    class AirplaneSim
    {
        //Airplane Simulation class
        //A collection of passengers starts outside the airplane, and boards the airplane. 
        //The passengers have a normal-curve variation in boarding speed. 
        //Each passenger has a unique assigned seat. 
        //When allowed to do so, each passenger proceeds to the seat on their ticket.

        public List<Passenger> Passengers = new List<Passenger>();
        public List<Seat> Seats = new List<Seat>();
        public List<Geometry> Lines = new List<Geometry>();

        public BoardingStrategy Strategy = BoardingStrategy.NONE;

        private bool AllSlowSeated => Passengers.Where(p => p.isSlow).All(p => p.seated);
        private bool AllFastSeated => Passengers.Where(p => !p.isSlow).All(p => p.seated);

        private List<Passenger> FastPassengers => Passengers.Where(p => !p.isSlow).ToList();
        private List<Passenger> SlowPassengers => Passengers.Where(p => p.isSlow).ToList();

        int dividerHeight = 200;
        public int boardingRampXOffset = 40;
        public int boardingRampWidth = 20;
        public int airplaneTopWall = 20;
        public int airplaneBottomWall = 70;
        public int chairDoorOffset = 10;

        public int Width;
        public int Height;

        public AirplaneSim(int nPassengers, int nSeats, int width, int height, BoardingStrategy strat)
        {
            Width = width;
            Height = height;
            Strategy = strat;
            GenerateSeats(nSeats/4,4);
            GeneratePassengers(nPassengers);
            GenerateGeometry();
            AssignSeats();
        }

        public void DrawPathHints(Graphics graphics)
        {
            graphics.FillRectangle(new LinearGradientBrush(new Rectangle(boardingRampXOffset, airplaneBottomWall-30, boardingRampWidth, dividerHeight - airplaneBottomWall + 30), Color.FromArgb(0, 255, 0), Color.FromArgb(255, 255, 255), 70.0f), boardingRampXOffset, airplaneBottomWall - 30, boardingRampWidth, dividerHeight - airplaneBottomWall + 30);

            graphics.FillRectangle(new LinearGradientBrush(new Rectangle(boardingRampXOffset, airplaneTopWall + 20, 400, 10), Color.FromArgb(0, 255, 0), Color.FromArgb(255, 255, 255), 0.0f), boardingRampXOffset, airplaneTopWall + 20, 400, 10);

            graphics.FillRectangle(new LinearGradientBrush(
                new Rectangle(boardingRampWidth + boardingRampXOffset, dividerHeight,
                    550 - boardingRampWidth - boardingRampXOffset, 300 - dividerHeight), Color.FromArgb(255, 255, 255),
                Color.FromArgb(255, 0, 0), 70.0f), new Rectangle(boardingRampWidth + boardingRampXOffset, dividerHeight,
                550 - boardingRampWidth - boardingRampXOffset, 300 - dividerHeight));

            graphics.FillRectangle(new LinearGradientBrush(
                new Rectangle(boardingRampWidth + boardingRampXOffset, dividerHeight,
                    550 - boardingRampWidth - boardingRampXOffset, 10), Color.FromArgb(0, 255, 0),
                Color.FromArgb(255, 255, 255), 0.0f), new Rectangle(boardingRampWidth + boardingRampXOffset, dividerHeight,
                550 - boardingRampWidth - boardingRampXOffset, 10));
        }

        public Bitmap DrawMap()
        {
            Bitmap im = new Bitmap(Width, Height);
            Graphics graphics = Graphics.FromImage(im);

            graphics.FillRectangle(Brushes.White, 0, 0, im.Width, im.Height);

            DrawPathHints(graphics);

            foreach (var l in Lines)
            {
                graphics.DrawLine(Pens.Black, l.x0, l.y0, l.x1, l.y1);
            }

            foreach (var s in Seats)
            {
                graphics.FillRectangle(Brushes.Black, s.locationX, s.locationY, s.width, s.height);
            }

            return im;
        }

        public Bitmap DrawSimulationState()
        {
            Bitmap im = new Bitmap(Width, Height);
            Graphics graphics = Graphics.FromImage(im);

            graphics.FillRectangle(Brushes.White, 0, 0, im.Width, im.Height);

            //DrawPathHints(graphics);

            foreach (var l in Lines)
            {
                graphics.DrawLine(Pens.Black, l.x0, l.y0, l.x1, l.y1);
            }

            foreach (var p in Passengers)
            {
                if (p.seated)
                {
                    graphics.FillEllipse(Brushes.DodgerBlue, p.locationX - p.radius / 2, p.locationY - p.radius / 2, p.radius, p.radius);
                }
                else
                {
                    graphics.FillEllipse(Brushes.Blue, p.locationX - p.radius / 2, p.locationY - p.radius / 2, p.radius, p.radius);
                }
            }

            foreach (var s in Seats)
            {
                graphics.FillRectangle(Brushes.Black, s.locationX, s.locationY, s.width, s.height);
            }

            return im;
        }

        private void GeneratePassengers(int nPassengers)
        {
            var rnd = new Random();

            for (int i = 0; i < nPassengers; i++)
            {
                var p = new Passenger();
                p.radius = 8;

                var locationChosen = false;
                while (!locationChosen)
                {
                    var tempX = rnd.Next(10, 490);
                    var tempY = rnd.Next(dividerHeight + 5, 299);

                    var occupied = Passengers.Any(pass => pass.locationX == tempX && pass.locationY == tempY);

                    if (!occupied)
                    {
                        p.locationX = tempX;
                        p.locationY = tempY;
                        locationChosen = true;
                    }
                }

                p.speed = rnd.Next(5, 15) / 10.0;
                if (p.speed < 1.0)
                {
                    p.isSlow = true;
                }

                Passengers.Add(p);
            }
        }

        private void GenerateSeats(int nRows, int nAisles)
        {
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nAisles; j++)
                {
                    var s = new Seat();
                    s.locationX = boardingRampXOffset + chairDoorOffset + boardingRampWidth + i * 12;
                    s.locationY = airplaneTopWall + j * 10 + ((j >= nAisles/2) ? 10 : 0);
                    s.height = 10;
                    s.width = 2;
                    Seats.Add(s);
                }
            }
        }

        private void GenerateGeometry()
        {
            var window1 = new Geometry
            {
                x0 = 0,
                y0 = dividerHeight,
                x1 = boardingRampXOffset,
                y1 = dividerHeight
            };

            var window2 = new Geometry
            {
                x0 = boardingRampXOffset + boardingRampWidth,
                y0 = dividerHeight,
                x1 = 550,
                y1 = dividerHeight
            };

            var window3 = new Geometry
            {
                x0 = 0,
                y0 = airplaneBottomWall,
                x1 = boardingRampXOffset,
                y1 = airplaneBottomWall
            };

            var window4 = new Geometry
            {
                x0 = boardingRampXOffset + boardingRampWidth,
                y0 = airplaneBottomWall,
                x1 = 550,
                y1 = airplaneBottomWall
            };

            var wall1 = new Geometry
            {
                x0 = 0,
                y0 = airplaneTopWall,
                x1 = 550,
                y1 = airplaneTopWall
            };

            var rampWall1 = new Geometry
            {
                x0 = boardingRampXOffset,
                y0 = airplaneBottomWall,
                x1 = boardingRampXOffset,
                y1 = dividerHeight
            };

            var rampWall2 = new Geometry
            {
                x0 = boardingRampXOffset + boardingRampWidth,
                y0 = airplaneBottomWall,
                x1 = boardingRampXOffset + boardingRampWidth,
                y1 = dividerHeight
            };

            Lines.Add(window1);
            Lines.Add(window2);
            Lines.Add(window3);
            Lines.Add(window4);
            Lines.Add(wall1);
            Lines.Add(rampWall1);
            Lines.Add(rampWall2);
        }

        public void AssignSeats()
        {
            var shuffledSeats = Seats.OrderBy(x => Guid.NewGuid()).ToList();
            for (int i = 0; i < Passengers.Count; i++)
            {
                Passengers[i].seat = shuffledSeats[i];
            }
        }

        public void UpdateNavigation(Bitmap im, PictureBox box)
        {
            //Copy bitmap
            Dictionary<Passenger, Bitmap> pbMap = new Dictionary<Passenger, Bitmap>();
            Dictionary<Passenger, Graphics> pgMap = new Dictionary<Passenger, Graphics>();

            foreach (var p in Passengers)
            {
                Bitmap copy = new Bitmap(im);
                var g = Graphics.FromImage(copy);
                pbMap[p] = copy;
                pgMap[p] = g;
            }

            //foreach (var p in Passengers)
            //{
            //    p.UpdateNavigation(pbMap[p], pgMap[p], box);
            //}

            Parallel.ForEach(Passengers, p =>
            {
                p.UpdateNavigation(pbMap[p], pgMap[p], box);
            });
        }

        public void MoveAll()
        {
            switch (Strategy)
            {
                case BoardingStrategy.NONE:
                    MoveStrategyNone();
                    break;
                case BoardingStrategy.FAST_FIRST:
                    MoveStrategyFastFirst();
                    break;
                case BoardingStrategy.SLOW_FIRST:
                    MoveStrategySlowFirst();
                    break;
            }
        }

        public void MoveStrategyNone()
        {
            foreach (var p in Passengers)
            {
                var listJustThis = new List<Passenger> { p };
                var otherPassengers = Passengers.Except(listJustThis);
                p.Move(otherPassengers.ToList());
            }
        }

        public void MoveStrategyFastFirst()
        {
            foreach (var p in FastPassengers)
            {
                var listJustThis = new List<Passenger> { p };
                var otherPassengers = Passengers.Except(listJustThis);
                p.Move(otherPassengers.ToList());
            }

            if (!AllFastSeated)
            {
                return;
            }

            foreach (var p in SlowPassengers)
            {
                var listJustThis = new List<Passenger> { p };
                var otherPassengers = Passengers.Except(listJustThis);
                p.Move(otherPassengers.ToList());
            }
        }

        public void MoveStrategySlowFirst()
        {
            foreach (var p in SlowPassengers)
            {
                var listJustThis = new List<Passenger> { p };
                var otherPassengers = Passengers.Except(listJustThis);
                p.Move(otherPassengers.ToList());
            }

            if (!AllSlowSeated)
            {
                return;
            }

            foreach (var p in FastPassengers)
            {
                var listJustThis = new List<Passenger> { p };
                var otherPassengers = Passengers.Except(listJustThis);
                p.Move(otherPassengers.ToList());
            }
        }

        public void Update(Bitmap im)
        {
            MoveAll();
        }

    }
}
