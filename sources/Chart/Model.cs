using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Chart
{
    public sealed class Model
    {
        private readonly Vertex[][] _graphs;
        public Vertex Size { get; private set; }

        public Model(IEnumerable<string> files)
        {
            var numberFormat = NumberFormatInfo.InvariantInfo;
            double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;

            _graphs = files
                .Select(file => File
                    .ReadLines(file)
                    .Select(line => line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries))
                    .Select(parts =>
                    {
                        var vx = new Vertex(
                            double.Parse(parts[2], numberFormat),
                            double.Parse(parts[3], numberFormat),
                            double.Parse(parts[4], numberFormat));

                        minX = Math.Min(minX, vx.X);
                        minY = Math.Min(minY, vx.Y);
                        minZ = Math.Min(minZ, vx.Z);

                        maxX = Math.Max(maxX, vx.X);
                        maxY = Math.Max(maxY, vx.Y);
                        maxZ = Math.Max(maxZ, vx.Z);

                        return vx;
                    })
                    .ToArray())
                .ToArray();

            foreach (Vertex[] graph in _graphs)
            {
                for (int i = 0; i < graph.Length; ++i)
                {
                    graph[i].X -= minX;
                    graph[i].Y -= minY;
                    graph[i].Z -= minZ;
                }
            }

            Size = new Vertex(maxX - minX, maxY - minY, maxZ - minZ);
        }

        public void Draw(Graphics gfx, int width, int height, Matrix matrix)
        {
            Matrix projection = Matrix.CreateProjection(0.0015);
            Matrix centerGraphMatrix = Matrix.CreateTranslate(-Size.X / 2, -Size.Y / 2, -Size.Z / 2);
            double minSize = Math.Min(width, height) / 1.8;
            Matrix scaleMatrix = Matrix.CrateScale(minSize / Size.X, minSize / Size.Y, minSize / Size.Z);
            Matrix centerScreenMatrix = Matrix.CreateTranslate(width / 2.0, height / 2.0, 0);
            matrix = centerGraphMatrix * scaleMatrix * matrix * projection * centerScreenMatrix;

            int idx = 0;
            int colorStep = 360 / _graphs.Length;
            foreach (Vertex[] graph in _graphs)
            {
                var vb = new VertexBuffer();
                vb = graph.Aggregate(vb, (current, vx) => current + vx);
                vb *= matrix;

                ++idx;
                Color color = Utils.HslToRgb(idx * colorStep / 360.0, 1.0, 0.5);
                using (var pen = new Pen(color))
                {
                    vb.DrawLineStrip(gfx, pen);
                }

                // Draw label

                using (var pen = new Pen(color, 5))
                {
                    gfx.DrawLine(pen, 10, idx * 20, 30, idx * 20);
                }

                gfx.DrawString("Series " + idx, SystemFonts.DefaultFont, Brushes.Black, 35, idx * 20 - SystemFonts.DefaultFont.Height / 2);
            }

            // Draw bounding box

            var box = new VertexBuffer();
            box.Add(new Vertex(0, 0, 0));
            box.Add(new Vertex(Size.X, 0, 0));

            box.Add(new Vertex(0, 0, Size.Z));
            box.Add(new Vertex(Size.X, 0, Size.Z));

            box.Add(new Vertex(0, Size.Y, 0));
            box.Add(new Vertex(Size.X, Size.Y, 0));

            box.Add(new Vertex(0, Size.Y, Size.Z));
            box.Add(new Vertex(Size.X, Size.Y, Size.Z));

            box.Add(new Vertex(0, 0, 0));
            box.Add(new Vertex(0, Size.Y, 0));

            box.Add(new Vertex(0, 0, Size.Z));
            box.Add(new Vertex(0, Size.Y, Size.Z));

            box.Add(new Vertex(Size.X, 0, 0));
            box.Add(new Vertex(Size.X, Size.Y, 0));

            box.Add(new Vertex(Size.X, 0, Size.Z));
            box.Add(new Vertex(Size.X, Size.Y, Size.Z));

            box.Add(new Vertex(0, 0, 0));
            box.Add(new Vertex(0, 0, Size.Z));

            box.Add(new Vertex(0, Size.Y, 0));
            box.Add(new Vertex(0, Size.Y, Size.Z));

            box.Add(new Vertex(Size.X, 0, 0));
            box.Add(new Vertex(Size.X, 0, Size.Z));

            box.Add(new Vertex(Size.X, Size.Y, 0));
            box.Add(new Vertex(Size.X, Size.Y, Size.Z));

            box *= matrix;
            box.DrawLines(gfx, Pens.Black);

            // Draw Axes

            using (var axesFont = new Font(SystemFonts.DefaultFont.FontFamily, 12f))
            {
                var vertex = new Vertex(Size.X, 0, 0);
                vertex *= matrix;
                gfx.DrawString("X", axesFont, Brushes.DarkBlue, (float)vertex.X, (float)vertex.Y);

                vertex = new Vertex(0, Size.Y, 0);
                vertex *= matrix;
                gfx.DrawString("Y", axesFont, Brushes.DarkBlue, (float)vertex.X, (float)vertex.Y);

                vertex = new Vertex(0, 0, Size.Z);
                vertex *= matrix;
                gfx.DrawString("Z", axesFont, Brushes.DarkBlue, (float)vertex.X, (float)vertex.Y);
            }

            // Draw grid
            const int count = 5;
            using (var gridFont = new Font(SystemFonts.DefaultFont.FontFamily, 10f))
            {
                for (int i = 0; i < count; ++i)
                {
                    var vertex = new Vertex(i * Size.X / count, 0, 0);
                    vertex *= matrix;
                    gfx.FillRectangle(Brushes.Black, (float)vertex.X - 1f, (float)vertex.Y - 1f, 3f, 3f);
                    gfx.DrawString((i * Size.X / count).ToString(CultureInfo.InvariantCulture), gridFont, Brushes.DarkBlue, (float)vertex.X, (float)vertex.Y);

                    vertex = new Vertex(0, i * Size.Y / count, 0);
                    vertex *= matrix;
                    gfx.FillRectangle(Brushes.Black, (float)vertex.X - 1f, (float)vertex.Y - 1f, 3f, 3f);
                    gfx.DrawString((i * Size.Y / count).ToString(CultureInfo.InvariantCulture), gridFont, Brushes.DarkBlue, (float)vertex.X, (float)vertex.Y);

                    vertex = new Vertex(0, 0, i * Size.Z / count);
                    vertex *= matrix;
                    gfx.FillRectangle(Brushes.Black, (float)vertex.X - 1f, (float)vertex.Y - 1f, 3f, 3f);
                    gfx.DrawString((i * Size.Z / count).ToString(CultureInfo.InvariantCulture), gridFont, Brushes.DarkBlue, (float)vertex.X, (float)vertex.Y);
                }
            }
        }
    }
}
