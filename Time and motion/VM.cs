using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Time_and_motion
{
    public class VM : INotifyPropertyChanged
    {
        private const int MIN_BALLS = 27;
        private const int MAX_BALLS = 127;
        private const string OUT_OF_RANGE_ERROR_FORMAT = "Please enter valid numbers between {0} and {1}!";
        private const int MIN_ARRAY_INDEX = 0;
        private const int MIN_ARRAY_INDEX_OFFSET = 1;
        private const int TERMINATION_VALUE = 0;
        private const int INVALID_LIST_COUNT = 0;
        private const string EMPTY_FILE_ERROR = "Please add values to your file!";
        private const string TERMINATION_AT_BEGINNING_OF_FILE_ERROR = "File has been terminated at the beginning. Was this intentional?";
        private const int MIN_LIST_COUNTER_VALUE = 0;
        private const int MIN_INDICATOR_COUNT = 0;
        private const int MAX_MINUTE_INDICATOR_CAPACITY = 4;
        private const int MAX_FIVE_MINUTE_INDICATOR_CAPACITY = 11;
        private const int MAX_HOUR_INDICATOR_CAPACITY = 12;
        private const int MAX_HOUR_INDICATOR_CAPACITY_OFFSET = 1;
        private const int MINUTES_IN_A_DAY = 1440;

        private string filePath;

        // Seperate unused vars
        //private int ballQueueCount;
        //private int minuteIndicatorCount;
        //private int fiveMinuteIndicatorCount;
        //private int hourIndicatorCount;
        //private int days;

        public string FilePath
        {
            set { filePath = value; NotifyPropertyChanged(); }
            get { return filePath; }
        }

        public void Generate()
        {
            try
            {
                List<int> startingBallQueues = ParseFile();
                
                foreach (int startingBallQueue in startingBallQueues)
                {
                    // every minute remove ball from queue
                    // if minute indicator has 4 balls then remove 4 balls and add one and one to the five-minute indicator
                    // if five minute indicator has 11 balls then remove 11 balls and add one to the hour indicator 
                    // if hour indicator has 12 balls then remove 12 balls

                    int ballQueueCount = startingBallQueue;
                    int minuteIndicatorCount = MIN_INDICATOR_COUNT;
                    int fiveMinuteIndicatorCount = MIN_INDICATOR_COUNT;
                    int hourIndicatorCount = MIN_INDICATOR_COUNT;
                    int days = 0;
                    int minutes = 0;

                    do
                    {
                        ballQueueCount--;   // comment out to test

                        if (minuteIndicatorCount >= MAX_MINUTE_INDICATOR_CAPACITY)
                        {
                            minuteIndicatorCount -= MAX_MINUTE_INDICATOR_CAPACITY;
                            ballQueueCount += MAX_MINUTE_INDICATOR_CAPACITY;
                            fiveMinuteIndicatorCount++;
                        }
                        else
                        {
                            minuteIndicatorCount++;
                        }

                        if (fiveMinuteIndicatorCount >= MAX_FIVE_MINUTE_INDICATOR_CAPACITY)
                        {
                            fiveMinuteIndicatorCount -= MAX_FIVE_MINUTE_INDICATOR_CAPACITY;
                            ballQueueCount += MAX_FIVE_MINUTE_INDICATOR_CAPACITY;
                            hourIndicatorCount++;
                        }
                        else
                        {
                            fiveMinuteIndicatorCount++;
                        }

                        if (hourIndicatorCount >= MAX_HOUR_INDICATOR_CAPACITY)
                        {
                            hourIndicatorCount -= MAX_HOUR_INDICATOR_CAPACITY;
                            ballQueueCount += MAX_HOUR_INDICATOR_CAPACITY;// + MAX_HOUR_INDICATOR_CAPACITY_OFFSET;
                        }
                        else
                        {
                            hourIndicatorCount++;
                        }

                        minutes++;

                        if (minutes == MINUTES_IN_A_DAY)
                        {
                            days++;
                        }
                    } while (ballQueueCount != startingBallQueue);

                    MessageBox.Show(startingBallQueue + " balls cycle after " + days + " days.");
                }
            }
            catch
            {
                throw;
            }
        }

        #region ParseFile
        private List<int> ParseFile()
        {
            StreamReader file = null;

            try
            {
                List<int> startingBallQueues = new List<int>();
                file = new StreamReader(FilePath);
                bool isFinished = false;
                int counter = MIN_LIST_COUNTER_VALUE;
                string line;

                while ((line = file.ReadLine()) != null && !isFinished)
                {
                    int result = int.Parse(line);

                    if (result == TERMINATION_VALUE)
                    {
                        if (counter == INVALID_LIST_COUNT)
                        {
                            throw new CustomException(TERMINATION_AT_BEGINNING_OF_FILE_ERROR);
                        }

                        isFinished = true;
                    }
                    else if (result >= MIN_BALLS && result <= MAX_BALLS)
                    {
                        startingBallQueues.Add(result);
                        counter++;
                    }
                    else
                    {
                        throw new CustomException(string.Format(OUT_OF_RANGE_ERROR_FORMAT, MIN_BALLS, MAX_BALLS));
                    }
                }

                if (startingBallQueues.Count == INVALID_LIST_COUNT)
                {
                    throw new CustomException(EMPTY_FILE_ERROR);
                }

                return startingBallQueues;
            }
            catch
            {
                throw;
            }
            finally
            {
                file.Close();
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
