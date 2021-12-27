using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AirplaneLoadingSimulation
{
    public partial class Form1 : Form
    {
        private const int nPassengers = 10;
        private const int nSeats = 100;

        private AirplaneSim Sim = new AirplaneSim(nPassengers, nSeats, 550, 300);

        private Bitmap im;

        private bool ActivateSim = false;
        private bool NavigationDone = false;
        public Form1()
        {
            InitializeComponent();
            im = Sim.DrawMap();
        }

        private void NavigateAndDraw()
        {
            Sim.UpdateNavigation(im, simulationPictureBox);
            NavigationDone = true;
        }

        private void simulationTick_Tick(object sender, EventArgs e)
        {
            if (NavigationDone)
            {
                im = Sim.DrawSimulationState();
                simulationPictureBox.Image = im;

                if (ActivateSim)
                {
                    Sim.Update(im);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sim.Update(im);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ActivateSim = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ActivateSim = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ActivateSim = false;
            Sim = new AirplaneSim(nPassengers, nSeats, 550, 300);
            im = Sim.DrawSimulationState();
            simulationPictureBox.Image = im;
            Sim.UpdateNavigation(im, simulationPictureBox);
        }

        private void timerNavigation_Tick(object sender, EventArgs e)
        {
            NavigateAndDraw();
            timerNavigation.Enabled = false;
        }
    }
}
