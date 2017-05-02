using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace App1
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        async Task<byte[]> LoadData()
        {
            var uri = new Uri("ms-appx:///sample.png"
                , UriKind.Absolute);
            var fileToRead = await StorageFile.GetFileFromApplicationUriAsync(uri);

            var png = new List<byte>();
            {
                byte[] buffer = new byte[1024];
                using (var fileReader = new BinaryReader(await fileToRead.OpenStreamForReadAsync()))
                {
                    while (true)
                    {
                        int read = fileReader.Read(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            break;
                        }
                        png.AddRange(buffer.Take(read));
                    }
                }
            }

            return png.ToArray();
        }

        async void LoadImage()
        {
            var NULL = IntPtr.Zero;

            var data = await LoadData();

            using (var png = new PngReader())
            {
                // get PNG file info struct (memory is allocated by libpng)
                if (!png.Read(data))
                {
                    return;
                }

                var bitmap = new WriteableBitmap(png.width, png.height);
                // WriteableBitmap uses BGRA format which is 4 bytes per pixel.
                using (var s = bitmap.PixelBuffer.AsStream())
                {
                    await png.WriteAsync(s);
                }
                this.image.Source = bitmap;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();

            LoadImage();
        }
    }
}
