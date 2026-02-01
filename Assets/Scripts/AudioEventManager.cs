using System.Collections.Generic;
using UnityEngine;

public class AudioEventManager : MonoBehaviour
{
    [System.Serializable]
    public struct SoundEvent
    {
        [SerializeField] private List<AudioClip> clips;
        [SerializeField] private float volume;

        public AudioClip GetRandomClip()
        {
            if (clips == null || clips.Count == 0)
            {
                return null;
            }

            return clips[Random.Range(0, clips.Count)];
        }

        public float Volume => volume;
    }

    [System.Serializable]
    private struct SoundEventEntry
    {
        public string key;
        public SoundEvent soundEvent;
    }

    public static AudioEventManager Instance { get; private set; }

    [SerializeField] private List<SoundEventEntry> soundEvents = new List<SoundEventEntry>();

    private readonly Dictionary<string, SoundEvent> soundEventMap = new Dictionary<string, SoundEvent>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        soundEventMap.Clear();
        foreach (SoundEventEntry entry in soundEvents)
        {
            if (string.IsNullOrWhiteSpace(entry.key))
            {
                Debug.LogWarning("[AudioEventManager] Ignoring SoundEvent with empty key.");
                continue;
            }

            if (soundEventMap.ContainsKey(entry.key))
            {
                Debug.LogWarning($"[AudioEventManager] Duplicate SoundEvent key '{entry.key}' found. Overwriting.");
            }

            soundEventMap[entry.key] = entry.soundEvent;
        }
    }

    public void PlaySoundEvent(string key, Vector3 location)
    {
        if (!soundEventMap.TryGetValue(key, out SoundEvent soundEvent))
        {
            Debug.LogWarning($"[AudioEventManager] SoundEvent '{key}' not found.");
            return;
        }

        AudioClip clip = soundEvent.GetRandomClip();
        if (clip == null)
        {
            Debug.LogWarning($"[AudioEventManager] SoundEvent '{key}' has no clips assigned.");
            return;
        }

        float volume = soundEvent.Volume;
        if (volume <= 0f)
        {
            volume = 1f;
        }

        AudioSource.PlayClipAtPoint(clip, location, volume);
    }
}
