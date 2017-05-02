using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        Byte[] LoadData()
        {
            var buffer = new List<Byte>();
            //var assembly = Assembly.GetExecutingAssembly();
            using (var stream = new FileStream("sample.png", FileMode.Open))
            {
                var tmp = new Byte[1024 * 1024];
                while (true)
                {
                    var readSize = stream.Read(tmp, 0, tmp.Length);
                    if (readSize == 0)
                    {
                        break;
                    }
                    buffer.AddRange(tmp);
                }
            }
            return buffer.ToArray();
        }

        void LoadImage()
        {
            var NULL = IntPtr.Zero;

            var bytes = LoadData();

            using (var png = new PngReader())
            {
                // get PNG file info struct (memory is allocated by libpng)
                if (!png.Read(bytes))
                {
                    return;
                }

                var buffer = new List<byte>();
                int rowLength=0;
                foreach(var row in png.GetRows())
                {
                    rowLength = row.Length;
                    buffer.AddRange(row);
                }
                var bitmap=BitmapSource.Create(png.width, png.height
                    , 96, 96
                    , PixelFormats.Bgra32
                    , null
                    , buffer.ToArray(), rowLength);

                this.image.Source = bitmap;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            LoadImage();
        }
    }
}
