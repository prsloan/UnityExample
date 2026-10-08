using UnityEngine;
using System.Collections;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour {
    public World world;
    public WorldPos pos;
    public static int chunkSize = 16;
    [System.NonSerialized] public Block[,,] blocks = new Block[chunkSize, chunkSize, chunkSize];
    public bool rendered;
    public bool update = false;
    MeshFilter filter;
    MeshCollider coll;

    // Use this for initialization
    void Start () {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
       
    }
	
	// Update is called once per frame
	void Update () {
        if (update) {
            update = false;
            UpdateChunk();
        }
    }


    // Other Methods/////////////////////////////////////

    public void SetBlocksUnmodified() {
        foreach (Block block in blocks) {
            block.changed = false;
        }
    }


    public Block GetBlock(int x, int y, int z) {
        if (InRange(x) && InRange(y) && InRange(z))
            return blocks[x, y, z];
        return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
    }
    //helper function
    public static bool InRange(int index) {
        if (index < 0 || index >= chunkSize)
            return false;

        return true;
    }

    public void SetBlock(int x, int y, int z, Block block) {
        if (InRange(x) && InRange(y) && InRange(z)) {
            blocks[x, y, z] = block;
        }
        else {
            world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
        }
    }




    //Updates the chunk based on its contents
    void UpdateChunk() {
        rendered = true;
        MeshData meshData = new MeshData();
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                for (int z = 0; z < chunkSize; z++) {
                    meshData = blocks[x, y, z].Blockdata(this, x, y, z, meshData);
                }
            }
        }
        RenderMesh(meshData);
    }
    //Sends the calculated mesh information
    //to the mesh and collision components
    void RenderMesh(MeshData meshData) {
        // Submesh 0 is the opaque terrain, submesh 1 is water (second material on the prefab).
        // 32-bit indices: a busy chunk plus double-sided water can pass 65k vertices.
        Mesh renderMesh = filter.mesh;
        renderMesh.Clear();
        renderMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        renderMesh.vertices = meshData.vertices.ToArray();
        renderMesh.subMeshCount = 2;
        renderMesh.SetTriangles(meshData.triangles, 0);
        renderMesh.SetTriangles(meshData.waterTriangles, 1);

        renderMesh.uv = meshData.uv.ToArray();
        renderMesh.RecalculateNormals();

        coll.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = meshData.colVertices.ToArray();
        mesh.triangles = meshData.colTriangles.ToArray();
        mesh.RecalculateNormals();

        coll.sharedMesh = mesh;
    }


    

}
