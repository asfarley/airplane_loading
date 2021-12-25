using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirplaneLoadingSimulation
{
    public class Seat
    {
        public int locationX;
        public int locationY;
        public int width;
        public int height;

        public int TargetLocationX => locationX - 10;
        public int TargetLocationY => locationY;
    }
}
