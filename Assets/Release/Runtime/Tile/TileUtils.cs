using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hyperbolic.Math;

namespace Hyperbolic.Tile
{
    public class TileUtils
    {
        /// <summary>
        /// Retorna a posição global a partir de uma posição local assumindo
        /// um GameObject como referência e uma escala.
        /// </summary>
        public static Vector3 LocalToWorld(GameObject obj, Vector3 u, Vector3 scale)
        {
            Matrix4x4 localToWorld = obj.transform.localToWorldMatrix;
            Vector3 normalized = localToWorld.MultiplyPoint3x4(u);
            Matrix4x4 localNormalized = new Matrix4x4();
            localNormalized.m00 = normalized.x;
            localNormalized.m11 = normalized.y;
            localNormalized.m22 = normalized.z;
            return localNormalized * scale;
        }

        /// <summary>
        /// Retorna a posição local a partir de uma posição global assumindo
        /// um GameObject como referência e uma escala.
        /// </summary>
        public static Vector3 WorldToLocal(GameObject obj, Vector3 u, Vector3 scale)
        {
            Vector3 inverseScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
            Matrix4x4 localToWorld = obj.transform.localToWorldMatrix;
            Vector3 normalized = localToWorld.MultiplyPoint3x4(u);
            Matrix4x4 localNormalized = new Matrix4x4();
            localNormalized.m00 = normalized.x;
            localNormalized.m11 = normalized.y;
            localNormalized.m22 = normalized.z;
            return localNormalized * inverseScale;
        }

        /// <summary>
        /// Projeção no Hiperbolóide de Minkowski de um plano que está no modelo Beltrami-Klein  
        /// utilizando um GameObject como referência global.
        /// </summary>
        public static void MeshKleinToMinkowski(GameObject meshObject, GameObject transformReference)
        {
            Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 worldV = LocalToWorld(transformReference, mesh.vertices[i], meshObject.transform.localScale);
                Vector3 worldVUp = new Vector3(worldV.x, worldV.z, worldV.y);
                VectorD3 worldVCartesian = Math.Math.NormalizeToCartesianCoordinates(worldVUp);
                VectorD3 result = Math.Math.BeltramiKleinToMinkowski(worldVCartesian);
                Vector3 res = result.ToUnity();
                vertices[i] = WorldToLocal(transformReference, new Vector3(res.x, res.z, res.y), meshObject.transform.localScale);
            }
            mesh.vertices = vertices;
        }

        /// <summary>
        /// Movimentação discreta no Hiperbolóide de Minkowski. Por conta da holonomia, movimentar
        /// no eixo x e depois no eixo y resulta em uma posição diferente de movimentar primeiro no
        /// eixo y e depois no eixo x. 
        /// </summary>
        public static void MoveDiscrete(GameObject meshObject, float x, float y, bool xFirst)
        {
            Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            for (var i = 0; i < vertices.Length; i++)
            {
                MatrixD4x4 result = MatrixD4x4.Identity();
                if (x == 0 && y == 0) result = MatrixD4x4.Identity();
                if (x == 0) result = Math.Math.HyperTranslateY(y);
                else if (y == 0) result = Math.Math.HyperTranslateX(x);
                else if (xFirst) result = Math.Math.HyperTranslateX(x) * Math.Math.HyperTranslateY(y);
                else if (!xFirst) result = Math.Math.HyperTranslateY(y) * Math.Math.HyperTranslateX(x);
                VectorD3 currentPos = new VectorD3(vertices[i].x, vertices[i].y, vertices[i].z);
                VectorD3 resultPos = result * currentPos;
                Vector3 resultFloat = resultPos.ToUnity();
                vertices[i] = new Vector3(resultFloat.x, resultFloat.z, resultFloat.y);
            }
            mesh.vertices = vertices;
        }

        /// <summary>
        /// Movimentação contínua no Hiperbolóide de Minkowski. Utilizando o eixo horizontal para rotacionar
        /// e o vertical para seguir em frente ou retornar.
        /// </summary>
        public static void Move(GameObject meshObject, float speed, GameObject transformReference)
        {
            float x = (float)Input.GetAxis("Horizontal") * -speed * (float)Time.deltaTime;
            float y = (float)Input.GetAxis("Vertical") * -speed * (float)Time.deltaTime;

            Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                MatrixD4x4 result;
                Vector3 worldV = LocalToWorld(transformReference, mesh.vertices[i], meshObject.transform.localScale);
                result = Math.Math.RotateZ(x * 2) * Math.Math.HyperTranslateY(y);
                VectorD3 currentPos = new VectorD3(vertices[i].x, vertices[i].y, vertices[i].z);
                VectorD3 resultPos = result * currentPos;
                Vector3 resultFloat = resultPos.ToUnity();
                vertices[i] = new Vector3(resultFloat.x, resultFloat.z, resultFloat.y);
            }
            mesh.vertices = vertices;
        }
    }
}
