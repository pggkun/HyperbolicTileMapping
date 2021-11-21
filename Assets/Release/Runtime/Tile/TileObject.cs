using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hyperbolic.Tile
{
    [CreateAssetMenu(fileName = "Tile", menuName = "Hyperbolic/Tile")]
    public class TileObject : ScriptableObject
    {
        [SerializeField] GameObject tilePrefab;
        [SerializeField] Texture texturePrefab;

        public GameObject TilePrefab => tilePrefab;
        public Texture TexturePrefab => texturePrefab;
    }

    public enum ChunkMap
    {
        center,
        up,
        down,
        left,
        right,
        upLeft,
        leftUp,
        upRight,
        rightUp,
        downLeft,
        leftDown,
        downRight,
        rightDown
    }
}
