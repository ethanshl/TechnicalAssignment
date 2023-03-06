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

        private void AddRectangle(object sender, MouseButtonEventArgs e)
        {
            customColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255)));

            if (e.OriginalSource is Rectangle)
            {
                this.dragObj = sender as UIElement;
                this.offset = e.GetPosition(cnvImage);
                this.offset.X -= Canvas.GetTop(this.dragObj);
                this.offset.Y -= Canvas.GetLeft(this.dragObj);
                this.cnvImage.CaptureMouse();


            }
            else
            {
                startPoint = e.GetPosition(cnvImage);
                rect = new Rectangle
                {
                    Width = 50,
                    Height = 50,
                    Fill = customColor,
                    StrokeThickness = 3,
                    Stroke = Brushes.Black
                };

                Canvas.SetLeft(rect, Mouse.GetPosition(cnvImage).X);
                Canvas.SetTop(rect, Mouse.GetPosition(cnvImage).Y);

                cnvImage.Children.Add(rect);

            }

        }

        private void RemoveRectangle(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Rectangle)
            {
                Rectangle activeRectangle = (Rectangle)e.OriginalSource;
                cnvImage.Children.Remove(activeRectangle);
            }
        }

     

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
  
            if (this.dragObj == null)
                return;
            var pos2 = e.GetPosition(sender as IInputElement);
            Canvas.SetLeft(this.dragObj, pos2.X - this.offset.X);
            Canvas.SetTop(this.dragObj,pos2.Y - this.offset.Y);
            
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            rect = null;
            this.dragObj = null;
            this.cnvImage.ReleaseMouseCapture();
        }
    }
}
