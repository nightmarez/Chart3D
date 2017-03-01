using System;

namespace Chart
{
    public sealed class Matrix
    {
        private const int Size = 4;
        private readonly double[,] _arr = new double[Size, Size];

        public double this[int i, int j]
        {
            get { return _arr[j, i]; }
            set { _arr[j, i] = value; }
        }

        public static Matrix CreateIdentity()
        {
            var matrix = new Matrix();

            for (int i = 0; i < Size; ++i)
                matrix[i, i] = 1.0;

            return matrix;
        }

        public Matrix Mult(Matrix matrix)
        {
            var result = new Matrix();

            for (int j = 0; j < Size; ++j)
                for (int i = 0; i < Size; ++i)
                    for (int k = 0; k < Size; ++k)
                        result[i, j] += this[k, j] * matrix[i, k];

            return result;
        }

        public static Matrix operator *(Matrix matrix0, Matrix matrix1)
        {
            return matrix0.Mult(matrix1);
        }

        public static Matrix CreateTranslate(double x, double y, double z)
        {
            Matrix matrix = CreateIdentity();

            matrix[0, 3] = x;
            matrix[1, 3] = y;
            matrix[2, 3] = z;

            return matrix;
        }

        public static Matrix CrateScale(double x, double y, double z)
        {
            Matrix matrix = CreateIdentity();

            matrix[0, 0] = x;
            matrix[1, 1] = y;
            matrix[2, 2] = z;

            return matrix;
        }

        public static Matrix CreateRotateX(double angle)
        {
            Matrix matrix = CreateIdentity();

            angle *= Math.PI / 180.0;
            double angleSin = Math.Sin(angle);
            double angleCos = Math.Cos(angle);

            matrix[1, 1] = angleCos;
            matrix[1, 2] = angleSin;
            matrix[2, 1] = -angleSin;
            matrix[2, 2] = angleCos;

            return matrix;
        }

        public static Matrix CreateRotateY(double angle)
        {
            Matrix matrix = CreateIdentity();

            angle *= Math.PI / 180.0;
            double angleSin = Math.Sin(angle);
            double angleCos = Math.Cos(angle);

            matrix[0, 0] = angleCos;
            matrix[0, 2] = -angleSin;
            matrix[2, 0] = angleSin;
            matrix[2, 2] = angleCos;

            return matrix;
        }

        public static Matrix CreateRotateZ(double angle)
        {
            Matrix matrix = CreateIdentity();

            angle *= Math.PI / 180.0;
            double angleSin = Math.Sin(angle);
            double angleCos = Math.Cos(angle);

            matrix[0, 0] = angleCos;
            matrix[0, 1] = angleSin;
            matrix[1, 0] = -angleSin;
            matrix[1, 1] = angleCos;

            return matrix;
        }

        public static Matrix CreateProjection(double p)
        {
            Matrix matrix = CreateIdentity();

            matrix[3, 2] = p;

            return matrix;
        }
    }
}
