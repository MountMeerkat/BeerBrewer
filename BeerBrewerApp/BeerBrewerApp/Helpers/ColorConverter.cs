using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerBrewerApp.Helpers
{
    public static class ColorConverter
    {
        public static byte[] ConvertHexToRgbArray(string hex)
        {
            int offset = 0;
            byte[] octets = new byte[3];

            if (hex.ToUpper().Substring(0, 1) == "#")
            {
                offset = 1;
            }

            for (int i = 0; i < hex.Length - offset; i += 2)
            {
                octets[i / 2] = byte.Parse(hex.Substring(i + offset, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return octets;
        }

        public static SKColor ConvertHexToSkColor(string hex)
        {
            byte[] octets = ConvertHexToRgbArray(hex);
            return new SKColor(octets[0], octets[1], octets[2]);
        }
    }
}
