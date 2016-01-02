using XnaColor = Microsoft.Xna.Framework.Color;
using WinColor = System.Drawing.Color;

namespace SRL.Main.Drawing
{
    public class RgbColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public RgbColor(byte r, byte g, byte b, byte a = byte.MaxValue)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public RgbColor(XnaColor color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }

        public XnaColor ToXnaColor(float intensity = 1)
        {
            return new XnaColor(new XnaColor(R, G, B) * intensity, intensity);
        }

        public WinColor ToWinColor()
        {
            return WinColor.FromArgb(A, R, G, B);
        }

        public WinColor ToWinColor(byte intensity = byte.MaxValue)
        {
            return WinColor.FromArgb(intensity, R * intensity / byte.MaxValue, G * intensity / byte.MaxValue, B * intensity / byte.MaxValue);
        }
    }
}
