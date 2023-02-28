using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    private ScriptableVolumeSettings _volumeSettings;

    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private CommonTypes.VolumeChannel _volumeChannel;

    [SerializeField]
    private TMP_Text _text;

    // Start is called before the first frame update
    private void Start()
    {
        switch (_volumeChannel)
        {
            case CommonTypes.VolumeChannel.Master:
                _slider.value = _volumeSettings.MasterVolume * 10;
                break;
            case CommonTypes.VolumeChannel.Music:
                _slider.value = _volumeSettings.MusicVolume * 10;
                break;
            case CommonTypes.VolumeChannel.SFX:
                _slider.value = _volumeSettings.SfxVolume * 10;
                break;
            default:
                break;
        }

        _text.text = _volumeChannel.Name();
    }   

    public void OnSliderValueChanged(Slider slider)
    {
        switch (_volumeChannel)
        {
            case CommonTypes.VolumeChannel.Master:
                _volumeSettings.SetMasterVolume(slider.value / 10);
                break;
            case CommonTypes.VolumeChannel.Music:
                _volumeSettings.SetMusicVolume(slider.value / 10);
                break;
            case CommonTypes.VolumeChannel.SFX:
                _volumeSettings.SetSfxVolume(slider.value / 10);
                break;
            default:
                break;
        }
    }
}
