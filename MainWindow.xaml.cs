using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FreeFilter;
using Microsoft.Win32;

namespace FreeFilterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string fileName = "someString";

        bool savedImage = false;

        BitmapImage btmpImage;

        SetFilter setFilterForm;

        FreeFastFilter handler = new FreeFastFilter();

        public MainWindow()
        {
            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
           
            InitializeComponent();

            setFilterForm = new SetFilter();

            menuItemClose.ToolTip = "Close images";

            menuItemSave.ToolTip = "Save filtered image";

            menuItemOpen.ToolTip = "Open image for filtering";
        }

        private void OpenImageFromFile()
        {
            
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.Filter = "Image (*.jpg)|*.jpg";

            if (fileDialog.ShowDialog() == true)
            {
                fileName = fileDialog.FileName;

                btmpImage = new BitmapImage();

                btmpImage.BeginInit();

                btmpImage.CacheOption = BitmapCacheOption.OnLoad;

                btmpImage.DecodePixelWidth = (int)(Width * 0.5);

                btmpImage.UriSource = new Uri(fileName, UriKind.Relative);

                btmpImage.EndInit();                              

                image1.Stretch = Stretch.Uniform;                              
                                
                image1.Source = btmpImage;

                btnApplyFilter.IsEnabled = true;

                menuItemSave.IsEnabled = false;

                image2.Source = Bitmap2BitmapImage(FreeFilterWPF.Properties.Resources.imgForWaiting);

                menuItemClose.IsEnabled = true;

                savedImage = false;
                                
            }
        }
        

        private void CloseImage()
        {
            if (savedImage)
            {
                image1.Source = null;

                image2.Source = null;

                menuItemClose.IsEnabled = false;

                menuItemSave.IsEnabled = false;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Filtered image isn't saved. Do you want exit without saving?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    SaveOutputImage();
                }
                else
                {
                    image1.Source = null;

                    image2.Source = null;

                    menuItemClose.IsEnabled = false;

                    menuItemSave.IsEnabled = false;
                }
            }
        }

        private void ApplyFilter()
        {
            if (!setFilterForm.correctFilter)
            {
                MessageBox.Show("Invalid filter. Enter normal filter.", "Error in filter", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            btnApplyFilter.IsEnabled = false;

            menuItemClose.IsEnabled = false;

            menuItemOpen.IsEnabled = false;

            SortedList<string, object> filterList = new SortedList<string, object>();

            int[,] filterMatrix;

            filterMatrix = setFilterForm.GetValuesOfFilter();

            Bitmap src = BitmapImage2Bitmap(btmpImage);

            filterList.Add("filter", filterMatrix);
                        
            handler.init(filterList);

            handler.Source = src;

            ThreadStart threadForApplyFilterStart = new ThreadStart(delegate() {
                handler.startHandle(new ProgressDelegate(progress)); 
                Op_Completed(); 
            });
                        
            Thread threadForApplyFilter = new Thread(threadForApplyFilterStart);

            threadForApplyFilter.Start();                 
                       
        }


        Bitmap BitmapImage2Bitmap(BitmapImage bi)
        {

            MemoryStream ms = new MemoryStream();
            
            Stream s = File.Open(fileName, FileMode.Open);

            s.Position = 0;

            s.CopyTo(ms);

            Bitmap bm = new Bitmap(ms);

            return bm;

        } 


        BitmapImage Bitmap2BitmapImage(Bitmap bm)
        {

            MemoryStream ms = new MemoryStream();

            bm.Save(ms, ImageFormat.Bmp);

            ms.Position = 0;

            BitmapImage bi = new BitmapImage();

            bi.BeginInit();

            bi.StreamSource = ms;

            bi.EndInit();

            return bi;
            
        }

        private void Op_Completed()
        {

            DispatcherOperation dispOp =
                                    image1.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                                    new Action(
                                                delegate()
                                                {
                                                    btmpImage = Bitmap2BitmapImage(handler.Result);
                                                    
                                                    image2.Source = btmpImage;                                                                                                                                                                                                           

                                                    btnApplyFilter.IsEnabled = true;

                                                    menuItemOpen.IsEnabled = true;

                                                    menuItemClose.IsEnabled = true;
                                                   
                                                }
                                    ));

        }
        

        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void SaveOutputImage()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Image Files(*.jpg)|*.jpg;";

            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == true)
            {
                string filename = saveFileDialog1.FileName;

                this.Cursor = System.Windows.Input.Cursors.Wait;

                handler.Result.Save(filename);

                savedImage = true;
            }
        }

        private void progress(double percent)
        {
            DispatcherOperation dispOp =
                                        progressBar1.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                                        new Action(
                                        delegate()
                                        {

                                            progressBar1.Value = (int)percent;

                                            if (percent < 0)
                                            {
                                                MessageBox.Show(percent.ToString());
                                            }

                                            taskbarItemInfo.ProgressValue = (double)percent/100;

                                            if (progressBar1.Value == 100)
                                            {
                                                progressBar1.Value = 0;

                                                taskbarItemInfo.ProgressValue = 0;

                                                menuItemSave.IsEnabled = true;
                                                                                               
                                            }

                                        }
                                     ));

        }

        private void ItemOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenImageFromFile();
        }

        private void ItemClose_Click(object sender, RoutedEventArgs e)
        {
            CloseImage();

            btnApplyFilter.IsEnabled = false;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveOutputImage();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            setFilterForm.Show();
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        
    }
}
