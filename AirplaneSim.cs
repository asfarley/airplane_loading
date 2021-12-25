using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AirplaneLoadingSimulation
{
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

        int dividerHeight = 200;
        public int boardingRampXOffset = 40;
        public int boardingRampWidth = 20;
        public int airplaneTopWall = 20;
        public int airplaneBottomWall = 70;
        public int chairDoorOffset = 10;

        public int Width;
        public int Height;

        public AirplaneSim(int nPassengers, int nSeats, int width, int height)
        {
            Width = width;
            Height = height;
            GenerateSeats(nSeats/4,4);
            GeneratePassengers(nPassengers);
            GenerateGeometry();
            AssignSeats();
        }

        public Bitmap DrawMap()
        {
            Bitmap im = new Bitmap(Width, Height);
            Graphics graphics = Graphics.FromImage(im);

            graphics.FillRectangle(Brushes.White, 0, 0, im.Width, im.Height);

            graphics.FillRectangle(Brushes.LawnGreen, boardingRampXOffset, airplaneBottomWall, boardingRampWidth, dividerHeight - airplaneBottomWall);

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

            graphics.FillRectangle(Brushes.LawnGreen, boardingRampXOffset, airplaneBottomWall, boardingRampWidth,  dividerHeight - airplaneBottomWall);

            foreach (var l in Lines)
            {
                graphics.DrawLine(Pens.Black, l.x0, l.y0, l.x1, l.y1);
            }

            foreach (var p in Passengers)
            {
                graphics.FillEllipse(Brushes.Blue, p.locationX - p.radius/2, p.locationY - p.radius/2, p.radius, p.radius);
                //graphics.DrawLine(Pens.Aqua,p.locationX,p.locationY, p.seat.locationX, p.seat.locationY);
            }

            foreach (var s in Seats)
            {
                graphics.FillRectangle(Brushes.Black, s.locationX, s.locationY, s.width, s.height);
            }

            return im;
        }

        private void GeneratePassengers(int nPassengers)
        {
            for (int i = 0; i < nPassengers; i++)
            {
                var p = new Passenger();
                p.radius = 8;
                p.locationX = i * 10 + 10;
                p.locationY = dividerHeight + 50;
                p.speed = 1;
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

        public void UpdateNavigation(Bitmap im)
        {
            foreach (var p in Passengers)
            {
                p.UpdateNavigation(im);
            }

        }

        public void MoveAll()
        {
            foreach (var p in Passengers)
            {
                p.Move();
            }

        }

        public void Update(Bitmap im)
        {
            MoveAll();
        }

    }
}
