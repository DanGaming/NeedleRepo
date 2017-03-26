using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WpfApplicationNeedleClass
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Rectangle tmpRect = new Rectangle();
        BackgroundWorker backgroundWorker;
        bool started = false;

        //first array index is the click event
        //second array index is the drag leave event
        bool[] needle1_confirmation = new bool[2];
        bool[] needle2_confirmation = new bool[2];
        bool[] needle3_confirmation = new bool[2];

        int start_time = 0, difficulty_multiplier = 0;
        List<string> result = new List<string>();
        //int endGame;
        int endGameScore;
        //  int endGameHighScore;
        string difficulty;
        //Score array loops etc
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork +=
            new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.ProgressChanged +=
            new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            //end game test code
            //array = new int[numOfIntegers]; 
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            saveButton.IsEnabled = false;
            //limit number of characters for highscore formatting
            nameTextBox.MaxLength = 10;
            //limit number of character input for discs as max is 10 only 2 characters needed
            numDiscs.MaxLength = 2;
            //stop user entering name into high score until the end game function is stepped into.
            nameTextBox.IsEnabled = false;
            buttonStart.IsEnabled = false;

            //stop user clicking needle until game starts
            needle3.IsEnabled = false;
            needle2.IsEnabled = false;
            needle1.IsEnabled = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// needle one mouseleftbutton down event handler
        /// </summary>
        private void needle1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //type casting
            Needle needleTmp = new Needle();
            needleTmp = sender as Needle;
            tmpRect = needleTmp.Children[needleTmp.Children.Count - 1] as Rectangle;
            DragDropEffects allowedEffects = DragDropEffects.Move;
            DragDrop.DoDragDrop(needleTmp, tmpRect, allowedEffects);

            //error handle so cannot solve mid game
            buttonStart.IsEnabled = false;
            //end game parameter
            updateScores();
        }
        /// <summary>
        /// needle two mouseleftbutton down event handler
        /// </summary>
        private void needle2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //type casting
            Needle needleTmp = new Needle();
            needleTmp = sender as Needle;
            tmpRect = needleTmp.Children[needleTmp.Children.Count - 1] as Rectangle;
            DragDropEffects allowedEffects = DragDropEffects.Move;
            DragDrop.DoDragDrop(needleTmp, tmpRect, allowedEffects);
            updateScores();
        }
        /// <summary>
        /// needle three mouseleftbutton down event handler
        /// </summary>
        private void needle3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //type casting
            Needle needleTmp = new Needle();
            needleTmp = sender as Needle;
            tmpRect = needleTmp.Children[needleTmp.Children.Count - 1] as Rectangle;
            DragDropEffects allowedEffects = DragDropEffects.Move;
            DragDrop.DoDragDrop(needleTmp, tmpRect, allowedEffects);
            updateScores();


            //double click error handler
            // if (e.ClickCount >= 2)
            //  {
            //      string hello; //only hit here on double click
            //  }
        }

        /// <summary>
        ///needle 1 drop event handler
        /// </summary>
        private void needle1_Drop(object sender, DragEventArgs e)
        {
            Rectangle to_current_rectangle = null;

            //User is dragging from needle 2 to needle 1
            if (needle2_confirmation[1] == true)
            {
                //assign a rectangle variable to the element needle the disc is coming from needle 2
                Rectangle from_current_rectangle = (Rectangle)needle2.Children[(needle2.Children.Count - 1)];

                //Check if the destination needle does not have any discs
                //and assign the new disc there since
                if (needle1.Children.Count == 0)
                {
                    needle2.Children.Remove(tmpRect);
                    needle1.Children.Add(tmpRect);
                }

                //this means needle 1 has one disc or more, so check the width of them 
                //to make sure that the disc FROM isn't greater in width than the 
                //current disc on needle 1
                else
                {
                    //assign a rectangle element to the top disc of needle 1
                    to_current_rectangle = (Rectangle)needle1.Children[(needle1.Children.Count - 1)];

                    if (from_current_rectangle.Width < to_current_rectangle.Width)
                    {
                        needle2.Children.Remove(tmpRect);
                        needle1.Children.Add(tmpRect);
                    }

                }

                needle2_confirmation[1] = false;
            }

            //if user is trying to move a disc from needle 3 to needle 1
            else if (needle3_confirmation[1] == true)
            {
                //assign a rectangle variable to the element needle the disc is coming from needle 3
                Rectangle from_current_rectangle = (Rectangle)needle3.Children[(needle3.Children.Count - 1)];

                //Check if the destination needle does not have any discs
                //and assign the new disc there since
                if (needle1.Children.Count == 0)
                {
                    needle3.Children.Remove(tmpRect);
                    needle1.Children.Add(tmpRect);
                }

                //this means needle 1 has one disc or more, so check the width of them 
                //to make sure that the disc FROM isn't greater in width than the 
                //current disc on needle 1
                else
                {
                    //assign a rectangle element to the top disc of needle 1
                    to_current_rectangle = (Rectangle)needle1.Children[(needle1.Children.Count - 1)];

                    if (from_current_rectangle.Width < to_current_rectangle.Width)
                    {
                        needle3.Children.Remove(tmpRect);
                        needle1.Children.Add(tmpRect);
                    }


                }

                needle3_confirmation[1] = false;
            }

            else
                MessageBox.Show("Please make sure that you have drag & dropped the disc correctly!");
        } //end of needle 1 event handler

        //user is dropping disc to needle 2

        /// <summary>
        ///needle 2 drop event handler
        /// </summary>
        private void needle2_Drop(object sender, DragEventArgs e)
        {
            Rectangle to_current_rectangle = null;


            //User is dragging from needle 1 to needle 2
            if (needle1_confirmation[1] == true)
            {
                //assign a rectangle variable to the element needle the disc is coming from needle 2
                Rectangle from_current_rectangle = (Rectangle)needle1.Children[(needle1.Children.Count - 1)];

                //Check if the destination needle does not have any discs
                //and assign the new disc there is
                if (needle2.Children.Count == 0)
                {
                    needle1.Children.Remove(tmpRect);
                    needle2.Children.Add(tmpRect);
                }
                //this means needle 2 has one disc or more, so check the width of them 
                //to make sure that the disc FROM isn't greater in width than the 
                //current disc on needle 1
                else
                {
                    //assign a rectangle element to the top disc of needle 1
                    to_current_rectangle = (Rectangle)needle2.Children[(needle2.Children.Count - 1)];

                    if (from_current_rectangle.Width < to_current_rectangle.Width)
                    {
                        needle1.Children.Remove(tmpRect);
                        needle2.Children.Add(tmpRect);
                    }
                }
                needle1_confirmation[1] = false;
            }

             //User is dragging from needle 3 to needle 2
            else if (needle3_confirmation[1] == true)
            {
                //assign a rectangle variable to the element needle the disc is coming from needle 2
                Rectangle from_current_rectangle = (Rectangle)needle3.Children[(needle3.Children.Count - 1)];

                //Check if the destination needle does not have any discs
                //and assign the new disc there is
                if (needle2.Children.Count == 0)
                {
                    needle3.Children.Remove(tmpRect);
                    needle2.Children.Add(tmpRect);
                }
                //this means needle 2 has one disc or more, so check the width of them 
                //to make sure that the disc FROM isn't greater in width than the 
                //current disc on needle 1
                else
                {
                    //assign a rectangle element to the top disc of needle 2
                    to_current_rectangle = (Rectangle)needle2.Children[(needle2.Children.Count - 1)];

                    if (from_current_rectangle.Width < to_current_rectangle.Width)
                    {
                        needle3.Children.Remove(tmpRect);
                        needle2.Children.Add(tmpRect);
                    }
                }
                needle3_confirmation[1] = false;
            }

            else
                MessageBox.Show("Please make sure that you have drag & dropped the disc correctly!");
        } //end of needle 2 event handler

        //user is dragging to needle 3

        /// <summary>
        ///needle 3 drop event handler
        /// </summary>
        private void needle3_Drop(object sender, DragEventArgs e)
        {
            Rectangle to_current_rectangle = null;



            //User is dragging from needle 1 to needle 3
            if (needle1_confirmation[1] == true)
            {
                //assign a rectangle variable to the element needle the disc is coming from needle 1
                Rectangle from_current_rectangle = (Rectangle)needle1.Children[(needle1.Children.Count - 1)];

                //Check if the destination needle does not have any discs
                //and assign the new disc there is
                if (needle3.Children.Count == 0)
                {
                    needle1.Children.Remove(tmpRect);
                    needle3.Children.Add(tmpRect);
                }

                //this means needle 3 has one disc or more, so check the width of them 
                //to make sure that the disc FROM isn't greater in width than the 
                //current disc on needle 1
                else
                {
                    //assign a rectangle element to the top disc of needle 1
                    to_current_rectangle = (Rectangle)needle3.Children[(needle3.Children.Count - 1)];

                    if (from_current_rectangle.Width < to_current_rectangle.Width)
                    {
                        needle1.Children.Remove(tmpRect);
                        needle3.Children.Add(tmpRect);
                    }


                }

                needle1_confirmation[1] = false;
            }

           //User is dragging from needle 2 to needle 3
            else if (needle2_confirmation[1] == true)
            {
                //assign a rectangle variable to the element needle the disc is coming from needle 1
                Rectangle from_current_rectangle = (Rectangle)needle2.Children[(needle2.Children.Count - 1)];

                //Check if the destination needle does not have any discs
                //and assign the new disc there is
                if (needle3.Children.Count == 0)
                {
                    needle2.Children.Remove(tmpRect);
                    needle3.Children.Add(tmpRect);
                }

                //this means needle 3 has one disc or more, so check the width of them 
                //to make sure that the disc FROM isn't greater in width than the 
                //current disc on needle 1
                else
                {
                    //assign a rectangle element to the top disc of needle 1
                    to_current_rectangle = (Rectangle)needle3.Children[(needle3.Children.Count - 1)];

                    if (from_current_rectangle.Width < to_current_rectangle.Width)
                    {
                        needle2.Children.Remove(tmpRect);
                        needle3.Children.Add(tmpRect);
                    }


                }

                needle2_confirmation[1] = false;
            }

            else
                MessageBox.Show("Please make sure that you have drag & dropped the disc correctly!");
        } //end of needle 3 event handler

        /// <summary>
        ///calls on the background worker and also sets the speed of solve
        /// </summary>
        private void TowerOfHanoi(int numDisks, string initNeedle, string endNeedle, string tmpNeedle)
        {
            if (numDisks == 1)
            {
                // move this disk from initNeedle to endNeedle
                backgroundWorker.ReportProgress(0, new TowerOfHanoiState(initNeedle, endNeedle));
                System.Threading.Thread.Sleep(50);

            }

            else
            {
                // move n-1 disks from initNeedle to tmpNeedle
                TowerOfHanoi(numDisks - 1, initNeedle, tmpNeedle, endNeedle);
                // move the last disk from initNeedle to endNeedle
                backgroundWorker.ReportProgress(0, new TowerOfHanoiState(initNeedle, endNeedle));
                System.Threading.Thread.Sleep(50);
                // move n-1 disks from tmpNeedle to endNeedle
                TowerOfHanoi(numDisks - 1, tmpNeedle, endNeedle, initNeedle);
            }
        }
        /// <summary>
        ///order of operation for backgroundworker
        /// </summary>
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            TowerOfHanoi(
            (int)e.Argument, "needle1", "needle3", "needle2");

        }
       
        /// <summary>
        ///recursive algorithm that shows solve
        /// </summary>        
        private void ShowProgress(string fromNeedle, string toNeedle)
        {
            textBlockResult.Text += fromNeedle + " to " + toNeedle + "\n";
            if (fromNeedle == "needle1" && toNeedle == "needle3")
            {
                if (needle1.Children.Count == 0)
                    MessageBox.Show("Code later");
                else
                {
                    Rectangle to_add = (Rectangle)needle1.Children[needle1.Children.Count - 1];
                    needle1.Children.RemoveAt(needle1.Children.Count - 1);
                    needle3.Children.Add(to_add);
                }
            }

            else if (fromNeedle == "needle3" && toNeedle == "needle1")
            {
                Rectangle to_add = (Rectangle)needle3.Children[needle3.Children.Count - 1];
                needle3.Children.RemoveAt(needle3.Children.Count - 1);
                needle1.Children.Add(to_add);
            }

            else if (fromNeedle == "needle1" && toNeedle == "needle2")
            {
                Rectangle to_add = (Rectangle)needle1.Children[needle1.Children.Count - 1];
                needle1.Children.RemoveAt(needle1.Children.Count - 1);
                needle2.Children.Add(to_add);
            }

            else if (fromNeedle == "needle2" && toNeedle == "needle1")
            {
                Rectangle to_add = (Rectangle)needle2.Children[needle2.Children.Count - 1];
                needle2.Children.RemoveAt(needle2.Children.Count - 1);
                needle1.Children.Add(to_add);
            }

            else if (fromNeedle == "needle2" && toNeedle == "needle3")
            {
                Rectangle to_add = (Rectangle)needle2.Children[needle2.Children.Count - 1];
                needle2.Children.RemoveAt(needle2.Children.Count - 1);
                needle3.Children.Add(to_add);
            }

            else if (fromNeedle == "needle3" && toNeedle == "needle2")
            {
                Rectangle to_add = (Rectangle)needle3.Children[needle3.Children.Count - 1];
                needle3.Children.RemoveAt(needle3.Children.Count - 1);
                needle2.Children.Add(to_add);
            }

            else
            {
                MessageBox.Show("fromNeedle is: " + fromNeedle + " and toNeedle is: " + toNeedle);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender,
        ProgressChangedEventArgs e)
        {
            TowerOfHanoiState state = (TowerOfHanoiState)e.UserState;
            ShowProgress(state.FromNeedle, state.ToNeedle);
        }

        /// <summary>
        ///recursive algorithm button event handler
        /// </summary>
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            //stop timer
            started = false;
            //clear timer content
            time.Content = String.Empty;
            //clear scorelabel - this stops highscore parameterbecause score is null
            scoreLabel.Content = String.Empty;
            needle3.IsEnabled = false;
            needle2.IsEnabled = false;
            needle1.IsEnabled = false;
            buttonStart.IsEnabled = false;
            
            int numDisks = needle1.Children.Count;
            backgroundWorker.RunWorkerAsync(numDisks);
        }

        /// <summary>
        ///start game button event handler
        /// </summary> 
        private void startGame_Click(object sender, RoutedEventArgs e)
        {
            needle3.IsEnabled = true;
            needle2.IsEnabled = true;
            needle1.IsEnabled = true;
            // if statement checks radio button is pressed then steps into
            if ((bool)easyDifficulty.IsChecked || (bool)mediumDifficulty.IsChecked || (bool)hardDifficulty.IsChecked)
            {

                //error handling to grey out buttons so the user cannot start the solve function mid game, 
                //cannot save highscores etc.
                startGame.IsEnabled = false;
                buttonStart.IsEnabled = true;
                saveButton.IsEnabled = false;
                hardDifficulty.IsEnabled = false;
                mediumDifficulty.IsEnabled = false;
                easyDifficulty.IsEnabled = false;

                int num_iterations;
                int.TryParse(numDiscs.Text, out num_iterations);

                if (num_iterations != 0)
                {
                    //start time = difficulty times number of discs
                    start_time = (difficulty_multiplier * num_iterations);
                    //start timer
                    dispatcherTimer.Start();

                    started = true;
                    //enter if else sorting here
                }

                else
                {
                    MessageBox.Show("Please enter a number");
                    startGame.IsEnabled = true;
                }
                if (num_iterations <= 10)
                {
                    for (int i = 0; i < num_iterations; i++)
                    {
                        //adding the rectangles
                        //add rectangles
                        Rectangle tmpRect = new Rectangle();
                        tmpRect.Width = 120;
                        tmpRect.Height = 20;
                        tmpRect.Stroke = Brushes.Blue;
                        tmpRect.RadiusX = 7;
                        tmpRect.RadiusY = 7;
                        RadialGradientBrush rgBrush = new RadialGradientBrush();
                        GradientStopCollection gsc = new GradientStopCollection();
                        gsc.Add(new GradientStop(Colors.White, 0));
                        gsc.Add(new GradientStop(Colors.Green, 1));
                        rgBrush.GradientStops = gsc;
                        rgBrush.GradientOrigin = new Point(0.1, 0.5);
                        rgBrush.RadiusX = 1;
                        rgBrush.RadiusY = 1;
                        tmpRect.Fill = rgBrush;
                        needle1.AddDisk(tmpRect);
                    }

                }

                else
                    MessageBox.Show("You need to pick 10 or less discs!\n Please press Restart :)");
                //startGame.IsEnabled = true;
            }
            else
                MessageBox.Show("Please choose a difficulty!");
        }

        /// <summary>
        ///timer for game
        /// </summary>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (started == true)
            {
                start_time -= 1;

                int minutes = start_time / 60;
                int second = start_time % 60;

                string sec = "";

                if (second < 10)
                {
                    sec = "0";
                    sec += second.ToString();
                }

                else
                    sec = second.ToString();

                string min = "";

                if (minutes < 10)
                {
                    min = "0";
                    min += minutes.ToString();
                }

                else
                    min = minutes.ToString();


                string total_time = min + ":" + sec;
                time.Content = total_time;
                
                //stop timer statement
                if (minutes == 0 && second == 0)
                {
                    started = false;
                    MessageBox.Show("you too slow bro");
                }

                    int remaining_time = int.Parse(min) * 60 + int.Parse(sec);
                    endGameScore = remaining_time * 150 / difficulty_multiplier;
                    scoreLabel.Content = endGameScore.ToString();
                  
                
            }
        }
       
        /// <summary>
        ///difficulty multiplier for score function. Although I no longer call score as the final score it is worked out 
        ///from the integer time, so the math will still be the same incrementally.
        /// </summary>
        private void easyDifficulty_Checked(object sender, RoutedEventArgs e)
        {
            difficulty_multiplier = 50;
        }

        private void mediumDifficulty_Checked(object sender, RoutedEventArgs e)
        {
            difficulty_multiplier = 25;
        }

        private void hardDifficulty_Checked(object sender, RoutedEventArgs e)
        {
            difficulty_multiplier = 10;
        }

        /// <summary>
        ///drag leave to a needle for needle1
        /// </summary>
        private void needle1_DragLeave(object sender, DragEventArgs e)
        {
            if (needle2_confirmation[1] == false && needle3_confirmation[1] == false)
                needle1_confirmation[1] = true;
        }

        /// <summary>
        ///drag leave to a needle for needle1
        /// </summary>
        private void needle3_DragLeave(object sender, DragEventArgs e)
        {
            if (needle1_confirmation[1] == false && needle2_confirmation[1] == false)
                needle3_confirmation[1] = true;
        }

        /// <summary>
        ///drag leave to a needle for needle1
        /// </summary>
        private void needle2_DragLeave(object sender, DragEventArgs e)
        {
            if (needle1_confirmation[1] == false && needle3_confirmation[1] == false)
                needle2_confirmation[1] = true;
        }

        /// <summary>
        /// A reset / restart button that clears all needles, clears the disc input and stops the timer.
        /// </summary>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
           
            needle3.IsEnabled = true;
            needle2.IsEnabled = true;
            needle1.IsEnabled = true;
            hardDifficulty.IsEnabled = true;
            mediumDifficulty.IsEnabled = true;
            easyDifficulty.IsEnabled = true;
            //clear the needles for new game
            needle1.Children.Clear();
            needle2.Children.Clear();
            needle3.Children.Clear();
            //empty the previous input for disc selection
            numDiscs.Text = String.Empty;
            //clear printout of moves from the solve function.
            textBlockResult.Text = String.Empty;

            //clear previous highscore
            nameTextBox.Text = "Anonymous";
            //stops the timer.
            started = false;
            //clears the timer
            time.Content = String.Empty;
            //clears the score
            scoreLabel.Content = String.Empty;
            //re enable start button.
            startGame.IsEnabled = true;
            //disable save button
            buttonStart.IsEnabled = false;
            saveButton.IsEnabled = false;
        }

        /// <summary>
        ///update scores function that checks if both needle 1 & 2 are empty and the endgamescore is greater than 0 do this
        /// </summary>
        public void updateScores()
        {

            //if needle 1 is empty and needle 2 is empty and the game score is more than 0 do this...
            if (needle1.Children.Count == 0 && needle2.Children.Count == 0 && endGameScore > 0)
            {
                nameTextBox.IsEnabled = true;

                if ((bool)easyDifficulty.IsChecked)
                {
                    started = false;
                    MessageBox.Show("High Score on easy! Please enter your name...");
                    //save button enabled and solve button disabled
                    saveButton.IsEnabled = true;
                    buttonStart.IsEnabled = false;
                    needle3.IsEnabled = false;
                    


                }
                else if ((bool)mediumDifficulty.IsChecked)
                {
                    started = false;
                    MessageBox.Show("High Score on medium! Please enter your name...");
                    
                    //error handle so cannot solve mid game
                    string username = nameTextBox.Text;

                    //save button enabled and solve button disabled
                    saveButton.IsEnabled = true;
                    buttonStart.IsEnabled = false;
                    needle3.IsEnabled = false;

                }
                else if ((bool)hardDifficulty.IsChecked)
                {
                    started = false;
                    MessageBox.Show("High Score on hard! Please enter your name...");
                    
                    //error handle so cannot solve mid game
                    string username = nameTextBox.Text;

                    //save button enabled and solve button disabled
                    saveButton.IsEnabled = true;
                    buttonStart.IsEnabled = false;
                    needle3.IsEnabled = false;
                }
            }
        }

        /// <summary>
        ///save high score button event handler
        /// </summary>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // StreamWriter swriter = new StreamWriter("Scores.txt", append: true);
            nameTextBox.IsEnabled = false;
            string username = nameTextBox.Text;
            
            //disable save button to stop user hitting twice
            saveButton.IsEnabled = false;

            if (nameTextBox.Text == "")
            {
                username = "Anonymous";
            }

            string time = this.time.ToString();

            //so we know what difficulty you finished the game on.
            if ((bool)hardDifficulty.IsChecked)
            {

                difficulty = "Hard";

            }
            if ((bool)mediumDifficulty.IsChecked)
            {


                difficulty = "Medium";

            }
            if ((bool)easyDifficulty.IsChecked)
            {


                difficulty = "Easy";

            }      

           //swriter.WriteLine(username + ";" + " " + difficulty + ";" + " Time " + start_time + ";" + " Score " + endGameScore + ";");
            //swriter.WriteLine(username + ";" + " Time " + start_time + ";" + difficulty);
            ////MessageBox.Show("Well done " + username + score + difficulty);
            //swriter.Close();

            //if file does not exist create new file
            if (!(File.Exists("Scores.txt")))
            {
                if (username == null || username == "Anonymous")
                {
                    StreamWriter swriter = new StreamWriter("Scores.txt");
                    swriter.WriteLine("Anonymous" + " " + start_time + " " + difficulty);
                    swriter.Flush();
                    swriter.Close();
                    MessageBox.Show("Record Saved to new file");
                }

                else
                {
                    StreamWriter swriter = new StreamWriter("Scores.txt");
                    swriter.WriteLine(username + " " + start_time + " " + difficulty);   
                    swriter.Flush();
                    swriter.Close();
                    MessageBox.Show("Record Saved to new file");
                }
            }
            else
            {
                compareRecords();

            }
}
        
        /// <summary>
        ///compare records function to see if score is better tha previous attempt/ username is the same. 
        /// </summary>
        private void compareRecords()
        {
            int i = 0;
            string line;
            bool found = false;

            result.Clear();

            StreamReader file = new StreamReader("Scores.txt");
            while ((line = file.ReadLine()) != null)
            {
                result.Add(line);
            }

            file.Close();

            if (!(nameTextBox.Text == null || nameTextBox.Text == "Anonymous"))
            {
                for (i = 0; i < result.Count; i++)
                {
                    if (result[i].Contains(nameTextBox.Text) && result[i].Contains(difficulty))
                    {
                        string tempRecord = result[i];
                        if (result[i].Contains(start_time.ToString()))
                        {
                            found = true;
                            MessageBox.Show("Record already exists!");
                            break;
                        }
                        else
                        {
                            //splits result
                            int x = int.Parse(result[i].Split(' ')[1]);

                            //if x is less than start_time do this
                            if (x < start_time)
                            {
                                result[i] = null;

                                StreamWriter swriter = new StreamWriter("Scores.txt");
                                for (i = 0; i < result.Count; i++)
                                {
                                    if (result[i] == null)
                                    { }
                                    else
                                    {
                                        swriter.WriteLine(result[i]);
                                    }

                                }
                                swriter.WriteLine(nameTextBox.Text + " " + start_time + " " + difficulty);
                                //result = result.OrderByDescending(p => p).ToList();

                                //result.Sort();

                                swriter.Flush();
                                swriter.Close();
                                MessageBox.Show("New fastest time! Record Saved");
                                found = true;

                            }

                            else
                            {
                                //you didnt beat previous record, no IO write performed.
                                MessageBox.Show("You didn't beat your previous record!");
                                found = true;
                            }
                        }
                    }
                }
            }


            //if found is false do this:
            if (found == false)
            {
                if (nameTextBox.Text == null || nameTextBox.Text == "Anonymous")
                {
                    StreamWriter swriter = new StreamWriter("Scores.txt");
                    for (i = 0; i < result.Count; i++)
                    {
                        if (result[i] == null)
                        { }
                        else
                        {
                            swriter.WriteLine(result[i]);
                        }
                    }
                    swriter.WriteLine("Anonymous" + " " + start_time + " " + difficulty);

                    //result = result.OrderByDescending(p => p).ToList();
                    //result.Sort();

                    swriter.Flush();
                    swriter.Close();
                    MessageBox.Show("Record Saved");
                }

                else
                {
                    StreamWriter swriter = new StreamWriter("Scores.txt");
                    for (i = 0; i < result.Count; i++)
                    {
                        if (result[i] == null)
                        { }
                        else
                        {
                            swriter.WriteLine(result[i]);
                        }
                    }
                    swriter.WriteLine(nameTextBox.Text + " " + start_time + " " + difficulty);

                    //result.Sort();

                    swriter.Flush();
                    swriter.Close();
                    MessageBox.Show("Record Saved");
                }
            }
        }
    
        /// <summary>
        ///clear high score listbox
        /// </summary>
        private void clearScores_Click(object sender, RoutedEventArgs e)
        {
            Listbox_Read.Items.Clear();
            //LoadHighscores.IsEnabled = true;
        }
    
        
        /// <summary>
        ///displays high scores to label
        /// </summary>
        public void Button_Click_5(object sender, RoutedEventArgs e)
        {
            
            //clear any previous scores before printing new scores
            Listbox_Read.Items.Clear();

            StreamReader sReader = new StreamReader("Scores.txt");
          
            string s = null;
            while ((s = sReader.ReadLine()) != null)
               // result.Sort();
            Listbox_Read.Items.Add(s);
            sReader.Close();
        }

        
        /// <summary>
        /// search function to find your previous username score.
        /// </summary>
        private void scoreSearch_Click(object sender, RoutedEventArgs e)
        {
            Listbox_Read.Items.Clear();
            StreamReader sread = new StreamReader("Scores.txt");
            string s = null;
           // bool found = false;
            string username = searchTextBox.Text;
            while ((s = sread.ReadLine()) != null)
            {
                if (s.Contains(username))
                {
                    
                    Listbox_Read.Items.Add(s);
                   // found = true;
                   // return;

                }
            }
            sread.Close();
        }
    }


    /// <summary>
    /// tower of hanoi class, access and mutate function
    /// </summary>
    public class TowerOfHanoiState
    {
        public string FromNeedle
        {
            get;
            set;
        }
        public string ToNeedle
        {
            get;
            set;
        }

        public TowerOfHanoiState(string fromNeedle, string toNeedle)
        {
            FromNeedle = fromNeedle;
            ToNeedle = toNeedle;
        }




    }

    }






