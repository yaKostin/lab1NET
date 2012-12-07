using System;
using System.Windows;
using System.Windows.Controls;

namespace FreeFilterWPF
{
    /// <summary>
    /// Interaction logic for SetFilter.xaml
    /// </summary>
    public partial class SetFilter : Window
    {
        
        private int[,] values = new int[4, 4];
               
        private TextBox[,] txtbxFilterValues;

        ComboBox cmbBoxSizeOfFilter = new ComboBox();

        Button ok = new Button();

        Grid layoutRoot;

        ColumnDefinition[] column;
        RowDefinition[] row;

        ComboBoxItem[] cmbBoxItems;

        public bool correctFilter = false;
        
        private  int sizeOfFilter = 3;

        private void InitializeFormComponents()
        {
            ok.Content = "OK";

            ok.Height = 25;

            ok.Width = 30;

            ok.ToolTip = "Press to save filter";

            cmbBoxSizeOfFilter.ToolTip = "Change size of filter";

            cmbBoxItems = new ComboBoxItem[3];

            cmbBoxSizeOfFilter.Height = 25;

            for (int i = 0; i < 3; i++)
            {
                cmbBoxItems[i] = new ComboBoxItem();

                cmbBoxItems[i].Content = (i + 2).ToString();

                cmbBoxSizeOfFilter.Items.Add(cmbBoxItems[i]);
            }

            SetMatrixEnter();

            cmbBoxSizeOfFilter.Text = sizeOfFilter.ToString();

        }

        public SetFilter()
        {
            cmbBoxSizeOfFilter.SelectionChanged += new SelectionChangedEventHandler(comboBox1_SelectionChangedHandler);

            ok.Click += new RoutedEventHandler(btnOk_Clicked);

            this.Closing += new System.ComponentModel.CancelEventHandler(SetFilter_Closed);

            InitializeComponent();

            InitializeFormComponents();
            
        }

        private void SetMatrixEnter()
        {
            layoutRoot = new Grid();

            layoutRoot.Width = 60*sizeOfFilter;

            layoutRoot.Height = 60*sizeOfFilter;

            layoutRoot.HorizontalAlignment = HorizontalAlignment.Left;

            layoutRoot.VerticalAlignment = VerticalAlignment.Top;

            column = new ColumnDefinition[sizeOfFilter];

            row = new RowDefinition[sizeOfFilter + 1];

            
            //создаем контролы и добавляем в корневой элемент
            txtbxFilterValues = new TextBox[sizeOfFilter, sizeOfFilter]; //{ Text = "hello world!" };

            for (int i = 0; i < sizeOfFilter; i++)
            {
                for (int j = 0; j < sizeOfFilter; j++)
                {
                    column[i] = new ColumnDefinition();

                    column[i].Width = new GridLength(40);

                    layoutRoot.ColumnDefinitions.Add(column[i]);

                    row[j] = new RowDefinition();

                    row[j].Height = new GridLength(30);

                    layoutRoot.RowDefinitions.Add(row[j]);

                    txtbxFilterValues[i, j] = new TextBox();

                    txtbxFilterValues[i, j].FontSize = 15;

                    txtbxFilterValues[i, j].FontWeight = FontWeights.Bold;

                    Grid.SetRow(txtbxFilterValues[i, j], i);

                    Grid.SetColumn(txtbxFilterValues[i, j], j);

                }
            }

            Grid.SetRow(cmbBoxSizeOfFilter, sizeOfFilter );

            Grid.SetColumn(cmbBoxSizeOfFilter, 1);

            row[sizeOfFilter] = new RowDefinition();

            row[sizeOfFilter].Height = new GridLength(40);

            //layoutRoot.RowDefinitions.Add(row[sizeOfFilter]);

            Grid.SetRow(ok, sizeOfFilter);

            Grid.SetRowSpan(ok, 1);

            for (int i = 0; i < sizeOfFilter; i++)
            {
                for (int j = 0; j < sizeOfFilter; j++)
                {
                    layoutRoot.Children.Add(txtbxFilterValues[i, j]);
                }
            }

            layoutRoot.Children.Add(cmbBoxSizeOfFilter);

            layoutRoot.Children.Add(ok);

            //добавляем корневой элемент на форму
            this.Content = layoutRoot;
        }

        private void DisconnectObject()
        {
            layoutRoot.Children.Clear();
        }

        //
        //Занести значения из textboxОВ в массив значений фильтра
        //
        private void ReadValuesFromMatrix()
        {
            //values = new int[sizeOfFilter, sizeOfFilter];

            for (int i = 0; i < sizeOfFilter; i++)
            {
                for (int j = 0; j < sizeOfFilter; j++)
                {
                    values[i, j] = Int32.Parse(txtbxFilterValues[i, j].Text);
                }
            }
        }
        
        //
        //Вернуть матрицу-фильтр
        //
        public int[,] GetValuesOfFilter()
        {
            return values;
        }

        //
        //Нажата кнопка ОК, 
        //
        private void btnOk_Clicked(object sender, RoutedEventArgs e)
        {

            if (IsCorrectFilter() == true)
            {
                ReadValuesFromMatrix();
            }

            this.Close();

        }

        /// <summary>
        /// Handler of the changing size of filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectionChangedHandler(object sender, SelectionChangedEventArgs e)
        {
            sizeOfFilter = cmbBoxSizeOfFilter.SelectedIndex + 2;

            DisconnectObject();

            SetMatrixEnter();
        } 

        //
        //При закрытии окна с вводом фильтра, сохранить значения фильтра
        //
        private void SetFilter_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {

            e.Cancel = true;

            if (IsCorrectFilter() == false)
            {

                MessageBoxResult result = MessageBox.Show("Invalid filter. Do you want to exit without saving filter?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                switch (result)
                {
                    case MessageBoxResult.Yes :
                        //e.Cancel = false;
                        this.Hide();
                        break;
                }

            }
            else
            {
                //e.Cancel = false;
                this.Hide();
            }
            
        }

        //
        /// <summary>Input validation of the filter
        /// </summary>
        private bool IsCorrectFilter()
        {

            for (int i = 0; i < sizeOfFilter; i++)
            {
                for (int j = 0; j < sizeOfFilter; j++)
                {

                    try
                    {
                        values[i, j] = Int32.Parse(txtbxFilterValues[i, j].Text);
                    }

                    catch (ArgumentNullException e)
                    {
                        correctFilter = false;
                        return false;
                    }

                    catch (FormatException e)
                    {
                        correctFilter = false;
                        return false;
                    }
                                       
                }

            }

            correctFilter = true;

            return true;

        }

    }
}
