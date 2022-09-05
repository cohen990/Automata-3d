using System;
using System.Collections.Generic;

namespace Terrain.Mesh
{
    public class CornerComparer: IEqualityComparer<Corner>
    {
        public bool Equals(Corner x, Corner y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Normal.Equals(y.Normal) && x.Vertex.Equals(y.Vertex) && x.uv0.Equals(y.uv0);
        }

        public int GetHashCode(Corner obj)
        {
            return HashCode.Combine(obj.Normal, obj.Vertex, obj.uv0);
        }
    }
}