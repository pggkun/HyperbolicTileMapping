using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hyperbolic.Math;

namespace Hyperbolic.Tile
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private TileObject tileObject;
        [SerializeField] private ChunkMap chunkMap;
        private GameObject referenceTransformObject;
        public TileObject TileObject { get => tileObject; set => tileObject = value; }
        public ChunkMap ChunkMap { get => chunkMap; set => chunkMap = value; }

        private void Start()
        {
            referenceTransformObject = GameObject.Find("Reference");
            AssignTexture();
            TileUtils.MeshKleinToMinkowski(gameObject, referenceTransformObject);
            AssignPosition((float)Math.Math.HypotenuseOfFundamentalRegionEndPoint(4, 5)[1].x * 4);
        }

        private void Update()
        {
            TileUtils.Move(gameObject, 1f, referenceTransformObject);
        }

        private void AssignTexture()
        {
            var texturePropertyBlock = new MaterialPropertyBlock();
            texturePropertyBlock.SetTexture("_MainTex", TileObject.TexturePrefab);
            GetComponent<Renderer>().SetPropertyBlock(texturePropertyBlock);
        }

        private void AssignPosition(float step)
        {
            step = step+ 0.025f;
            if (chunkMap == ChunkMap.center) TileUtils.MoveDiscrete(gameObject, 0, 0, false);
            if (chunkMap == ChunkMap.up) TileUtils.MoveDiscrete(gameObject, step, 0, false);
            if (chunkMap == ChunkMap.left) TileUtils.MoveDiscrete(gameObject, 0, step, false);
            if (chunkMap == ChunkMap.down) TileUtils.MoveDiscrete(gameObject, -step, 0, false);
            if (chunkMap == ChunkMap.right) TileUtils.MoveDiscrete(gameObject, 0, -step, false);
            if (chunkMap == ChunkMap.leftUp) TileUtils.MoveDiscrete(gameObject, step, step, false);
            if (chunkMap == ChunkMap.upLeft) TileUtils.MoveDiscrete(gameObject, step, step, true);
            if (chunkMap == ChunkMap.rightDown) TileUtils.MoveDiscrete(gameObject, -step, -step, false);
            if (chunkMap == ChunkMap.downRight) TileUtils.MoveDiscrete(gameObject, -step, -step, true);
            if (chunkMap == ChunkMap.rightUp) TileUtils.MoveDiscrete(gameObject, step, -step, false);
            if (chunkMap == ChunkMap.upRight) TileUtils.MoveDiscrete(gameObject, step, -step, true);
            if (chunkMap == ChunkMap.leftDown) TileUtils.MoveDiscrete(gameObject, -step, step, false);
            if (chunkMap == ChunkMap.downLeft) TileUtils.MoveDiscrete(gameObject, -step, step, true);
        }
    }
}
