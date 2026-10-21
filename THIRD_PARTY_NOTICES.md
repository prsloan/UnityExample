# Third-Party Notices

The MIT License in `LICENSE` covers the original code and assets in this
repository. The components below are not covered by it. They keep their own
terms.

## SimplexNoise for C#

- **File:** `Assets/Scripts/Utility/Noise.cs`
- **Author:** Heikki Törmälä
- **License:** public domain, dedicated under the Unlicense (<https://unlicense.org/>).
  The full dedication is kept in the file header.
- **Lineage:** this is a C# implementation of Ken Perlin's simplex noise
  algorithm (2001). It is based loosely on SimplexNoise1234 by Stefan Gustavson,
  which its author has also released into the public domain.

## Unity Standard Assets

- **Files:** `Assets/Standard Assets/`, a trimmed copy with the first-person
  controller, its audio, and the cross-platform input scripts. It also covers
  `Assets/Editor/CrossPlatformInput/CrossPlatformInputInitialize.cs`.
- **Author:** Unity Technologies
- **License:** Unity Companion License
  (<https://unity.com/legal/licenses/unity-companion-license>). You may use and
  modify these files only in projects built with the Unity engine.

## Unity Shader Graph samples

- **Files:** `Assets/Art/Shaders/Subgraphs/` (AlphaMerge, BranchOnRP,
  DistanceMask, ExpandVec2), `Assets/Art/Textures/water_caustics.png`, and
  `Assets/Art/Textures/puddle_norm.png`
- **Source:** the samples in the `com.unity.shadergraph` package (17.6.0)
- **Author:** Unity Technologies
- **License:** Unity Companion License, as above.

## Not included

The splash audio used during development comes from the Rocklynn Productions
Water Splash Pack, a licensed Unity Asset Store pack. It can't be
redistributed, so it isn't part of this repository. See the README for how the
game loads optional audio from `Assets/Resources/WaterSounds/`.
