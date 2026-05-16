using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Slider masterVol;
    [SerializeField] private Slider musicVol;
    [SerializeField] private Slider soundVol;

    [SerializeField] private AudioMixer mixer;

    void Start()
    {
        masterVol.value = Mathf.Pow(10, (GetMixerVolume("MasterVolume") / 20));
        musicVol.value = Mathf.Pow(10, (GetMixerVolume("MusicVolume") / 20));
        soundVol.value = Mathf.Pow(10, (GetMixerVolume("SoundVolume") / 20));
    }

    private float GetMixerVolume(string groupName)
    {
        float value;
        bool result = mixer.GetFloat(groupName, out value);
        if (result)
            return value;
        else
            return 0f;
    }

    public void SetMasterVolume()
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(masterVol.value) * 20);
    }

    public void SetMusicVolume()
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(musicVol.value) * 20);
    }

    public void SetSoundVolume()
    {
        mixer.SetFloat("SoundVolume", Mathf.Log10(soundVol.value) * 20);
    }
}
