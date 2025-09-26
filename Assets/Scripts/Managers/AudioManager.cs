using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        
        private const string Mixer_BGM = "bgm";
        private const string Mixer_SFX = "sfx";
        
        private const float Volume_Threshold = 0.1f;
        private const float Volume_Decrease_Rate = 0.25f;
        private const float Volume_Decrease_Interval = 0.6f;
        
        private static Vector2 sfxPitchRange = new(0.85f, 1.1f);
        
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource[] sfx;
        [SerializeField] private AudioSource[] bgm;
        [SerializeField] private float sfxMinDistance;
        [SerializeField] private float volumeScaleFactor;
        [SerializeField] bool shouldPlayBGM;

        private float _bgmVolume;
        private float _sfxVolume;
    
        private int _bgmIndex;
        private bool _canPlaySFX;
        
        private void Awake()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        
            AllowSFX(true);
        }

        private void Start()
        {
            mixer.SetFloat(Mixer_BGM, Mathf.Log10(_bgmVolume) * volumeScaleFactor);
            mixer.SetFloat(Mixer_SFX, Mathf.Log10(_sfxVolume) * volumeScaleFactor);
        }

        private void Update()
        {
            if (!shouldPlayBGM)
                StopAllBGM();
            else
            {
                if (!bgm[_bgmIndex].isPlaying)
                    PlayBGM(_bgmIndex);
            }
        }
        
        public AudioMixer GetMixer() => mixer;
        public float GetVolumeScaleFactor() => volumeScaleFactor;

        public void SetupBGMVolume(float savedBGMVolume) => _bgmVolume = savedBGMVolume;
        public void SetupSFXVolume(float savedSFXVolume) => _sfxVolume = savedSFXVolume;
        public void AllowSFX(bool allow) => _canPlaySFX = allow;

        public void PlayBGM(int bgmIndex)
        {
            _bgmIndex = bgmIndex;

            StopAllBGM();
            bgm[_bgmIndex].Play();
        }
        
        public void PlaySFX(int sfxIndex, Transform source)
        {
            if (!_canPlaySFX) return;

            if (source != null && Vector2.Distance(PlayerManager.Instance.PlayerGameObject.transform.position, source.position) > sfxMinDistance)
                return;

            if (sfxIndex < sfx.Length)
            {
                sfx[sfxIndex].pitch = Random.Range(sfxPitchRange.x, sfxPitchRange.y);
                sfx[sfxIndex].Play();
            }
        }

        public void StopAllBGM()
        {
            foreach (var s in bgm)
                s.Stop();
        }
        
        public void StopSFX(int index)
        {
            if(sfx != null && sfx[index].isPlaying)
                sfx[index].Stop();
        }

        public void StopSFXWithDelay(int index) 
            => StartCoroutine(DecreaseVolumeRoutine(sfx[index]));

        private IEnumerator DecreaseVolumeRoutine(AudioSource audio)
        {
            if (audio == null) yield return null;

            float defaultVolume = audio.volume;

            while (audio.volume > Volume_Threshold)
            {
                audio.volume -= audio.volume * Volume_Decrease_Rate;
                yield return new WaitForSeconds(Volume_Decrease_Interval);

                if (audio.volume <= Volume_Threshold)
                {
                    audio.Stop();
                    audio.volume = defaultVolume;
                    break;
                }
            }
        }
    }
}
