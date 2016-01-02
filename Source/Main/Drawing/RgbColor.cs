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

        public RgbColor(float r, float g, float b, float a = 1)
        {
            R = (byte)(r * 255);
            G = (byte)(g * 255);
            B = (byte)(b * 255);
            A = (byte)(a * 255);
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
    }
}
