using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class soundScript : MonoBehaviour {

    public AudioClip[] availableSongs;

    List<AudioClip> playlist;

    AudioSource source;
    

    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
    }

    public void playSFX(AudioClip sfx)
    {
        source.PlayOneShot(sfx);
    }

    public void changePlaylistTo(int[] newPL)
    {
        playlist.Clear();

        foreach (int i in newPL)
        {
            playlist.Add(availableSongs[Mathf.Clamp(i, 0, availableSongs.Length - 1)]);
        }
    }
}
