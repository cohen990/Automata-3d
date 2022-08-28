using UnityEngine;

namespace Terrain
{
    public class ChunkBehaviour : MonoBehaviour
    {
        public void FormMesh(Chunk chunk)
        {
            ChunkMesh.Generate(GetComponent<MeshFilter>(), chunk);
            GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        }
    }
}