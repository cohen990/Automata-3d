using UnityEngine;

namespace Terrain.Mesh
{
    public class DecomposedCornerBuffer
    {
        public readonly Vector3[] Vertices;
        public readonly Vector3[] Normals;
        public readonly Vector2[] UV;
        public readonly Vector2[] UV2;

        public DecomposedCornerBuffer(int size)
        {
            Vertices = new Vector3[size];
            Normals = new Vector3[size];
            UV = new Vector2[size];
            UV2 = new Vector2[size];
        }

        public void Set(int index, Corner item)
        {
            Vertices[index] = item.Vertex;
            Normals[index] = item.Normal;
            UV[index] = item.uv0;
            UV2[index] = item.uv2;
        }
    }
}