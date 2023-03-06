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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Brush customColor;
        Random r = new Random();
        private Point startPoint;
        Point offset;
        private Rectangle rect;
        UIElement dragObj = null;

        public MainWindow()
        {
            InitializeComponent();
        }
 

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
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog()
                {
                    Filter = "Image Files (*.bmp, *.png, *.jpg)|*.bmp;*.png;*.jpg"
                };
                if (saveFileDialog.ShowDialog() == true)
                {

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgPhoto.Source));
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        encoder.Save(stream);
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

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
                    Height = 50,
                    Width = 50,
                    Fill = customColor,
                    StrokeThickness = 3,
                    Stroke = Brushes.Black
                };

                Canvas.SetLeft(rect, Mouse.GetPosition(cnvImage).X);
                Canvas.SetTop(rect, Mouse.GetPosition(cnvImage).Y);
                rect.PreviewMouseRightButtonDown += rectPreviewMouseLeftButtonDown;
                cnvImage.Children.Add(rect);

            }
        }
        private void rectPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.dragObj = sender as UIElement;
            this.offset = e.GetPosition(cnvImage);
            this.offset.X -= Canvas.GetTop(this.dragObj);
            this.offset.Y -= Canvas.GetLeft(this.dragObj);
            this.cnvImage.CaptureMouse();
        }


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

        private void canvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            rect = null;
            this.dragObj = null;
            this.cnvImage.ReleaseMouseCapture();
        }
    }
}
