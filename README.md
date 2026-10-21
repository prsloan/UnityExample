## Blocky

A small voxel sandbox built in Unity 6 (URP). The world is generated in chunks
around the player as they move, so the player can walk infinitely.

Features:

- Procedural terrain from simplex noise: stone, grass, caves, and lakes
- First-person movement with collision, jumping, gravity, and footstep sounds
- Transparent water with swimming and splash sounds
- Remove a block by pointing at it and right-clicking
- Edited chunks are saved to `voxelGameSaves/` and reloaded on the next visit

Open `Assets/Scene1.unity` and press Play.

### Third-party content

- `Assets/Scripts/Utility/Noise.cs`: SimplexNoise for C# by Heikki Törmälä, public domain (Unlicense).
- `Assets/Standard Assets`: a trimmed copy of Unity's Standard Assets (first-person controller and
  cross-platform input), under the Unity Companion License.
- `Assets/Art/Shaders/Subgraphs`, `Assets/Art/Textures/water_caustics.png`, and
  `Assets/Art/Textures/puddle_norm.png`: from Unity's Shader Graph package samples, under the
  Unity Companion License.

Optional splash audio: `WaterMovement` loads any clips found in
`Assets/Resources/WaterSounds/{Slosh,SplashIn,SplashOut}/` and falls back to synthesized sounds
when there are none. The clips used during development come from a licensed Asset Store pack
that can't be redistributed, so that folder is in `.gitignore`.
