using System;
using UnityEngine;

namespace Terrain.Mesh
{
    public static class TextureLocations
    {
        public static readonly Vector2 Dirt = new(0, 0);
        public static readonly Vector2 Grass = new(1, 0);
        public static readonly Vector2 Stone = new(0, 1);
        public static readonly Vector2 Bedrock = new(1, 1);
        public static readonly Vector2 Air = Vector2.negativeInfinity;
    }
}