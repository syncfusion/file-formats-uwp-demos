#region Copyright Syncfusion Inc. 2001 - 2024
// Copyright Syncfusion Inc. 2001 - 2024. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
//using Common;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Common;
using Windows.UI.Xaml.Controls;
using System.Reflection;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Xml.Serialization;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace EssentialXlsIO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImportNestedCollection : SampleLayout
    {
        public ImportNestedCollection()
        {
            this.InitializeComponent();            
           
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
			{
                this.btnGenerateExcel.HorizontalAlignment = HorizontalAlignment.Center;
                this.grdMain.Padding = new Thickness(10, 0, 0, 0);
				this.textBox1.Text = "Click the button to view an Excel spreadsheet generated by Essential XlsIO. Please note that Microsoft Excel Viewer or Microsoft Excel is required to view the resultant document.";
			}
			else
            this.textBox1.Text = "Click the button to view an Excel spreadsheet generated by Essential XlsIO. Please note that Microsoft Excel Viewer or Microsoft Excel is required to view the resultant document.";

        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // ... A List.
            List<string> data = new List<string>();
            data.Add("Default");
            data.Add("Merge");
            data.Add("Repeat");

            // ... Get the ComboBox reference.
            var comboBox = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = data;

            // ... Make the first item selected.
            comboBox.SelectedIndex = 0;

            this.actionType.Visibility = Visibility.Collapsed;
            this.levelTextBox.Visibility = Visibility.Collapsed;
        }

        private async void btnGenerateExcel_Click(object sender, RoutedEventArgs e)
        {                        
            #region Workbook initialization
            //New instance of XlsIO is created.[Equivalent to launching MS Excel with no workbooks open].
            //The instantiation process consists of two steps.

            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;
            
            IWorkbook workbook = application.Workbooks.Create(1);
            //The first worksheet object in the worksheets collection is accessed.
            IWorksheet worksheet = workbook.Worksheets[0];

            IList<Brands> list = GetVehicleDetails();

            ExcelImportDataOptions importDataOptions = new ExcelImportDataOptions();
			importDataOptions.FirstRow = 4;
			
            if (comboBox1.SelectedIndex == 0)
                importDataOptions.NestedDataLayoutOptions = ExcelNestedDataLayoutOptions.Default;
            else if (comboBox1.SelectedIndex == 1)
                importDataOptions.NestedDataLayoutOptions = ExcelNestedDataLayoutOptions.Merge;
            else if (comboBox1.SelectedIndex == 2)
                importDataOptions.NestedDataLayoutOptions = ExcelNestedDataLayoutOptions.Repeat;

            if (checkbox1.IsChecked.Value)
            {
                if (rdbExpand.IsChecked.Value)
                {
                    importDataOptions.NestedDataGroupOptions = ExcelNestedDataGroupOptions.Expand;
                }
                else if (rdbCollapse.IsChecked.Value)
                {
                    importDataOptions.NestedDataGroupOptions = ExcelNestedDataGroupOptions.Collapse;
                    if (levelTextBox.Text != string.Empty)
                    {
                        importDataOptions.CollapseLevel = int.Parse(levelTextBox.Text);
                    }
                }
            }

            worksheet.ImportData(list, importDataOptions);

            #region Define Styles
            IStyle pageHeader = workbook.Styles.Add("PageHeaderStyle");
            IStyle tableHeader = workbook.Styles.Add("TableHeaderStyle");

            pageHeader.Font.FontName = "Calibri";
            pageHeader.Font.Size = 16;
            pageHeader.Font.Bold = true;
            pageHeader.Color = Windows.UI.Color.FromArgb(0, 146, 208, 80);
            pageHeader.HorizontalAlignment = ExcelHAlign.HAlignCenter;
            pageHeader.VerticalAlignment = ExcelVAlign.VAlignCenter;

            tableHeader.Font.Bold = true;
            tableHeader.Font.FontName = "Calibri";
            tableHeader.Color = Windows.UI.Color.FromArgb(0, 146, 208, 80);

            #endregion

            #region Apply Styles
            // Apply style for header
            worksheet["A1:C2"].Merge();
            worksheet["A1"].Text = "Automobile Brands in the US";
            worksheet["A1"].CellStyle = pageHeader;

            worksheet["A4:C4"].CellStyle = tableHeader;

            worksheet.Columns[0].ColumnWidth = 10;
            worksheet.Columns[1].ColumnWidth = 20;
            worksheet.Columns[2].ColumnWidth = 25;

            #endregion

            #endregion

            #region Save the Workbook
            StorageFile storageFile;
            if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")))
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
                savePicker.SuggestedFileName = "ImportData";
                savePicker.FileTypeChoices.Add("Excel Files", new List<string>() { ".xlsx", });
                storageFile = await savePicker.PickSaveFileAsync();
            }
            else
            {
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                storageFile = await local.CreateFileAsync("ImportData.xlsx", CreationCollisionOption.ReplaceExisting);
            }

            if (storageFile != null)
            {
                //Saving the workbook
                await workbook.SaveAsAsync(storageFile);
                workbook.Close();
                excelEngine.Dispose();

                MessageDialog msgDialog = new MessageDialog("Do you want to view the Document?", "File has been saved successfully.");

                UICommand yesCmd = new UICommand("Yes");
                msgDialog.Commands.Add(yesCmd);
                UICommand noCmd = new UICommand("No");
                msgDialog.Commands.Add(noCmd);
                IUICommand cmd = await msgDialog.ShowAsync();
                if (cmd == yesCmd)
                {
                    // Launch the saved file
                    bool success = await Windows.System.Launcher.LaunchFileAsync(storageFile);
                }
            }
            else
            {
                workbook.Close();
                excelEngine.Dispose();
            }
            #endregion
        }

        private void Checkbox1_Checked(object sender, RoutedEventArgs e)
        {
            if (this.checkbox1.IsChecked.Value)
                this.actionType.Visibility = Visibility.Visible;
            else
                this.actionType.Visibility = Visibility.Collapsed;
        }

        private void Rdb_Checked(object sender, RoutedEventArgs e)
        {
            if (this.rdbCollapse != null)
            {
                if (this.rdbCollapse.IsChecked.Value)
                    this.levelTextBox.Visibility = Visibility.Visible;
                else
                    this.levelTextBox.Visibility = Visibility.Collapsed;
            }
        }

        #region Dispose
        public override void Dispose()
        {
            base.Dispose();

            UnlinkChildrens(grdMain);
            DisposeTextBlock(textBox1);
            textBox1 = null;
            DisposeTextBlock(textBox2);
            textBox2 = null;
            DisposeTextBlock(textBox3);
            textBox3 = null;
            DisposeTextBlock(textBox4);
            textBox4 = null;
            DisposeTextBlock(textBox5);
            textBox5 = null;
            DisposeTextBlock(textBox6);
            textBox6 = null;

            DisposeButton(btnGenerateExcel);
            btnGenerateExcel.Click -= btnGenerateExcel_Click;
            btnGenerateExcel = null;      

            DisposeStackPanel(stackPnlOptions);
            stackPnlOptions = null;

            comboBox1 = null;

            DisposeGrid(grd1);
            grd1 = null;

            DisposeGrid(grdMain);
            grdMain = null;
        }

        private void DisposeTextBlock(TextBlock textBlock)
        {
            if (textBlock == null)
                return;
            textBlock.ClearValue(TextBlock.FontFamilyProperty);
            textBlock.ClearValue(TextBlock.FontSizeProperty);
            textBlock.ClearValue(TextBlock.TextProperty);
            textBlock.ClearValue(TextBlock.TextWrappingProperty);
            textBlock.ClearValue(TextBlock.ForegroundProperty);
        }

        private void DisposeButton(Button button)
        {
            if (button == null)
                return;
            button.ClearValue(Button.FontFamilyProperty);
            button.ClearValue(Button.FontSizeProperty);
            button.ClearValue(Button.PaddingProperty);
            button.ClearValue(Button.ForegroundProperty);
            button.ClearValue(Button.BackgroundProperty);
            button.ClearValue(Button.ContentProperty);
            button.ClearValue(Button.HeightProperty);
            button.ClearValue(Button.WidthProperty);            
        }

        private void DisposeRadioButton(RadioButton radioButton)
        {
            if (radioButton == null)
                return;
            radioButton.ClearValue(RadioButton.GroupNameProperty);
            radioButton.ClearValue(RadioButton.ContentProperty);
            radioButton.ClearValue(RadioButton.FontFamilyProperty);
            radioButton.ClearValue(RadioButton.FontSizeProperty);
            radioButton.ClearValue(RadioButton.ForegroundProperty);
            radioButton.ClearValue(RadioButton.WidthProperty);
            radioButton.ClearValue(RadioButton.IsCheckedProperty);
        }

        private void DisposeStackPanel(StackPanel stackPanel)
        {
            if (stackPanel == null)
                return;
            stackPanel.ClearValue(StackPanel.OrientationProperty);
            stackPanel.ClearValue(StackPanel.HorizontalAlignmentProperty);
        }

        private void DisposeGrid(Grid grid)
        {
            if (grid == null)
                return;
            grid.ClearValue(Grid.BackgroundProperty);
            grid.ClearValue(Grid.MarginProperty);
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
        }

        void UnlinkChildrens(UIElement element)
        {
            if (element == null)
                return;
            if (element is Panel)
            {
                for (int i = 0; i < (element as Panel).Children.Count; i++)
                {
                    UIElement childElement = (element as Panel).Children[i];
                    UnlinkChildrens(childElement);
                    (element as Panel).Children.Remove(childElement);
                    i--;
                }
            }
            else if (element is ItemsControl)
            {
                for (int j = 0; j < (element as ItemsControl).Items.Count; j++)
                {
                    UIElement childElement = ((element as ItemsControl).Items[j] as UIElement);
                    if (childElement == null)
                    {
                        //(element as ItemsControl).Items.RemoveAt(j);
                        //j--;
                    }
                    else
                    {
                        UnlinkChildrens(childElement);
                        (element as ItemsControl).Items.Remove(childElement);
                        j--;
                    }
                }
            }
            else if (element is ContentControl)
            {
                UnlinkChildrens((element as ContentControl).Content as UIElement);
                (element as ContentControl).Content = null;
            }
            else if (element is UserControl)
            {
                UnlinkChildrens((element as UserControl).Content as UIElement);
                (element as UserControl).Content = null;
            }
        }
        #endregion

        #region Helper Methods
        private IList<Brands> GetVehicleDetails()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(BrandObjects));
            Assembly assembly = typeof(TemplateMarker).GetTypeInfo().Assembly;
            string resourcePath = "Syncfusion.SampleBrowser.UWP.XlsIO.XlsIO.Tutorials.Samples.Assets.Resources.Templates.ExportData.xml";
            Stream fileStream = assembly.GetManifestResourceStream(resourcePath);
            TextReader textReader = new StreamReader(fileStream);
            BrandObjects brands = (BrandObjects)deserializer.Deserialize(textReader);

            List<Brands> list = new List<Brands>();
            string brandName = brands.BrandObject[0].BrandName;
            string vehicleType = brands.BrandObject[0].VahicleType;
            string modelName = brands.BrandObject[0].ModelName;
            Brands brand = new Brands(brandName);
            brand.VehicleTypes = new List<VehicleTypes>();

            VehicleTypes vehicle = new VehicleTypes(vehicleType);
            vehicle.Models = new List<Model>();
            Model model = new Model(modelName);

            brand.VehicleTypes.Add(vehicle);
            list.Add(brand);

            foreach (BrandObject brandObj in brands.BrandObject)
            {
                if (brandName == brandObj.BrandName)
                {
                    if (vehicleType == brandObj.VahicleType)
                    {
                        vehicle.Models.Add(new Model(brandObj.ModelName));
                        continue;
                    }
                    else
                    {
                        vehicle = new VehicleTypes(brandObj.VahicleType);
                        vehicle.Models = new List<Model>();
                        vehicle.Models.Add(new Model(brandObj.ModelName));
                        brand.VehicleTypes.Add(vehicle);
                        vehicleType = brandObj.VahicleType;
                    }
                    continue;
                }
                else
                {
                    brand = new Brands(brandObj.BrandName);
                    vehicle = new VehicleTypes(brandObj.VahicleType);
                    vehicle.Models = new List<Model>();
                    vehicle.Models.Add(new Model(brandObj.ModelName));
                    brand.VehicleTypes = new List<VehicleTypes>();
                    brand.VehicleTypes.Add(vehicle);
                    vehicleType = brandObj.VahicleType;
                    list.Add(brand);
                    brandName = brandObj.BrandName;
                }
            }
            textReader.Close();
            return list;
        }
        #endregion
     
    }
    #region Helper Class
    [XmlRootAttribute("BrandObjects")]
    public class BrandObjects
    {
        [XmlElement("BrandObject")]
        public BrandObject[] BrandObject { get; set; }
    }
    public class BrandObject
    {
        public string BrandName { get; set; }
        public string VahicleType { get; set; }
        public string ModelName { get; set; }
    }
    //[Serializable]
    public class Brands
    {
        private string m_brandName;
        [DisplayNameAttribute("Brand")]
        public string BrandName
        {
            get { return m_brandName; }
            set { m_brandName = value; }
        }

        private IList<VehicleTypes> m_vehicles;
        public IList<VehicleTypes> VehicleTypes
        {
            get { return m_vehicles; }
            set { m_vehicles = value; }
        }

        public Brands(string brandName)
        {
            m_brandName = brandName;
        }
    }

    public class VehicleTypes
    {
        private string m_vehicle;
        [DisplayNameAttribute("Vehicle Type")]
        public string Vehicle
        {
            get { return m_vehicle; }
            set { m_vehicle = value; }
        }

        private IList<Model> m_models;
        public IList<Model> Models
        {
            get { return m_models; }
            set { m_models = value; }
        }

        public VehicleTypes(string vehicle)
        {
            m_vehicle = vehicle;
        }
    }

    #endregion
    
}
