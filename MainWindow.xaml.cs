using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TechnicalAssignment
{
    /// <summary>
    /// Tasks 1,2,3,6,7,and 8 completed
    /// </summary>
    public partial class MainWindow : Window
    {
        Brush customColor;
        Random r = new Random();
        private Point startPoint;
        private Point offset;
        private Rectangle rect;
        UIElement dragObj = null;

        public MainWindow()
        {
            InitializeComponent();
        }
 
        //Click the Open button to upload your image.
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        //Click the Save button and save the image and drawing.
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Rect rect = new Rect(cnvImage.RenderSize);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Right,
                (int)rect.Bottom, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(cnvImage);
            try
            {
                var saveFileDialog = new SaveFileDialog()
                {
                    Filter = "Image Files (*.bmp, *.png, *.jpg)|*.bmp;*.png;*.jpg"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    BitmapEncoder pngEncoder = new PngBitmapEncoder();
                    pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        pngEncoder.Save(stream);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        //Single mouse1 click to delete a reactangle; 
        //Click mouse1 and drag to create and resize the rectangle.
        private void canvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            customColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255)));
            if (e.OriginalSource is Rectangle)
            {
                Rectangle activeRectangle = (Rectangle)e.OriginalSource;
                cnvImage.Children.Remove(activeRectangle);
            }
            else
            {
                startPoint = e.GetPosition(cnvImage);
                rect = new Rectangle
                {
                    Fill = customColor,
                    StrokeThickness = 3,
                    Stroke = Brushes.Black
                };

                Canvas.SetLeft(rect, Mouse.GetPosition(cnvImage).X);
                Canvas.SetTop(rect, Mouse.GetPosition(cnvImage).Y);
                rect.PreviewMouseRightButtonDown += rectPreviewMouseRightButtonDown;
                cnvImage.Children.Add(rect);

            }
        }

        //Click mouse2 to select the rectangle you want to reposition.
        private void rectPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            this.dragObj = sender as UIElement;
            this.offset = e.GetPosition(cnvImage);
            this.offset.X -= Canvas.GetTop(this.dragObj);
            this.offset.Y -= Canvas.GetLeft(this.dragObj);
            this.cnvImage.CaptureMouse();
        }

        //cnvMouseMove event to handle rectangle resizing and repositioning.
        private void canvasMouseMove(object sender, MouseEventArgs e)
        {
            if (rect != null)
            {
                var pos = e.GetPosition(cnvImage);

                var x = Math.Min(pos.X, startPoint.X);
                var y = Math.Min(pos.Y, startPoint.Y);

                var w = Math.Max(pos.X, startPoint.X) - x;
                var h = Math.Max(pos.Y, startPoint.Y) - y;

                rect.Width = w;
                rect.Height = h;

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
            }else if (this.dragObj != null)
            {
                var pos2 = e.GetPosition(sender as IInputElement);
                Canvas.SetLeft(this.dragObj, pos2.X - this.offset.X);
                Canvas.SetTop(this.dragObj, pos2.Y - this.offset.Y);

            }
 

        }
        //Complete the process of rectangle creation and reposition.
        private void canvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            rect = null;
            this.dragObj = null;
            this.cnvImage.ReleaseMouseCapture();
        }
    }
}
