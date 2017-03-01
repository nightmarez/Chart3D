using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Chart
{
    public sealed class VertexBuffer
    {
        private List<Vertex> _arr = new List<Vertex>();

        public void Add(Vertex vx)
        {
            _arr.Add(vx);
        }

        public static VertexBuffer operator +(VertexBuffer vb, Vertex vx)
        {
            vb.Add(vx);
            return vb;
        }

        public void Mult(Matrix matrix)
        {
            var result = new List<Vertex>(_arr.Count);
            result.AddRange(_arr.Select(vx => vx * matrix));
            _arr.Clear();
            _arr = result;
        }

        public static VertexBuffer operator *(VertexBuffer vb, Matrix matrix)
        {
            vb.Mult(matrix);
            return vb;
        }

        public void DrawLineStrip(Graphics gfx, Pen pen)
        {
            for (int i = 0; i < _arr.Count - 1; ++i)
                gfx.DrawLine(pen, (float)_arr[i].X, (float)_arr[i].Y, (float)_arr[i + 1].X, (float)_arr[i + 1].Y);
        }

        public void DrawLines(Graphics gfx, Pen pen)
        {
            var vxs = new List<Vertex>(2);

            foreach (Vertex vx in _arr)
            {
                vxs.Add(vx);

                if (vxs.Count > 1)
                {
                    gfx.DrawLine(pen, (float)vxs[0].X, (float)vxs[0].Y, (float)vxs[1].X, (float)vxs[1].Y);
                    vxs.Clear();
                }
            }
        }
    }
}
