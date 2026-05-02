using UnityEngine;
using System.Collections.Generic;

public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance;

    public AudioSource nonBulkSource;

    public int poolSize = 10;
    public int maxSimultaneousPerClip = 5;

    public float minDelay = 0.01f;
    public float maxDelay = 0.1f;

    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    private List<AudioSource> pool = new List<AudioSource>();

    // How many instances of each clip are playing
    private Dictionary<AudioClip, int> activeClipCounts = new Dictionary<AudioClip, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Create audio source pool
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource s = gameObject.AddComponent<AudioSource>();
            s.playOnAwake = false;
            pool.Add(s);
        }
    }

    private AudioSource GetFreeSource()
    {
        foreach (var s in pool)
            if (!s.isPlaying)
                return s;

        return null;
    }

    public static void PlayBulkOneShot(AudioClip clip, float volume = 1f)
    {
        Instance.StartCoroutine(Instance.PlayRoutine(clip, volume));
    }

    public static void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        Instance.nonBulkSource.PlayOneShot(clip, volume);   
    }

    private System.Collections.IEnumerator PlayRoutine(AudioClip clip, float volume)
    {
        // Ensure tracking exists
        if (!activeClipCounts.ContainsKey(clip))
            activeClipCounts[clip] = 0;

        // Too many of THIS clip currently playing?
        if (activeClipCounts[clip] >= maxSimultaneousPerClip)
            yield break;

        // Small random delay to avoid robotic stacking
        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        // Get an available source
        AudioSource src = GetFreeSource();
        if (src == null)
            yield break; // no free sources → silently drop the sound

        // Apply pitch variation
        src.pitch = Random.Range(minPitch, maxPitch);

        // Count as active
        activeClipCounts[clip]++;

        src.clip = clip;
        src.volume = volume;
        src.Play();

        // Wait until this sound finishes, then decrement
        yield return new WaitForSeconds(clip.length / src.pitch);
        activeClipCounts[clip]--;
    }
}
