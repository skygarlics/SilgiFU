using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Windows.Shapes;

namespace SilgiFU
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private module.StopWatch stopwatch = new module.StopWatch();
        private module.Problem problem;
        EventHandler TimerUpdate;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void randomButton_Click(object sender, RoutedEventArgs e)
        {
            problemTextblock.Text = "TODO : make random test";
        }


        private void answerToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try { string temp = problem.answer; }
            catch (NullReferenceException except)
            {
                MessageBoxResult warn = MessageBox.Show("문제 세팅이 필요합니다", "Error");
                this.sender.IsChecked = !this.sender.IsChecked;
                return;
            }

            if (this.sender.IsChecked == true)
            {
                problemTextblock.Text = problem.answer;
                this.sender.Content = "문제 보기";
            }
            else
            {
                problemTextblock.Text = problem.question;
                this.sender.Content = "답안 보기";
            }
        }

        private void timerToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            TimerUpdate = new EventHandler(TimerUpdate_Tick);
            stopwatch.AddCallback(TimerUpdate);
            stopwatch.Start();
            timerToggleButton.Content = "Pause";
            
        }

        private void timerToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            stopwatch.Stop();
            timerToggleButton.Content = "Start";
        }

        private void resetTimerButton_Click(object sender, RoutedEventArgs e)
        {
            timerToggleButton.IsChecked = false;
            //stopwatch.Stop();
            //stopwatch.Reset();
            stopwatch.RemoveCallback(TimerUpdate);
            timerTextBlock.Text = "00:00:00";
        }

        public void TimerUpdate_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = stopwatch.getElapsed();
            string timer_string = String.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            timerTextBlock.Text = timer_string;
        }

        private void openFolderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /*
        private void problemButton_Click(object sender, RoutedEventArgs e)
        {
            //problemTextblock.Text = "문제";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            openFileDialog.Filter = "text file|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                // Open problem
                string path = openFileDialog.FileName;
                problem = new module.Problem();
                problem.Parser(new StreamReader(path));

                // Set problem
                problemTextblock.Text = problem.question;

            }
        }
        */
    }
}
