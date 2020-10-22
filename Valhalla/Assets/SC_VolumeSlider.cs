using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_VolumeSlider : MonoBehaviour
{
    public void SetLevel(float sliderValue)
    {
        SC_AudioManager.single.UpdateVolume(sliderValue);
    }
}
