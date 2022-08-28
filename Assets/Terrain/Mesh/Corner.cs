using UnityEngine;

namespace Terrain.Mesh
{
    public class Corner
    {
        public Corner(Vector3 vertex, Vector3 normal, Vector2 uv0, Vector2 uv2)
        {
            Vertex = vertex;
            Normal = normal;
            this.uv0 = uv0;
            this.uv2 = uv2;
        }

        public Vector3 Normal { get; }
        public Vector3 Vertex { get; }
        // ReSharper disable once InconsistentNaming
        public Vector2 uv0 { get; }
        // ReSharper disable once InconsistentNaming
        public Vector2 uv2 { get; }

        public override int GetHashCode()
        {
            return Normal.GetHashCode() ^ (Vertex.GetHashCode() << 2);
        }

        public override string ToString()
        {
            return
                $"Position: {Vertex.ToString()}, Normal: {Normal.ToString()}, uv0: {uv0.ToString()}, uv2: {uv2.ToString()}";
        }

        public CornerBufferKey ToCornerBufferKey()
        {
            return new CornerBufferKey
            {
                Vertex = Vertex,
                Normal = Normal,
                uv0 = uv0
            };
        }
        
        public struct CornerBufferKey
        {
            public Vector3 Vertex;
            public Vector3 Normal;
            // ReSharper disable once InconsistentNaming
            public Vector2 uv0;
            
            public override int GetHashCode()
            {
                return Normal.GetHashCode() ^ (Vertex.GetHashCode() << 2) ^ uv0.GetHashCode() << 4;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || obj.GetType() != GetType())
                {
                    return false;
                }

                var other = (CornerBufferKey) obj;

                return Normal.Equals(other.Normal) && Vertex.Equals(other.Vertex) && uv0.Equals(other.uv0);
            }
        }
    }
}