using UnityEngine;

namespace Hyperbolic.Math
{
    /// <summary>
    /// Vetor de 3 dimensões que utiliza doubles.
    /// </summary>
    public struct VectorD3
    {
        public double x, y, z;
        public VectorD3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return $"({this.x}, {this.y}, {this.z})";
        }

        /// <summary>
        /// Realiza subtração de vetores usando doubles.
        /// </summary>
        public static VectorD3 operator -(VectorD3 a, VectorD3 b)
        {
            return new VectorD3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        /// <summary>
        /// Retorna o ponto correspondente nas coordenadas do Unity
        /// onde o eixo Y corresponde a altura, usando floats.
        /// </summary>
        public Vector3 ToUnity()
        {
            return new Vector3((float)this.x, (float)this.z, (float)this.y);
        }
    }
}
