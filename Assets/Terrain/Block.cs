namespace Terrain
{
    public static class Block
    {
        public const int NULL = -1;
        public const int AIR = 0;
        public const int DIRT = 1;
        public const int GRASS = 2;
        public const int STONE = 3;
        public const int BEDROCK = 4;

        public static bool IsEmpty(int block)
        {
            return block <= 0;
        }
    }
}