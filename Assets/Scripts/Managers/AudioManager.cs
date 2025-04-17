using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer mixer;
    public float multiplier = 25f;

    private float bgmVolume;
    private float sfxVolume;

    [SerializeField] private float sfxMinDistance;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    public bool playBGM;
    private int bgmIndex;

    private bool canPlaySFX;

    void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        Invoke("AllowSFX", 0.5f);
    }

    void Start()
    {
        mixer.SetFloat("bgm", Mathf.Log10(bgmVolume) * multiplier);
        mixer.SetFloat("sfx", Mathf.Log10(sfxVolume) * multiplier);
    }

    void Update()
    {
        if (!playBGM)
            StopAllBGM();
        else
        {
            if (!bgm[bgmIndex].isPlaying)
                PlayBGM(bgmIndex);
        }
    }

    public void SetupBGMVolume(float _savedBGMVolume) => bgmVolume = _savedBGMVolume;
    public void SetupSFXVolume(float _savedSFXVolume) => sfxVolume = _savedSFXVolume;

    public void PlaySFX(int _sfxIndex, Transform _source)
    {
        // if (sfx[_sfxIndex].isPlaying)
        //     return;

        if (!canPlaySFX) return;

        if (_source != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinDistance)
            return;

        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].pitch = Random.Range(0.85f, 1.1f);
            sfx[_sfxIndex].Play();
        }
    }

    public void StopSFX(int _index) => sfx[_index].Stop();

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    public void PlayBGM(int _bgmIndex)
    {
        bgmIndex = _bgmIndex;

        StopAllBGM();
        bgm[bgmIndex].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    public void StopSFXWithDelay(int _index) => StartCoroutine(DecreaseVolume(sfx[_index]));

    private void AllowSFX() => canPlaySFX = true;

    private IEnumerator DecreaseVolume(AudioSource _audio)
    {
        if (_audio == null) yield return null;

        float defaultVolume = _audio.volume;

        while (_audio.volume > 0.1f)
        {
            _audio.volume -= _audio.volume * 0.25f;
            yield return new WaitForSeconds(0.6f);

            if (_audio.volume <= 0.1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }
    }
}
