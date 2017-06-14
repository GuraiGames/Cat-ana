using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMusic : MonoBehaviour {

    private float current_volume = -1f;

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

    }
}
