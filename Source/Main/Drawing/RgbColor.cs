using XnaColor = Microsoft.Xna.Framework.Color;

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
            R = (byte)(r * byte.MaxValue);
            G = (byte)(g * byte.MaxValue);
            B = (byte)(b * byte.MaxValue);
            A = (byte)(a * byte.MaxValue);
        }

        public RgbColor(XnaColor color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }

        public XnaColor ToXnaColor()
        {
            return new XnaColor(R, G, B, A);
        }


        public XnaColor ToXnaColor(float intensity = 1)
        {
            return new XnaColor(new XnaColor(R, G, B) * intensity, intensity);
        }
    }
}
