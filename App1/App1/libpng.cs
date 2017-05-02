using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace App1
{
    static class libpng
    {
        public const string PNG_LIBPNG_VER_STRING = "1.6.29";

        const string DllName = "libpng16";

        public const int PNG_COLOR_TYPE_RGB = 2;
        public const int PNG_COLOR_TYPE_RGB_ALPHA = 2 | 4;

        [DllImport(DllName)]
        public static extern IntPtr png_create_read_struct(string user_png_ver, IntPtr error_ptr, IntPtr error_fn, IntPtr warn_fn);

        [DllImport(DllName)]
        public static extern IntPtr png_destroy_read_struct(ref IntPtr png_ptr_ptr, ref IntPtr info_ptr_ptr, ref IntPtr end_info_ptr_ptr);

        [DllImport(DllName)]
        public static extern IntPtr png_create_info_struct(IntPtr png_ptr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReadDataFromInputStream(IntPtr png_ptr, IntPtr outBytes, int byteCountToRead);

        [DllImport(DllName)]
        public static extern IntPtr png_set_read_fn(IntPtr png_ptr, IntPtr io_ptr, ReadDataFromInputStream read_data_fn);

        [DllImport(DllName)]
        public static extern void png_read_info(IntPtr png_ptr, IntPtr info_ptr);

        [DllImport(DllName)]
        public static extern uint png_get_IHDR(IntPtr png_ptr, IntPtr info_ptr
            , ref int width, ref int height
            , ref int bit_depth, ref int color_type
            , ref int interlace_method, ref int compression_method, ref int filter_method);

        [DllImport(DllName)]
        public static extern int png_get_rowbytes(IntPtr png_ptr, IntPtr info_ptr);

        [DllImport(DllName)]
        public static extern void png_read_row(IntPtr png_ptr, Byte[] row, IntPtr display_row);
    }


    class PngReader: IDisposable
    {
        IntPtr png_ptr;
        IntPtr info_ptr;
        IntPtr NULL = IntPtr.Zero;

        public PngReader()
        {
            png_ptr = libpng.png_create_read_struct(libpng.PNG_LIBPNG_VER_STRING, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            if (png_ptr == IntPtr.Zero)
            {
                throw new Exception();
            }

            info_ptr = libpng.png_create_info_struct(png_ptr);
            if (info_ptr == IntPtr.Zero)
            {
                Dispose();
                throw new Exception();
            }
        }

        public int width = 0;
        public int height = 0;
        public int bitDepth = 0;
        public int colorType = -1;
        public int interlace_method = 0;
        public int compression_method = 0;
        public int filter_method = 0;
        public bool Read(Byte[] pngBytes)
        {
            int pos = 0;
            libpng.ReadDataFromInputStream callback = (p, pBuffer, size) =>
            {
                Marshal.Copy(pngBytes, pos, pBuffer, size);
                pos += size;
            };
            libpng.png_set_read_fn(png_ptr, IntPtr.Zero, callback);

            libpng.png_read_info(png_ptr, info_ptr);

            var retval = libpng.png_get_IHDR(png_ptr, info_ptr
               , ref width
               , ref height
               , ref bitDepth
               , ref colorType
               , ref interlace_method
               , ref compression_method
               , ref filter_method
               );
            if (retval != 1)
            {
                return false;
            }
            return true;
        }

        public async Task WriteAsync(Stream s)
        {
            switch (colorType)
            {
                case libpng.PNG_COLOR_TYPE_RGB:
                    {
                        throw new NotImplementedException();
                    }

                case libpng.PNG_COLOR_TYPE_RGB_ALPHA:
                    {
                        var bytesPerRow = libpng.png_get_rowbytes(png_ptr, info_ptr);
                        var rowData = new byte[bytesPerRow];
                        for (int rowIdx = 0; rowIdx < height; ++rowIdx)
                        {
                            libpng.png_read_row(png_ptr, rowData, IntPtr.Zero);
                            await s.WriteAsync(rowData, 0, rowData.Length);
                        }
                    }
                    break;

                default:
                    //PULSAR_ASSERT_MSG(false, "Invalid PNG ColorType enum value given.\n");
                    break;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。
                // libpng must free file info struct memory before we bail
                if (png_ptr != IntPtr.Zero)
                {
                    libpng.png_destroy_read_struct(ref png_ptr, ref info_ptr, ref NULL);
                    info_ptr = IntPtr.Zero;
                    png_ptr = IntPtr.Zero;
                }

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~PngImage() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
