using System;
using System.Drawing;

namespace Chart
{
    public static class Utils
    {
        private static double Hue(double p, double q, double h)
        {
            if (h < 0.0) h += 1.0;
            if (h > 1.0) h -= 1.0;
            if (h * 6.0 < 1.0) return p + (q - p) * h * 6.0;
            if (h * 2.0 < 1.0) return q;
            if (h * 3.0 < 2.0) return p + (q - p) * (2.0 / 3.0 - h) * 6.0;
            return p;
        }

        public static Color HslToRgb(double h, double s, double l)
        {
            double r, g, b;

            if (Math.Abs(s) <= double.Epsilon) {
                r = g = b = 0.0;
            }
            else {
                double q = l < 0.5
                    ? l * (s + 1.0)
                    : l + s - (l * s);

                double p = l * 2.0 - q;
                r = Hue(p, q, h + 1.0 / 3.0);
                g = Hue(p, q, h);
                b = Hue(p, q, h - 1.0 / 3.0);
            }

            return Color.FromArgb(255, (int)(r * 255), (int)(g * 255), (int)(b * 255));
        }
    }
}
