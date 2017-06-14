using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeVolume : MonoBehaviour {

    [SerializeField] private float current_volume = -1f;

    public GameObject music_obj;
    public GameObject music_slider_obj;

    public void ToggleMusic()
    {
        if (AudioListener.volume == 0)
        {
            AudioListener.volume = current_volume;
        }   
        else
        {
            if (current_volume == -1f)
            {
                current_volume = AudioListener.volume;
            }
            AudioListener.volume = 0;
        }
    }
	
    public void ScrollMusic()
    {
        current_volume = music_slider_obj.GetComponent<Slider>().value;
        if (music_obj.GetComponent<Toggle>().isOn)
        {
            AudioListener.volume = current_volume;            
        }  
    }
}
