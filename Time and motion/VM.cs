using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

        private string filePath;

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
                // TODO: Add in logic to generate ball clock result
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
