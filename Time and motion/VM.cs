using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Time_and_motion
{
    public class VM : INotifyPropertyChanged
    {
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
                ReadFile();
            }
            catch
            {
                throw;
            }

            // TODO: Add in logic to generate ball clock result
        }

        private void ReadFile()
        {
            try
            {
                // TODO: Add in logic to read .txt file
            }
            catch
            {
                throw;
            }
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
