using UnityEngine;
using System.IO;

// Chunk save format (little-endian, written with BinaryWriter):
//   byte[2] magic "VX", byte version, int32 count,
//   then count entries of: byte x, byte y, byte z, byte blockType.
// Only blocks the player has changed are stored; the rest is regenerated.
//noinspection CheckNameSpace
public static class Serialization {
    private static string _saveFolderName = "voxelGameSaves";

    private const byte Magic0 = (byte)'V';
    private const byte Magic1 = (byte)'X';
    const byte FormatVersion = 1;

    // Stable ids for each block type. Never renumber these, only append.
    enum BlockType : byte { Stone = 0, Air = 1, Grass = 2, Leaves = 3, Wood = 4 }

    static BlockType TypeOf(Block block) {
        switch (block)
        {
            case BlockAir: return BlockType.Air;
            case BlockGrass: return BlockType.Grass;
            case BlockLeaves: return BlockType.Leaves;
            case BlockWood: return BlockType.Wood;
        }
        return BlockType.Stone;
    }

    static Block CreateBlock(BlockType type) {
        switch (type) {
            case BlockType.Air: return new BlockAir();
            case BlockType.Grass: return new BlockGrass();
            case BlockType.Leaves: return new BlockLeaves();
            case BlockType.Wood: return new BlockWood();
            case BlockType.Stone: return new Block();
        }
        return null;
    }

    public static string SaveLocation(string worldName) {
        string saveLocation = _saveFolderName + "/" + worldName + "/";

        if (!Directory.Exists(saveLocation)) {
            Directory.CreateDirectory(saveLocation);
        }

        return saveLocation;
    }

    public static string FileName(WorldPos chunkLocation) {
        string fileName = chunkLocation.x + "," + chunkLocation.y + "," + chunkLocation.z + ".bin";
        return fileName;
    }

    public static void SaveChunk(Chunk chunk) {
        Save save = new Save(chunk);
        if (save.blocks.Count == 0)
            return;
        string saveFile = SaveLocation(chunk.world.worldName);
        saveFile += FileName(chunk.pos);

        using (var writer = new BinaryWriter(File.Open(saveFile, FileMode.Create, FileAccess.Write, FileShare.None))) {
            writer.Write(Magic0);
            writer.Write(Magic1);
            writer.Write(FormatVersion);
            writer.Write(save.blocks.Count);
            foreach (var entry in save.blocks) {
                writer.Write((byte)entry.Key.x);
                writer.Write((byte)entry.Key.y);
                writer.Write((byte)entry.Key.z);
                writer.Write((byte)TypeOf(entry.Value));
            }
        }
    }


    public static bool Load(Chunk chunk) {
        string saveFile = SaveLocation(chunk.world.worldName);
        saveFile += FileName(chunk.pos);

        if (!File.Exists(saveFile))
            return false;

        try {
            using (var reader = new BinaryReader(File.Open(saveFile, FileMode.Open, FileAccess.Read))) {
                if (reader.ReadByte() != Magic0 || reader.ReadByte() != Magic1
                    || reader.ReadByte() != FormatVersion) {
                    Debug.LogWarning("Unrecognized chunk save format, ignoring " + saveFile);
                    return false;
                }

                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++) {
                    int x = reader.ReadByte();
                    int y = reader.ReadByte();
                    int z = reader.ReadByte();
                    Block block = CreateBlock((BlockType)reader.ReadByte());
                    if (block == null || !Chunk.InRange(x) || !Chunk.InRange(y) || !Chunk.InRange(z))
                        continue;

                    chunk.blocks[x, y, z] = block;
                }
            }
        }
        catch (EndOfStreamException) {
            Debug.LogWarning("Truncated chunk save, ignoring the rest of " + saveFile);
            return false;
        }
        return true;
    }
}
