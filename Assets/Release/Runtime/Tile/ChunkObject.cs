using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hyperbolic.Tile
{
    [CreateAssetMenu(fileName = "Chunk", menuName = "Hyperbolic/Chunk")]
    public class ChunkObject : ScriptableObject
    {
        [SerializeField] List<ChunkTile> tiles;

        public void Initialize()
        {
            GameObject reference = Instantiate(new GameObject(), new Vector3(0, 0, 0), Quaternion.identity);
            reference.name = "Reference";
            foreach(ChunkTile tile in tiles)
            {
                GameObject obj = Instantiate(tile.tile.TilePrefab, new Vector3(0,0,0), Quaternion.Euler(new Vector3(-90, 0, 0)));
                Tile objTile = obj.AddComponent<Tile>();
                objTile.TileObject = tile.tile;
                objTile.ChunkMap = tile.chunkMap;
            }
        }
    }

    [System.Serializable]
    public class ChunkTile
    {
        [SerializeField] public TileObject tile;
        [SerializeField] public ChunkMap chunkMap;
    }
}
