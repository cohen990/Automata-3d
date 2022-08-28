public static class PhysicsLayers
{
    private const int Terrain = 8;
    public static int TerrainMask => Mask(Terrain);

    private static int Mask(int layer)
    {
        return 1 << layer;
    }
}