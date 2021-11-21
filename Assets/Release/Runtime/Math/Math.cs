using UnityEngine;
using MathD = System.Math;

namespace Hyperbolic.Math
{
    public class Math
    {
        /// <summary>
        /// Retorna o ponto correspondente nas coordenadas cartesianas
        /// onde o eixo Z corresponde a altura, usando doubles.
        /// </summary>
        public static VectorD3 NormalizeToCartesianCoordinates(Vector3 u)
        {
            return new VectorD3((double)u.x, (double)u.z, (double)u.y);
        }

        /// <summary>
        /// Retorna a Cotangente de x em radianos.
        /// </summary>
        public static double Cot(double x)
        {
            return  1.0 / MathD.Tan(x);
        }

        /// <summary>
        /// Retorna o ponto correspondente ao vértice projetado no Disco de Poincaré
        /// a partir do Modelo do Hiperbolóide de Minkowski.
        /// </summary>
        public static VectorD3 MinkowskiToPoincare(VectorD3 P)
        {
            VectorD3 P0 = new VectorD3(0.0, 0.0, -1.0);
            double intersectionHeight = MathD.Sqrt((P.x * P.x) + (P.y + P.y) + 1.0);
            VectorD3 Ps = new VectorD3(P.x, P.y, intersectionHeight);
            VectorD3 V = P - P0;
            double t = P0.z / V.z;
            double pX = P0.x + (t * V.x);
            double pY = P0.y + (t * V.y);
            return new VectorD3(pX, pY, 0.0);
        }

        /// <summary>
        /// Retorna o ponto correspondente ao vértice projetado no hiperbolóide
        /// de Minkowski a partir da projeção de Beltrami-Klein.
        /// </summary>
        public static VectorD3 BeltramiKleinToMinkowski(VectorD3 P)
        {
            double factor = MathD.Sqrt(1.0 / (1.0 - ((P.x * P.x) + (P.y * P.y))));
            double x1 = P.x * factor;
            double y1 = P.y * factor;
            double z1 = factor;
            return new VectorD3(x1, y1, z1);
        }

        /// <summary>
        /// Retorna o ponto correspondente à hipotenusa da região fundamental
        /// da tesselação {p, q} descrita por Dunham.
        /// </summary>
        public static VectorD3[] HypotenuseOfFundamentalRegionEndPoint(int p, int q)
        {
            double cosh2 = Cot(MathD.PI / (double) p) * Cot(MathD.PI / (double) q);
            double sinh2 = MathD.Sqrt(cosh2 * cosh2 - 1);

            double coshq = MathD.Cos(MathD.PI / (double) q) / MathD.Cos(MathD.PI / (double) p);
            double sinhq = MathD.Sqrt(coshq * coshq - 1);

            double rad2 = sinh2 / ( cosh2 + 1 );
            double x2pt = sinhq / ( coshq + 1 );

            double xqpt = MathD.Cos( MathD.PI / (double) p ) * rad2;
            double yqpt = MathD.Sin( MathD.PI / (double) p ) * rad2;

            //Debug.Log($"xqpt = {xqpt}, yqpt = {yqpt}");
            return new VectorD3[]{new VectorD3(xqpt, yqpt, 0f), new VectorD3(x2pt, 0f, 0f)};
        }

        /// <summary>
        /// Retorna a matriz de de translação hiperbólica ao longo do eixo X.
        /// </summary>
        public static MatrixD4x4 HyperTranslateX(double angle)
        {
            MatrixD4x4 m = MatrixD4x4.Identity();
            m.m11 = MathD.Cosh(angle);
            m.m21 = MathD.Sinh(angle);
            m.m12 = MathD.Sinh(angle);
            m.m22 = MathD.Cosh(angle);
            return m;
        }

        /// <summary>
        /// Retorna a matriz de translação hiperbólica ao longo do eixo Y.
        /// </summary>
        public static MatrixD4x4 HyperTranslateY(double angle)
        {
            MatrixD4x4 m = MatrixD4x4.Identity();
            m.m00 = MathD.Cosh(angle);
            m.m02 = MathD.Sinh(angle);
            m.m20 = MathD.Sinh(angle);
            m.m22 = MathD.Cosh(angle);
            return m;
        }

        /// <summary>
        /// Retorna a matriz de rotação afim em torno do eixo X.
        /// </summary>
        public static MatrixD4x4 RotateZ(double angle)
        {
            MatrixD4x4 m = MatrixD4x4.Identity();
            m.m00 = MathD.Cos(angle);
            m.m10 = MathD.Sin(angle);
            m.m01 = -MathD.Sin(angle);
            m.m11 = MathD.Cos(angle);
            return m;
        }
    }
}
