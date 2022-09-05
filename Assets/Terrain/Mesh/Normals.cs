using UnityEngine;

namespace Terrain.Mesh
{
    public static class Normals
    {
        public static Vector3 NegativeZ = new(0, 0, -1);
        public static Vector3 NegativeY = new(0, -1, 0);
        public static Vector3 NegativeX = new(-1, 0, 0);
        public static Vector3 PositiveZ = new(0, 0, 1);
        public static Vector3 PositiveY = new(0, 1, 0);
        public static Vector3 PositiveX = new(1, 0, 0);
    }
}