using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Audio
{
    [System.Serializable]
    public struct SfxEntry
    {
        public Sfx id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
        public bool loop;
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Mixer")]
        [SerializeField] private AudioMixer mainMixer;           // kéo MainAudioMixer vào đây
        [SerializeField] private string musicVolumeParam = "MusicVolume";
        [SerializeField] private string sfxVolumeParam = "SFXVolume";

        [Header("Sources (children)")]
        [SerializeField] private AudioSource musicSource;        // kéo MusicSource
        [SerializeField] private AudioSource sfxSource;          // kéo SFXSource

        [Header("Libraries")]
        [SerializeField] private List<SfxEntry> sfxEntries = new();

        private Dictionary<Sfx, SfxEntry> sfxMap;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Build map
            sfxMap = new Dictionary<Sfx, SfxEntry>();
            foreach (var e in sfxEntries)
            {
                if (e.clip == null) continue;
                sfxMap[e.id] = e;
            }
        }

        //================== Public API ==================//
        public void PlaySfx(Sfx id)
        {
            if (!sfxMap.TryGetValue(id, out var entry) || entry.clip == null) return;

            if (entry.loop)
            {
                sfxSource.clip = entry.clip;
                sfxSource.loop = true;
                sfxSource.volume = entry.volume <= 0f ? 1f : entry.volume;
                if (!sfxSource.isPlaying) sfxSource.Play();
            }
            else
            {
                sfxSource.PlayOneShot(entry.clip, entry.volume <= 0f ? 1f : entry.volume);
            }
        }

        public void StopLoopingSfx()
        {
            sfxSource.loop = false;
            sfxSource.Stop();
            sfxSource.clip = null;
        }

        public void PlayMusic(AudioClip clip, float fadeSeconds = 0.5f, float targetVolume = 1f, bool loop = true)
        {
            if (clip == null) return;
            StopAllCoroutines();
            StartCoroutine(Co_CrossfadeMusic(clip, fadeSeconds, targetVolume, loop));
        }

        public void StopMusic(float fadeSeconds = 0.3f)
        {
            StopAllCoroutines();
            StartCoroutine(Co_FadeOutMusic(fadeSeconds));
        }

        // Volume: nhận 0..1, map sang dB cho Mixer (-80..0)
        public void SetMusicVolume01(float v01) => SetMixer01(musicVolumeParam, v01);
        public void SetSfxVolume01(float v01) => SetMixer01(sfxVolumeParam, v01);

        //================== Internals ==================//
        private IEnumerator Co_CrossfadeMusic(AudioClip next, float fade, float targetVol, bool loop)
        {
            float startVol = musicSource.volume;
            // Fade out current
            for (float t = 0; t < fade; t += Time.unscaledDeltaTime)
            {
                float k = 1f - (t / fade);
                musicSource.volume = startVol * k;
                yield return null;
            }

            musicSource.Stop();
            musicSource.clip = next;
            musicSource.loop = loop;
            musicSource.Play();

            // Fade in
            for (float t = 0; t < fade; t += Time.unscaledDeltaTime)
            {
                float k = (t / fade);
                musicSource.volume = Mathf.Lerp(0f, targetVol, k);
                yield return null;
            }
            musicSource.volume = targetVol;
        }

        private IEnumerator Co_FadeOutMusic(float fade)
        {
            float startVol = musicSource.volume;
            for (float t = 0; t < fade; t += Time.unscaledDeltaTime)
            {
                float k = 1f - (t / fade);
                musicSource.volume = startVol * k;
                yield return null;
            }
            musicSource.Stop();
            musicSource.clip = null;
        }

        private void SetMixer01(string param, float v01)
        {
            v01 = Mathf.Clamp01(v01);
            float dB = (v01 <= 0.0001f) ? -80f : Mathf.Lerp(-30f, 0f, v01); // -30dB..0dB đủ dùng
            if (mainMixer != null) mainMixer.SetFloat(param, dB);
        }
    }
}
