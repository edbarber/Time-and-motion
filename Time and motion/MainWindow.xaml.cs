﻿using System;
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
using Microsoft.Win32;

namespace Time_and_motion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string MESSAGE_BOX_WARNING_TITLE = "Warning";
        private const string MESSAGE_BOX_ERROR_TITLE = "Error";

        private VM vm;

        public MainWindow()
        {
            InitializeComponent();

            vm = new VM();
            DataContext = vm;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                vm.FilePath = openFileDialog.FileName;
            }
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.Generate();
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == typeof(CustomException).Name)
                {
                    MessageBox.Show(ex.Message, MESSAGE_BOX_WARNING_TITLE, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(ex.Message, MESSAGE_BOX_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
