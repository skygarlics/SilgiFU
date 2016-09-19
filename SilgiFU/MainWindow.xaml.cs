using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Text;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Data;

namespace SilgiFU
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private module.StopWatch stopwatch = new module.StopWatch();
        private module.Problem problem;
       
        List<module.Problem> probList = new List<module.Problem>();
        Random random = new Random((int)DateTime.Now.Ticks);
        Encoding kor_encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
        double limit_miliseconds;

        EventHandler TimerUpdate;
        EventHandler TimeOut;

        public MainWindow()
        {
            InitializeComponent();
            problemListBox.ItemsSource = probList;
        }


        private void timerToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            TimerUpdate = new EventHandler(timerUpdate_Tick);
            stopwatch.AddCallback(TimerUpdate);
            stopwatch.Start();

            var sender1 = (ToggleButton)sender;
            sender1.Content = "Pause";
        }



        private void timerToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var sender1 = (ToggleButton)sender;
            stopwatch.Stop();
            timerToggleButton.Content = "Start";
        }

        private void resetTimerButton_Click(object sender, RoutedEventArgs e)
        {
            resetTimer();
        }

        private void resetTimer()
        {
            timerToggleButton.IsChecked = false;
            stopwatch.Stop();
            stopwatch.Reset();
            stopwatch.RemoveCallback(TimerUpdate);
            timerTextBlock.Text = "00:00.00";
        }

        public void timerUpdate_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = stopwatch.getElapsed();
            string timer_string = String.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            timerTextBlock.Text = timer_string;
        }

        private void openFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            openFileDialog.IsFolderPicker = true;
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string folder = openFileDialog.FileName;
                foreach (string path in System.IO.Directory.GetFiles(folder))
                {
                    if (path.Substring(path.Length - 4) == ".txt")
                    {
                        probList.Add(new module.Problem(new StreamReader(path, kor_encode)));
                        refreshProblemListbox();
                    }
                }            
            }
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            //problemTextblock.Text = "문제";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            openFileDialog.Filter = "text file|*.txt";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string path in openFileDialog.FileNames)
                {
                    if (path.Substring(path.Length - 4) == ".txt")
                    {
                        // Open problem
                        //string path = openFileDialog.FileName;

                        // Set problem
                        probList.Add(new module.Problem(new StreamReader(path)));
                        refreshProblemListbox();
                    }
                }            
            }
        }

        private void setProblem(module.Problem prob)
        {
            resetTimer();
            problem = prob;
            titleTextBlock.Text = prob.title;
            problemTextblock.Text = prob.question;
            answerToggleButton.IsChecked = false;
            answerToggleButton.Content = "답안 보기";
        }


        private void refreshProblemListbox()
        {
            problemListBox.ItemsSource = null;
            problemListBox.ItemsSource = probList;
        }

        private void problemListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var sender1 = (ListBox)sender;
            int index = sender1.SelectedIndex;
            if (index > -1)
            {
                setProblem(probList[index]);
            }
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            var sender1 = (ListBox)problemListBox;
            int index = sender1.SelectedIndex;
            int length = problemListBox.Items.Count;

            if (index > 0)
            {
                index--;
                setProblem(probList[index]);
                sender1.SelectedIndex = index;
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            var sender1 = (ListBox)problemListBox;
            int index = sender1.SelectedIndex;
            int length = problemListBox.Items.Count;
            
            if (index > -1)
            {
                // length-1 is index of last item
                if (index < length -1)
                {
                    index++;
                    setProblem(probList[index]);
                    sender1.SelectedIndex = index;
                }
            }

        }

        private void randomButton_Click(object sender, RoutedEventArgs e)
        {
            var sender1 = (ListBox)problemListBox;
            int index = sender1.SelectedIndex;

            int length = problemListBox.Items.Count;
            if (length == 0) return;

            for (int idx=0; idx<100; idx++)
            {
                int next_index = random.Next(0, length);
                if (index == next_index) continue;
                        
                setProblem(probList[next_index]); 
                sender1.SelectedIndex = next_index;
            }
        }

        private void removeProblemButton_Click(object sender, RoutedEventArgs e)
        {
            var sender1 = problemListBox;
            foreach (module.Problem prob in sender1.SelectedItems)
            {
                probList.Remove(prob);
            }
            refreshProblemListbox();
        }

        private void removeAllButton_Click(object sender, RoutedEventArgs e)
        {
            var sender1 = problemListBox;
            probList.Clear();
            refreshProblemListbox();
        }

        private void timelimitCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            timelimitTextBox.IsEnabled = true;
            TimeOut = new EventHandler(timeout_Tick);
            stopwatch.AddCallback(TimeOut);
        }

        private void timelimitCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            timelimitTextBox.IsEnabled = false;
            stopwatch.RemoveCallback(TimeOut);
        }

        private void timeout_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = stopwatch.getElapsed();
            if (ts.TotalMilliseconds >= limit_miliseconds * 1000)
            {
                //stopwatch.Stop();
                timerToggleButton.IsChecked = false;
                MessageBox.Show("Time Out!");
            }
        }


        private void timelimitTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            char ckey = KeyEventUtility.GetCharFromKey(e.Key);
            if (!char.IsControl(ckey) && !char.IsDigit(ckey) && (ckey != '.'))
            {              
                e.Handled = true;
            }
        }

        private void timelimitTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            limit_miliseconds = Convert.ToDouble(timelimitTextBox.Text);
        }

        private void answerToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var sender1 = (ToggleButton)sender;
            try { string temp = problem.answer; }
            catch (NullReferenceException except)
            {
                MessageBoxResult warn = MessageBox.Show("문제 세팅이 필요합니다", "Error");
                sender1.IsChecked = !sender1.IsChecked;
                return;
            }

            if (sender1.IsChecked == true)
            {
                problemTextblock.Text = problem.answer;
                sender1.Content = "문제 보기";
            }
            else
            {
                problemTextblock.Text = problem.question;
                sender1.Content = "답안 보기";
            }
        }
    }
}
