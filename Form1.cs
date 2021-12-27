using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AirplaneLoadingSimulation
{
    public partial class Form1 : Form
    {
        private int nPassengers = 1;
        private int nSeats = 100;
        private AirplaneSim Sim;
        private BoardingStrategy strategy = BoardingStrategy.NONE;
        private Stopwatch BoardingTimer;

        private int nRunsNONEStrategy = 0;
        private int nRunsFASTStrategy = 0;
        private int nRunSLOWStrategy = 0;

        private TimeSpan TotalTimeNONE = new TimeSpan(0);
        private TimeSpan TotalTimeFAST = new TimeSpan(0);
        private TimeSpan TotalTimeSLOW = new TimeSpan(0);

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

                    if (Sim.AllSeated)
                    {
                        if (!BoardingTimer.IsRunning) return;

                        BoardingTimer.Stop();
                        boardingTimeTextbox.Text = BoardingTimer.Elapsed.ToString();

                        switch (strategy)
                        {
                            case BoardingStrategy.NONE:
                                nRunsNONEStrategy++;
                                TotalTimeNONE += BoardingTimer.Elapsed;
                                break;
                            case BoardingStrategy.FAST_FIRST:
                                nRunsFASTStrategy++;
                                TotalTimeFAST += BoardingTimer.Elapsed;
                                break;
                            case BoardingStrategy.SLOW_FIRST:
                                nRunSLOWStrategy++;
                                TotalTimeSLOW += BoardingTimer.Elapsed;
                                break;
                        }

                        if (nRunsNONEStrategy > 0)
                        {
                            var avgBoardingTimeNone = TimeSpan.FromTicks(TotalTimeNONE.Ticks / nRunsNONEStrategy);
                            boardingTimeNoneTextbox.Text = avgBoardingTimeNone.ToString();
                        }

                        if (nRunsFASTStrategy > 0)
                        {
                            var avgBoardingTimeFast = TimeSpan.FromTicks(TotalTimeFAST.Ticks / nRunsFASTStrategy);
                            boardingTimeFastTextbox.Text = avgBoardingTimeFast.ToString();
                        }

                        if (nRunSLOWStrategy > 0)
                        {
                            var avgBoardingTimeSlow = TimeSpan.FromTicks(TotalTimeSLOW.Ticks / nRunSLOWStrategy);
                            boardingTimeSlowTextbox.Text = avgBoardingTimeSlow.ToString();
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sim.Update(im);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BoardingTimer = Stopwatch.StartNew();
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
