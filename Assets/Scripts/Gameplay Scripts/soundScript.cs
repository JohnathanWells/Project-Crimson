using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class soundScript : MonoBehaviour {

    public AudioClip[] availableSongs;
    public int currentSong = 0;

    List<int> playlist;

    GameManager manager;

    AudioSource source;
    AudioSource musicSource;

    bool readyForTransition;
    

    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
        musicSource = GameObject.FindGameObjectWithTag("musicSource").GetComponent<AudioSource>();
        manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();

        playlist = new List<int>();
        manager.loadPlaylist(playlist);
        changeSong(0);

        InvokeRepeating("advanceIfNecessary", 1f, 3f);
    }

    public void playSFX(AudioClip sfx)
    {
        if (source != null)
            source.PlayOneShot(sfx);
    }

    public void stopSFX()
    {
        source.Stop();
    }

    public void togglePauseMusic(bool pause)
    {
        Debug.Log("pause music");
        if (pause)
        {
            musicSource.Pause();
        }
        else
            musicSource.UnPause();
    }

    public void changeSong(int to)
    {
        currentSong = to % playlist.Count;

        updateSong();

        StartCoroutine(countForTransition(availableSongs[playlist[currentSong]].length));

        musicSource.Play();
    }

    public void updateSong()
    {
        //Debug.Log(playlist[0] + ", " + playlist[1]);
        musicSource.clip = availableSongs[playlist[currentSong]];
    }

    public void changePlaylistTo(int[] newPL)
    {
        //Debug.Log("aaaa");
        playlist.Clear();

        foreach (int i in newPL)
        {
            playlist.Add(Mathf.Clamp(i, 0, availableSongs.Length - 1));
        }
    }

    void advanceIfNecessary()
    {
        if (!musicSource.isPlaying && readyForTransition)
        {
            currentSong++;
            changeSong(currentSong);
            Debug.Log(currentSong);
        }
    }

    IEnumerator countForTransition(float time)
    {
        readyForTransition = false;
        yield return new WaitForSeconds(time);
        readyForTransition = true;
    }
}
