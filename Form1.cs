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
        private AirplaneSim Sim = new AirplaneSim(1, 50, 550, 300);

        private Bitmap im;

        private bool ActivateSim = false;
        public Form1()
        {
            InitializeComponent();

            im = Sim.DrawSimulationState();
            simulationPictureBox.Image = im;
            Sim.UpdateNavigation(im);
        }

        private void simulationTick_Tick(object sender, EventArgs e)
        {
            im = Sim.DrawSimulationState();
            simulationPictureBox.Image = im;

            if (ActivateSim)
            {
                Sim.Update(im);
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
    }
}
