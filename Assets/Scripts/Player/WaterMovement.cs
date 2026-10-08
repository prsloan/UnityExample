using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Resistance, buoyancy, underwater fog and sloshing sounds for the first-person player.
// It looks up the blocks at the player's feet, waist and eyes and drives the hooks on
// FirstPersonController. It attaches itself at scene load, so no scene setup is needed.
[RequireComponent(typeof(CharacterController))]
public class WaterMovement : MonoBehaviour {
    public float wadeSpeedScale = 0.65f;    // feet in water
    public float swimSpeedScale = 0.45f;    // waist in water
    public float sloshInterval = 0.55f;
    public Color underwaterFogColor = new Color(0.1f, 0.3f, 0.55f);
    public float underwaterFogDensity = 0.06f;

    World world;
    FirstPersonController controller;
    CharacterController body;
    Camera eyes;
    AudioSource audioSource;
    AudioClip[] sloshes;
    AudioClip[] splashesIn;
    AudioClip[] splashesOut;

    bool wasInWater;
    bool wasSubmerged;
    float nextSlosh;

    bool savedFog;
    Color savedFogColor;
    float savedFogDensity;
    FogMode savedFogMode;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AttachToPlayer() {
        FirstPersonController player = FindFirstObjectByType<FirstPersonController>();
        if (player != null && player.GetComponent<WaterMovement>() == null)
            player.gameObject.AddComponent<WaterMovement>();
    }

    void Start() {
        world = FindFirstObjectByType<World>();
        controller = GetComponent<FirstPersonController>();
        body = GetComponent<CharacterController>();
        eyes = Camera.main;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        // Drop-in sounds: any clips in Assets/Resources/WaterSounds/{Slosh,SplashIn,SplashOut}/
        // replace the synthesized ones for that group.
        sloshes = Resources.LoadAll<AudioClip>("WaterSounds/Slosh");
        if (sloshes.Length == 0) {
            sloshes = new AudioClip[] {
                MakeNoiseClip("slosh1", 1, 0.40f, 0.07f),
                MakeNoiseClip("slosh2", 2, 0.45f, 0.06f),
                MakeNoiseClip("slosh3", 3, 0.35f, 0.08f),
            };
        }
        splashesIn = Resources.LoadAll<AudioClip>("WaterSounds/SplashIn");
        splashesOut = Resources.LoadAll<AudioClip>("WaterSounds/SplashOut");
        if (splashesIn.Length == 0)
            splashesIn = new AudioClip[] { MakeNoiseClip("splash", 4, 0.70f, 0.22f) };
        if (splashesOut.Length == 0)
            splashesOut = splashesIn;
    }

    void Update() {
        if (world == null || controller == null || eyes == null)
            return;

        Bounds bounds = body.bounds;
        bool feet = IsWater(new Vector3(bounds.center.x, bounds.min.y + 0.15f, bounds.center.z));
        bool waist = IsWater(bounds.center);
        bool head = IsWater(eyes.transform.position);

        controller.InWater = feet;
        controller.Swimming = waist;
        controller.AtSurface = waist && !head;
        controller.MoveSpeedScale = waist ? swimSpeedScale : (feet ? wadeSpeedScale : 1f);

        if (feet != wasInWater) {
            PlaySplash(feet ? splashesIn : splashesOut, feet ? 1f : 0.7f);
            wasInWater = feet;
        }
        if (head != wasSubmerged) {
            SetUnderwaterFog(head);
            wasSubmerged = head;
        }

        Vector3 v = body.velocity;
        bool moving = new Vector2(v.x, v.z).magnitude > 0.5f || (waist && Mathf.Abs(v.y) > 0.5f);
        if (feet && moving && Time.time >= nextSlosh) {
            nextSlosh = Time.time + sloshInterval * (waist ? 1.2f : 1f);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(sloshes[Random.Range(0, sloshes.Length)], waist ? 0.8f : 0.5f);
        }
    }

    void OnDisable() {
        if (wasSubmerged)
            SetUnderwaterFog(false);
    }

    // Blocks are centered on whole numbers, so rounding a position gives the block it is in.
    bool IsWater(Vector3 p) {
        return world.GetBlock(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y), Mathf.RoundToInt(p.z)) is BlockWater;
    }

    void PlaySplash(AudioClip[] clips, float volume) {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)], volume);
    }

    void SetUnderwaterFog(bool underwater) {
        if (underwater) {
            savedFog = RenderSettings.fog;
            savedFogColor = RenderSettings.fogColor;
            savedFogDensity = RenderSettings.fogDensity;
            savedFogMode = RenderSettings.fogMode;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = underwaterFogColor;
            RenderSettings.fogDensity = underwaterFogDensity;
        }
        else {
            RenderSettings.fog = savedFog;
            RenderSettings.fogMode = savedFogMode;
            RenderSettings.fogColor = savedFogColor;
            RenderSettings.fogDensity = savedFogDensity;
        }
    }

    // There are no water sound files in the project, so these are synthesized once at start:
    // low-passed white noise (smaller smoothing = deeper) under a swell envelope with a
    // slow wobble, which reads as a wash of water.
    static AudioClip MakeNoiseClip(string name, int seed, float seconds, float smoothing) {
        const int sampleRate = 22050;
        int count = (int)(sampleRate * seconds);
        float[] samples = new float[count];
        System.Random rng = new System.Random(seed);

        // Normalize for the one-pole filter's output level so every clip is similarly loud.
        float gain = 0.5f / Mathf.Sqrt(smoothing / (2f - smoothing) / 3f);
        float low = 0f;
        for (int i = 0; i < count; i++) {
            float noise = (float)(rng.NextDouble() * 2.0 - 1.0);
            low += (noise - low) * smoothing;
            float t = (float)i / count;
            float swell = Mathf.Sin(t * Mathf.PI);
            swell *= swell;
            float wobble = 0.65f + 0.35f * Mathf.Sin(t * Mathf.PI * 6f);
            samples[i] = Mathf.Clamp(low * gain * swell * wobble, -1f, 1f);
        }

        AudioClip clip = AudioClip.Create(name, count, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
