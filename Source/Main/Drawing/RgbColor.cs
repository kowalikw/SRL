using XnaColor = Microsoft.Xna.Framework.Color;
using WinColor = System.Drawing.Color;
using Microsoft.Xna.Framework;

namespace SRL.Main.Drawing
{
    public class RgbColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public RgbColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public XnaColor ToXnaColor(float intensity = 1)
        {
            //return new XnaColor(R, G, B) * intensity;
            return new XnaColor(new XnaColor(R, G, B) * intensity, intensity);
        }

        public WinColor ToWinColor(byte intensity = byte.MaxValue)
        {
            return WinColor.FromArgb(intensity, R, G, B);
        }
    }
}
