using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CGeo;
using DColor = System.Drawing.Color;

namespace ComputationalGeometry
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var bmp = new Bitmap(100, 100);
            var superstructure = Triangulation.CreateSuperstructure(new CGeo.Point(0, 0), new CGeo.Point(10, 10));
            bmp.Draw(superstructure, DColor.AliceBlue, DColor.AliceBlue);
            image.Source = bmp.ToImageSource();            
        }        
    }
}
