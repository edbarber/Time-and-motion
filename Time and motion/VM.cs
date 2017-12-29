using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Diagnostics;

namespace Time_and_motion
{
    public class VM : INotifyPropertyChanged
    {
        #region Constants
        private const int MIN_BALLS = 27;
        private const int MAX_BALLS = 127;
        private const string OUT_OF_RANGE_WARNING_FORMAT = "Please enter valid numbers between {0} and {1}!";
        private const int MIN_ARRAY_INDEX = 0;
        private const int MIN_ARRAY_INDEX_OFFSET = 1;
        private const int TERMINATION_VALUE = 0;
        private const int MIN_IENUMERABLE_COUNT = 0;
        private const string EMPTY_FILE_WARNING = "Please add values to your file!";
        private const string NO_FILE_WARNING = "Please specify a location of a valid file!";
        private const string NULL_REFERENCE_EXCEPTION = "ArgumentNullException";
        private const string DIRECTORY_NOT_FOUND_EXCEPTION = "DirectoryNotFoundException";
        private const string FILE_NOT_FOUND_EXCEPTION = "FileNotFoundException";
        private const string TERMINATION_AT_BEGINNING_OF_FILE_WARNING = "File has been terminated at the beginning. Was this intentional?";
        private const int MAX_MINUTE_INDICATOR_CAPACITY = 4;
        private const int MAX_FIVE_MINUTE_INDICATOR_CAPACITY = 11;
        private const int MAX_HOUR_INDICATOR_CAPACITY = 11;
        private const int MAX_HOUR_INDICATOR_CAPACITY_OFFSET = 12;
        private const int MINUTES_IN_A_DAY = 1440;
        private const string OUTPUT_FILE_NAME = "Output.txt";
        private const string OUTPUT_DIRECTORY_NAME = "Time and motion";
        private const string OUTPUT_LINES_FORMAT = "{0} ball{1} cycle after {2} day{3}.";
        private const string PLURAL = "s";
        private const int MIN_DAYS_OFFSET = 1;
        private const int MIN_DAYS = 0;
        private const int MIN_MINUTES = 0;
        private const int MIN_IENUMERABLE_COUNT_OFFSET = 1;
        #endregion

        #region GlobalVariables
        private string filePath;
        #endregion

        #region Properties
        public string FilePath
        {
            set { filePath = value; NotifyPropertyChanged(); }
            get { return filePath; }
        }
        #endregion

        #region ParseFile
        private List<Queue<Ball>> ParseFile()
        {
            StreamReader file = null;

            try
            {
                List<Queue<Ball>> startingBallQueues = new List<Queue<Ball>>();
                file = new StreamReader(FilePath);
                bool isFinished = false;
                int counter = MIN_IENUMERABLE_COUNT;
                string line;

                while ((line = file.ReadLine()) != null && !isFinished)
                {
                    int result = int.Parse(line);

                    if (result == TERMINATION_VALUE)
                    {
                        if (counter == MIN_IENUMERABLE_COUNT)
                        {
                            throw new CustomException(TERMINATION_AT_BEGINNING_OF_FILE_WARNING);
                        }

                        isFinished = true;
                    }
                    else if (result >= MIN_BALLS && result <= MAX_BALLS)
                    {
                        Queue<Ball> balls = new Queue<Ball>(result);

                        for (int i = MIN_ARRAY_INDEX; i < result; i++)
                        {
                            Ball ball = new Ball(balls.Count);
                            balls.Enqueue(ball);
                        }

                        startingBallQueues.Add(balls);
                        counter++;
                    }
                    else
                    {
                        throw new CustomException(string.Format(OUT_OF_RANGE_WARNING_FORMAT, MIN_BALLS, MAX_BALLS));
                    }
                }

                if (startingBallQueues.Count == MIN_IENUMERABLE_COUNT)
                {
                    throw new CustomException(EMPTY_FILE_WARNING);
                }

                return startingBallQueues;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                }
            }
        }
        #endregion

        #region Generate
        public void Generate()
        {
            try
            {
                List<Queue<Ball>> ballQueues = ParseFile();
                string[] output = new string[ballQueues.Count];

                for (int i = MIN_ARRAY_INDEX; i < ballQueues.Count; i++)
                {
                    // every minute remove ball from queue
                    // if minute indicator has 4 balls then remove 4 balls and add one and one to the five-minute indicator
                    // if five minute indicator has 11 balls then remove 11 balls and add one to the hour indicator 
                    // if hour indicator has 12 balls then remove 12 balls

                    Queue<Ball> ballQueue = ballQueues[i];
                    Stack<Ball> minuteIndicator = new Stack<Ball>();
                    Stack<Ball> fiveMinuteIndicator = new Stack<Ball>();
                    Stack<Ball> hourIndicator = new Stack<Ball>();
                    int initBallQueueCount = ballQueue.Count;
                    int days = MIN_DAYS;
                    int minutes = MIN_MINUTES;

                    do
                    {
                        minuteIndicator.Push(ballQueue.Dequeue());

                        if (minuteIndicator.Count > MAX_MINUTE_INDICATOR_CAPACITY)
                        {
                            for (int j = MIN_ARRAY_INDEX; j < MAX_MINUTE_INDICATOR_CAPACITY; j++)
                            {
                                ballQueue.Enqueue(minuteIndicator.Pop());
                            }

                            fiveMinuteIndicator.Push(minuteIndicator.Pop());
                        }

                        if (fiveMinuteIndicator.Count > MAX_FIVE_MINUTE_INDICATOR_CAPACITY)
                        {
                            for (int j = MIN_ARRAY_INDEX; j < MAX_FIVE_MINUTE_INDICATOR_CAPACITY; j++)
                            {
                                ballQueue.Enqueue(fiveMinuteIndicator.Pop());
                            }

                            hourIndicator.Push(fiveMinuteIndicator.Pop());
                        }

                        if (hourIndicator.Count > MAX_HOUR_INDICATOR_CAPACITY)
                        {
                            for (int j = MIN_ARRAY_INDEX; j < MAX_HOUR_INDICATOR_CAPACITY_OFFSET; j++)
                            {
                                ballQueue.Enqueue(hourIndicator.Pop());
                            }
                        }

                        minutes++;

                        if (minutes == MINUTES_IN_A_DAY)
                        {
                            days++;
                            minutes = MIN_MINUTES;
                        }
                    } while (!IsInitialOrder(ballQueue));

                    output[i] = FormatOutputLine(days, initBallQueueCount);
                }

                OutputFile(output);
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == NULL_REFERENCE_EXCEPTION || ex.GetType().Name == DIRECTORY_NOT_FOUND_EXCEPTION ||
                    ex.GetType().Name == FILE_NOT_FOUND_EXCEPTION)
                {
                    FilePath = null;
                    throw new CustomException(NO_FILE_WARNING);
                }

                throw;
            }
        }
        #endregion

        #region CheckOrderOfBallQueue
        private bool IsInitialOrder(Queue<Ball> ballQueue)
        {
            bool isInitialOrder = true;

            for (int i = MIN_ARRAY_INDEX; i < (ballQueue.Count - MIN_ARRAY_INDEX_OFFSET); i++)
            {
                int ballOrderDiff = ballQueue.ElementAt(i + MIN_ARRAY_INDEX_OFFSET).GetOrderIndex() -
                    ballQueue.ElementAt(i).GetOrderIndex() - Ball.VALID_ORDER_DIFF;

                if (ballOrderDiff != Ball.VALID_ORDER_DIFF)
                {
                    isInitialOrder = false;
                }
            }

            return isInitialOrder;
        }
        #endregion

        #region FormatOutputLine
        private string FormatOutputLine(int days, int ballCount)
        {
            string outputLine;

            if (days != MIN_DAYS_OFFSET && ballCount != MIN_IENUMERABLE_COUNT_OFFSET)
            {
                outputLine = string.Format(OUTPUT_LINES_FORMAT, new string[] {
                    ballCount.ToString(), PLURAL, days.ToString(), PLURAL });
            }
            else if (days != MIN_DAYS_OFFSET)
            {
                outputLine = string.Format(OUTPUT_LINES_FORMAT, new string[] {
                    ballCount.ToString(), string.Empty, days.ToString(), PLURAL });
            }
            else if (ballCount != MIN_IENUMERABLE_COUNT)
            {
                outputLine = string.Format(OUTPUT_LINES_FORMAT, new string[] {
                    ballCount.ToString(), PLURAL, days.ToString(), string.Empty});
            }
            else
            {
                outputLine = string.Format(OUTPUT_LINES_FORMAT, new string[] {
                    ballCount.ToString(), string.Empty, days.ToString(), string.Empty});
            }

            return outputLine;
        }
        #endregion

        #region OutputFile
        private void OutputFile(string[] values)
        {
            StreamWriter file = null;

            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string fullPath = Path.Combine(path, OUTPUT_DIRECTORY_NAME);

                Directory.CreateDirectory(fullPath);

                string filePath = Path.Combine(fullPath, OUTPUT_FILE_NAME);
                file = new StreamWriter(filePath, false);

                foreach (string value in values)
                {
                    file.WriteLine(value);
                }

                Process.Start(filePath);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                }
            }
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
