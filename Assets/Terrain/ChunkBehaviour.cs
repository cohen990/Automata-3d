using UnityEngine;

namespace Terrain
{
    public class ChunkBehaviour : MonoBehaviour
    {
        public void FormMesh(Chunk chunk, World world)
        {
            ChunkMesh.Generate(GetComponent<MeshFilter>(), chunk, world);
            GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        }

    }
}