using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class ImageHelper
    {

        public static ImageFileFormat GetImageFormat(string imageFile)
        {
            var buffer = new byte[4];
            int length;
            using (var fs = File.OpenRead(imageFile))
            {
                length = fs.Read(buffer, 0, buffer.Length);
            }

            return GetImageFormat(buffer);
        }

        public static ImageFileFormat GetImageFormat(byte[] buffer)
        {
            if (buffer?.Length >= 4)
            {
                switch (buffer[0])
                {
                    case 0xFF:
                        if (buffer[1] == 0xD8)
                            return ImageFileFormat.JPG;
                        break;
                    case 0x89:
                        if (buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                            return ImageFileFormat.PNG;
                        break;
                    case 0x47:
                        if (buffer[1] == 0x49 && buffer[2] == 0x46)
                            return ImageFileFormat.GIF;
                        break;
                    case 0x0C:
                        if (buffer[1] == 0xED)
                            return ImageFileFormat.TIF;
                        break;
                    case 0x42:
                        if (buffer[1] == 0x4D)
                            return ImageFileFormat.BMP;
                        break;
                    case 0x52:
                        var str = Encoding.ASCII.GetString(buffer);
                        if (str.IndexOf("RIFF") == 0 && str.IndexOf("WEBP") == 8)
                            return ImageFileFormat.WEBP;
                        break;
                    default:
                        break;
                }
            }
            return ImageFileFormat.Unkown;
        }
    }

    public enum ImageFileFormat
    {
        Unkown,
        BMP,
        JPG,
        PNG,
        TIF,
        GIF,
        WEBP,
        //HEIC,
        //HEIF
    }
}
