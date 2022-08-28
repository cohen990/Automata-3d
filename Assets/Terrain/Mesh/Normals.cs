using UnityEngine;

namespace Terrain.Mesh
{
    public static class Normals
    {
        public static Vector3 NegativeZ = new Vector3(0, 0, -1);
        public static Vector3 NegativeY = new Vector3(0, -1, 0);
        public static Vector3 NegativeX = new Vector3(-1, 0, 0);
        public static Vector3 PositiveZ = new Vector3(0, 0, 1);
        public static Vector3 PositiveY = new Vector3(0, 1, 0);
        public static Vector3 PositiveX = new Vector3(1, 0, 0);
    }
}