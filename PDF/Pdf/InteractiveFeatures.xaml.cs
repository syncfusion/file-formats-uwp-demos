#region Copyright Syncfusion Inc. 2001 - 2020
// Copyright Syncfusion Inc. 2001 - 2020. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Reflection;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using Syncfusion.Pdf.Tables;
using Syncfusion.Pdf.Interactive;
using Windows.UI;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Popups;
using Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace EssentialPdf
{
    #region Products
    public class CylceProducts
    {
        public string ProductID { get; set; }
        public string Product { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string TotalPrice { get; set; }
    }
    #endregion
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InteractiveFeatures : UserControl,IDisposable
    {
        public InteractiveFeatures()
        {
            this.InitializeComponent();
            if ((Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")))
            {
                this.grdMain.Margin = new Thickness(10, 10, 10, 10);
                this.grdMain.Padding = new Thickness(10, 0, 10, 0);
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //}
        # region Fields
        PdfDocument doc;
        PdfPage page;
        System.Drawing.Color white = System.Drawing.Color.FromArgb(255, 255, 255, 255);
        #endregion
        PdfLightTable table = null;
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            # region Field Definitions
            doc = new PdfDocument();
            doc.PageSettings.Margins.All = 0;
            doc.PageSettings.Size = new SizeF(PdfPageSize.A4.Width, 600);
            page = doc.Pages.Add();
            PdfGraphics g = page.Graphics;
            RectangleF rect = new RectangleF(0, 0, page.Graphics.ClientSize.Width, 100);

            PdfBrush whiteBrush = new PdfSolidBrush(white);
            PdfPen whitePen = new PdfPen(white, 5);
            PdfBrush purpleBrush = new PdfSolidBrush(System.Drawing.Color.FromArgb(255, 158, 0, 160));
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 25);
            System.Drawing.Color maroonColor = System.Drawing.Color.FromArgb(255, 188, 32, 60);
            System.Drawing.Color orangeColor = System.Drawing.Color.FromArgb(255, 255, 167, 73);
            #endregion

            #region Header
            g.DrawRectangle(purpleBrush, rect);
            g.DrawPie(whitePen, whiteBrush, new RectangleF(-20, 35, 700, 200), 20, -180);
            g.DrawRectangle(whiteBrush, new RectangleF(0, 99.5f, 700, 200));
            g.DrawString("Invoice", new PdfStandardFont(PdfFontFamily.TimesRoman, 24), PdfBrushes.White, new PointF(500, 10));
            Stream imageStream = typeof(InteractiveFeatures).GetTypeInfo().Assembly.GetManifestResourceStream("Syncfusion.SampleBrowser.UWP.Pdf.Pdf.Assets.AdventureCycle.jpg");
            g.DrawImage(PdfImage.FromStream(imageStream), new RectangleF(100, 70, 390, 130));
            #endregion

            #region Body

            //Invoice Number
            Random invoiceNumber = new Random();
            g.DrawString("Invoice No: " + invoiceNumber.Next().ToString(), new PdfStandardFont(PdfFontFamily.Helvetica, 14), new PdfSolidBrush(maroonColor), new PointF(50, 210));
            g.DrawString("Date: ", new PdfStandardFont(PdfFontFamily.Helvetica, 14), new PdfSolidBrush(maroonColor), new PointF(350, 210));

            //Current Date
            PdfTextBoxField textBoxField = new PdfTextBoxField(page, "date");
            textBoxField.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            textBoxField.Bounds = new RectangleF(384, 204, 150, 30);
            textBoxField.ForeColor = new PdfColor(maroonColor);
            textBoxField.ReadOnly = true;
            doc.Actions.AfterOpen = new PdfJavaScriptAction(@"var newdate = new Date(); 
            var thisfieldis = this.getField('date');  
            
            var theday = util.printd('dddd',newdate); 
            var thedate = util.printd('d',newdate); 
            var themonth = util.printd('mmmm',newdate);
            var theyear = util.printd('yyyy',newdate);  
            
            thisfieldis.strokeColor=color.transparent;
            thisfieldis.value = theday + ' ' + thedate + ', ' + themonth + ' ' + theyear ;");
            doc.Form.Fields.Add(textBoxField);

            //invoice table
            if (table != null)
            {
                this.table.BeginCellLayout -= table_BeginCellLayout;
            }
            table = new PdfLightTable();
            table.Style.ShowHeader = true;
            g.DrawRectangle(new PdfSolidBrush(System.Drawing.Color.FromArgb(238, 238, 238, 248)), new RectangleF(50, 240, 500, 140));

            //Header Style
            PdfCellStyle headerStyle = new PdfCellStyle();
            headerStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
            headerStyle.TextBrush = whiteBrush;
            headerStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
            headerStyle.BackgroundBrush = new PdfSolidBrush(orangeColor);
            headerStyle.BorderPen = new PdfPen(whiteBrush, 0);
            table.Style.HeaderStyle = headerStyle;

            //Cell Style
            PdfCellStyle bodyStyle = new PdfCellStyle();
            bodyStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            bodyStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Left);
            bodyStyle.BorderPen = new PdfPen(whiteBrush, 0);
            table.Style.DefaultStyle = bodyStyle;
            table.DataSource = GetProductReport();
            table.Columns[0].Width = 90;
            table.Columns[1].Width = 160;
            table.Columns[3].Width = 100;
            table.Columns[4].Width = 65;
            table.Style.CellPadding = 3;
            table.BeginCellLayout += table_BeginCellLayout;

            PdfLightTableLayoutResult result = table.Draw(page, new RectangleF(50, 240, 500, 140));

            g.DrawString("Grand Total:", new PdfStandardFont(PdfFontFamily.Helvetica, 12), new PdfSolidBrush(System.Drawing.Color.FromArgb(255, 255, 167, 73)), new PointF(result.Bounds.Right - 150, result.Bounds.Bottom));
            CreateTextBox(page, "GrandTotal", "Grand Total", new RectangleF(result.Bounds.Width - 15, result.Bounds.Bottom - 2, 66, 18), true, "");


            //Send to Server
            PdfButtonField sendButton = new PdfButtonField(page, "OrderOnline");
            sendButton.Bounds = new RectangleF(200, result.Bounds.Bottom + 70, 80, 25);
            sendButton.BorderColor = white;
            sendButton.BackColor = maroonColor;
            sendButton.ForeColor = white;
            sendButton.Text = "Order Online";
            PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
            submitAction.DataFormat = SubmitDataFormat.Html;
            sendButton.Actions.MouseUp = submitAction;
            doc.Form.Fields.Add(sendButton);

            //Order by Mail
            PdfButtonField sendMail = new PdfButtonField(page, "sendMail");
            sendMail.Bounds = new RectangleF(300, result.Bounds.Bottom + 70, 80, 25);
            sendMail.Text = "Order By Mail";
            sendMail.BorderColor = white;
            sendMail.BackColor = maroonColor;
            sendMail.ForeColor = white;

            // Create a javascript action.
            PdfJavaScriptAction javaAction = new PdfJavaScriptAction("address = app.response(\"Enter an e-mail address.\",\"SEND E-MAIL\",\"\");"
+ "var aSubmitFields = [];"
+ "for( var i = 0 ; i < this.numFields; i++){"
        + "aSubmitFields[i] = this.getNthFieldName(i);"
    + "}"
+ "if (address){ cmdLine = \"mailto:\" + address;this.submitForm(cmdLine,true,false,aSubmitFields);}");

            sendMail.Actions.MouseUp = javaAction;
            doc.Form.Fields.Add(sendMail);

            //Print
            PdfButtonField printButton = new PdfButtonField(page, "print");
            printButton.Bounds = new RectangleF(400, result.Bounds.Bottom + 70, 80, 25);
            printButton.BorderColor = white;
            printButton.BackColor = maroonColor;
            printButton.ForeColor = white;
            printButton.Text = "Print";
            printButton.Actions.MouseUp = new PdfJavaScriptAction("this.print (true); ");
            doc.Form.Fields.Add(printButton);

            Stream attachmentDocument = typeof(InteractiveFeatures).GetTypeInfo().Assembly.GetManifestResourceStream("Syncfusion.SampleBrowser.UWP.Pdf.Pdf.Assets.Product Catalog.pdf");
            PdfAttachment attachment = new PdfAttachment("Product Catalog.pdf", attachmentDocument);
            attachment.ModificationDate = DateTime.Now;
            attachment.Description = "Specification";
            doc.Attachments.Add(attachment);

            //Open Specification
            PdfButtonField openSpecificationButton = new PdfButtonField(page, "openSpecification");
            openSpecificationButton.Bounds = new RectangleF(50, result.Bounds.Bottom + 20, 87, 15);
            openSpecificationButton.TextAlignment = PdfTextAlignment.Left;
            openSpecificationButton.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            openSpecificationButton.BorderStyle = PdfBorderStyle.Underline;
            openSpecificationButton.BorderColor = orangeColor;
            openSpecificationButton.BackColor = new PdfColor(255, 255, 255);
            openSpecificationButton.ForeColor = orangeColor;
            openSpecificationButton.Text = "Open Specification";
            openSpecificationButton.Actions.MouseUp = new PdfJavaScriptAction("this.exportDataObject({ cName: 'Product Catalog.pdf', nLaunch: 2 });");
            doc.Form.Fields.Add(openSpecificationButton);

            RectangleF uriAnnotationRectangle = new RectangleF(page.Graphics.ClientSize.Width - 160, page.Graphics.ClientSize.Height - 30, 80, 20);
            PdfTextWebLink linkAnnot = new PdfTextWebLink();
            linkAnnot.Url = "http://www.adventure-works.com";
            linkAnnot.Text = "http://www.adventure-works.com";
            linkAnnot.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
            linkAnnot.Brush = PdfBrushes.White;
            linkAnnot.DrawTextWebLink(page, uriAnnotationRectangle.Location);
            #endregion

            #region Footer
            g.DrawRectangle(purpleBrush, new RectangleF(0, page.Graphics.ClientSize.Height - 100, page.Graphics.ClientSize.Width, 100));
            g.DrawPie(whitePen, whiteBrush, new RectangleF(-20, page.Graphics.ClientSize.Height - 250, 700, 200), 0, 180);
            #endregion
            MemoryStream stream = new MemoryStream();
            await doc.SaveAsync(stream);
            doc.Close(true);
            Save(stream, "InteractiveFeatures.pdf");
        }

        #region Helper Methods
        public static IEnumerable<CylceProducts> GetProductReport()
        {
            Stream xmlStream = typeof(InteractiveFeatures).GetTypeInfo().Assembly.GetManifestResourceStream("Syncfusion.SampleBrowser.UWP.Pdf.Pdf.Assets.AdventureWorkCycle.xml");

            using (StreamReader reader = new StreamReader(xmlStream, true))
            {
                return XElement.Parse(reader.ReadToEnd())
                    .Elements("Report")
                    .Select(c => new CylceProducts
                    {
                        ProductID = c.Element("ProductID").Value,
                        Product = c.Element("Product").Value,
                        Price = c.Element("Price").Value,
                        Quantity = c.Element("Quantity").Value,
                        TotalPrice = c.Element("TotalPrice").Value
                    });
            }
        }

        void table_BeginCellLayout(object sender, BeginCellLayoutEventArgs args)
        {
            if (args.CellIndex == 2 && args.RowIndex > -1)
            {
                CreateTextBox(page, "price" + args.RowIndex.ToString(), "Price", args.Bounds, true, args.Value);
                args.Skip = true;

            }
            else if (args.CellIndex == 3 && args.RowIndex == -1)
            {
                PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(new RectangleF(args.Bounds.Right - 18, args.Bounds.Top + 2, 1, 1),
                               "Please enter a validate interger between 1 to 50");
                popupAnnotation.Border.Width = 4;
                popupAnnotation.Open = false;
                popupAnnotation.Border.HorizontalRadius = 10;
                popupAnnotation.Border.VerticalRadius = 10;
                popupAnnotation.Icon = PdfPopupIcon.Comment;
                page.Annotations.Add(popupAnnotation);
            }
            else if (args.CellIndex == 3 && args.RowIndex > -1)
            {
                PdfTextBoxField textBoxField = new PdfTextBoxField(page, "quantity" + args.RowIndex.ToString());


                //Set properties to the textbox.
                textBoxField.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12); ;
                textBoxField.BorderColor = new PdfColor(white);
                textBoxField.BackColor = System.Drawing.Color.FromArgb(255, 238, 238, 248);
                textBoxField.Bounds = args.Bounds;
                textBoxField.Text = "0";
                PdfJavaScriptAction action = new PdfJavaScriptAction(@"event.rc = event.value > -1 && event.value < 51; 
                var f = this.getField('price" + args.RowIndex.ToString() + @"')
                var f1 = this.getField('quantity" + args.RowIndex.ToString() + @"')
                var f2 = this.getField('TotalPrice" + args.RowIndex.ToString() + @"')
                var f3 = this.getField('GrandTotal');
                if(!event.rc)
                {

                f1.fillColor=color.red;
                app.beep();
                }
                else
                {
                    f1.fillColor = color.transparent;
                    f2.value = f1.value * f.value;
                    f3.value = this.getField('TotalPrice0').value + this.getField('TotalPrice1').value + this.getField('TotalPrice2').value + this.getField('TotalPrice3').value + this.getField('TotalPrice4').value +this.getField('TotalPrice5').value;
                }");
                textBoxField.Actions.LostFocus = action;
                doc.Form.Fields.Add(textBoxField);
            }
            else if (args.CellIndex == 4 && args.RowIndex > -1)
            {
                CreateTextBox(page, "TotalPrice" + args.RowIndex.ToString(), "Total Price", args.Bounds, true, "0");
            }
        }
        /// <summary>
        /// Creates textbox and adds it in the form.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="text"></param>
        /// <param name="tooltip"></param>
        /// <param name="f"></param>
        /// <param name="bounds"></param>
        private void CreateTextBox(PdfPage page, string text, string tooltip, RectangleF bounds, bool readOnly, string value)
        {
            // Create a Text box field.
            PdfTextBoxField textBoxField = new PdfTextBoxField(page, text);
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            //Set properties to the textbox.
            textBoxField.Font = font;
            textBoxField.BackColor = System.Drawing.Color.FromArgb(255, 238, 238, 248);
            textBoxField.BorderColor = white;
            textBoxField.Bounds = bounds;
            textBoxField.ToolTip = tooltip;
            textBoxField.ReadOnly = readOnly;
            textBoxField.Text = value;
            doc.Form.Fields.Add(textBoxField);
        }
        async void Save(Stream stream, string filename)
        {

            stream.Position = 0;

            StorageFile stFile;
            if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")))
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.DefaultFileExtension = ".pdf";
                savePicker.SuggestedFileName = "Sample";
                savePicker.FileTypeChoices.Add("Adobe PDF Document", new List<string>() { ".pdf" });
                stFile = await savePicker.PickSaveFileAsync();
            }
            else
            {
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                stFile = await local.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            }
            if (stFile != null)
            {
                Windows.Storage.Streams.IRandomAccessStream fileStream = await stFile.OpenAsync(FileAccessMode.ReadWrite);
                Stream st = fileStream.AsStreamForWrite();
				st.SetLength(0);
                st.Write((stream as MemoryStream).ToArray(), 0, (int)stream.Length);
                st.Flush();
                st.Dispose();
                fileStream.Dispose();
            }

            MessageDialog msgDialog = new MessageDialog("Do you want to view the Document?", "File created.");
            UICommand yesCmd = new UICommand("Yes");
            msgDialog.Commands.Add(yesCmd);
            UICommand noCmd = new UICommand("No");
            msgDialog.Commands.Add(noCmd);
            IUICommand cmd = await msgDialog.ShowAsync();
            if (cmd == yesCmd)
            {
                // Launch the retrieved file
                bool success = await Windows.System.Launcher.LaunchFileAsync(stFile);
            }
        }
        #endregion
        public void Dispose()
        {
            if (table != null)
            {
                this.table.BeginCellLayout -= table_BeginCellLayout;
            }
        }
    }
}
