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
        private int nPassengers = 15;
        private int nSeats = 100;
        private AirplaneSim Sim;
        private BoardingStrategy strategy = BoardingStrategy.NONE;

        private Bitmap im;

        private bool ActivateSim = false;
        private bool NavigationDone = false;
        public Form1()
        {
            InitializeComponent();
            numPassengersTextBox.Text = nPassengers.ToString();
            Sim = new AirplaneSim(nPassengers, nSeats, 550, 300, strategy);
            im = Sim.DrawMap();
        }

        private void NavigateAndDraw()
        {
            Sim.UpdateNavigation(im, simulationPictureBox);
            NavigationDone = true;
            im = Sim.DrawSimulationState();
            simulationPictureBox.Image = im;
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
            nPassengers = Int32.Parse(numPassengersTextBox.Text);
            Sim = new AirplaneSim(nPassengers, nSeats, 550, 300, strategy);
            im = Sim.DrawMap();
            simulationPictureBox.Image = im;
            Sim.UpdateNavigation(im, simulationPictureBox);
        }

        private void timerNavigation_Tick(object sender, EventArgs e)
        {
            NavigateAndDraw();
            timerNavigation.Enabled = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                strategy = BoardingStrategy.NONE;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                strategy = BoardingStrategy.FAST_FIRST;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                strategy = BoardingStrategy.SLOW_FIRST;
            }
        }
    }
}
