using System;

namespace Terrain.Mesh
{
    [Flags]
    public enum Faces
    {
        None = 0,
        NegativeX = 1,
        PositiveX = 2,
        NegativeY = 4,
        PositiveY = 8,
        NegativeZ = 16,
        PositiveZ = 32
    }
}