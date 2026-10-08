using UnityEngine;
using System.Collections;
using System;

// Transparent, non-solid block. Its faces go into the chunk's water submesh
// (second material) and are left out of the collision mesh.
[Serializable]
public class BlockWater : Block {

    // Cell in tiles.png used if the water material gets a texture. Unused by default.
    const int tileX = 3;
    const int tileY = 3;

    public BlockWater() : base() {}

    public override Tile TexturePosition(Direction direction) {
        Tile tile = new Tile();
        tile.x = tileX;
        tile.y = tileY;
        return tile;
    }

    public override bool IsSolid(Block.Direction direction) {
        return false;
    }

    public override MeshData Blockdata
        (Chunk chunk, int x, int y, int z, MeshData meshData) {
        // Solid blocks switch this back on, so it has to be cleared for every water block.
        meshData.useRenderDataForCol = false;

        if (ShowFace(chunk, x, y + 1, z, Direction.down)) {
            meshData = FaceDataUp(chunk, x, y, z, meshData);
        }
        if (ShowFace(chunk, x, y - 1, z, Direction.up)) {
            meshData = FaceDataDown(chunk, x, y, z, meshData);
        }
        if (ShowFace(chunk, x, y, z + 1, Direction.south)) {
            meshData = FaceDataNorth(chunk, x, y, z, meshData);
        }
        if (ShowFace(chunk, x, y, z - 1, Direction.north)) {
            meshData = FaceDataSouth(chunk, x, y, z, meshData);
        }
        if (ShowFace(chunk, x + 1, y, z, Direction.west)) {
            meshData = FaceDataEast(chunk, x, y, z, meshData);
        }
        if (ShowFace(chunk, x - 1, y, z, Direction.east)) {
            meshData = FaceDataWest(chunk, x, y, z, meshData);
        }
        return meshData;
    }

    // A water face is only drawn against air-like neighbors: not other water, not solid blocks.
    static bool ShowFace(Chunk chunk, int x, int y, int z, Direction facingUs) {
        Block neighbor = chunk.GetBlock(x, y, z);
        return !(neighbor is BlockWater) && !neighbor.IsSolid(facingUs);
    }

    // Each face is added twice, the second time with reversed winding and its own
    // vertices, so the surface is visible from underneath too. (Sharing vertices would
    // make RecalculateNormals cancel the two normals out.)
    void AddWaterFace(MeshData meshData, Direction direction,
                      Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
        Vector2[] uvs = FaceUVs(direction);

        meshData.AddVertex(a);
        meshData.AddVertex(b);
        meshData.AddVertex(c);
        meshData.AddVertex(d);
        meshData.AddWaterQuadTriangles();
        meshData.uv.AddRange(uvs);

        meshData.AddVertex(d);
        meshData.AddVertex(c);
        meshData.AddVertex(b);
        meshData.AddVertex(a);
        meshData.AddWaterQuadTriangles();
        meshData.uv.Add(uvs[3]);
        meshData.uv.Add(uvs[2]);
        meshData.uv.Add(uvs[1]);
        meshData.uv.Add(uvs[0]);
    }

    protected override MeshData FaceDataUp
        (Chunk chunk, int x, int y, int z, MeshData meshData) {
        AddWaterFace(meshData, Direction.up,
            new Vector3(x - 0.5f, y + 0.5f, z + 0.5f),
            new Vector3(x + 0.5f, y + 0.5f, z + 0.5f),
            new Vector3(x + 0.5f, y + 0.5f, z - 0.5f),
            new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        return meshData;
    }

    protected override MeshData FaceDataDown
        (Chunk chunk, int x, int y, int z, MeshData meshData) {
        AddWaterFace(meshData, Direction.down,
            new Vector3(x - 0.5f, y - 0.5f, z - 0.5f),
            new Vector3(x + 0.5f, y - 0.5f, z - 0.5f),
            new Vector3(x + 0.5f, y - 0.5f, z + 0.5f),
            new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
        return meshData;
    }

    protected override MeshData FaceDataNorth
        (Chunk chunk, int x, int y, int z, MeshData meshData) {
        AddWaterFace(meshData, Direction.north,
            new Vector3(x + 0.5f, y - 0.5f, z + 0.5f),
            new Vector3(x + 0.5f, y + 0.5f, z + 0.5f),
            new Vector3(x - 0.5f, y + 0.5f, z + 0.5f),
            new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
        return meshData;
    }

    protected override MeshData FaceDataEast
        (Chunk chunk, int x, int y, int z, MeshData meshData) {
        AddWaterFace(meshData, Direction.east,
            new Vector3(x + 0.5f, y - 0.5f, z - 0.5f),
            new Vector3(x + 0.5f, y + 0.5f, z - 0.5f),
            new Vector3(x + 0.5f, y + 0.5f, z + 0.5f),
            new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        return meshData;
    }

    protected override MeshData FaceDataSouth
        (Chunk chunk, int x, int y, int z, MeshData meshData) {
        AddWaterFace(meshData, Direction.south,
            new Vector3(x - 0.5f, y - 0.5f, z - 0.5f),
            new Vector3(x - 0.5f, y + 0.5f, z - 0.5f),
            new Vector3(x + 0.5f, y + 0.5f, z - 0.5f),
            new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        return meshData;
    }

    protected override MeshData FaceDataWest
        (Chunk chunk, int x, int y, int z, MeshData meshData) {
        AddWaterFace(meshData, Direction.west,
            new Vector3(x - 0.5f, y - 0.5f, z + 0.5f),
            new Vector3(x - 0.5f, y + 0.5f, z + 0.5f),
            new Vector3(x - 0.5f, y + 0.5f, z - 0.5f),
            new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        return meshData;
    }
}
