using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class soundScript : MonoBehaviour {

    public AudioClip[] availableSongs;
    public int currentSong = 0;
    public AudioClip creditSong;

    List<int> playlist;

    public GameManager manager;

    AudioSource source;
    AudioSource musicSource;

    bool readyForTransition;
    public musicMenuScript phoneMusicManager;
    bool randomPlaylist = false;
    

    void Awake()
    {
        //phoneMusicManager = GameObject.FindGameObjectWithTag("MusicPhoneTab").GetComponent<musicMenuScript>();
        source = gameObject.GetComponent<AudioSource>();
        musicSource = GameObject.FindGameObjectWithTag("musicSource").GetComponent<AudioSource>();
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
        if (pause)
        {
            musicSource.Pause();
        }
        else
            musicSource.UnPause();
    }

    public void togglePauseMusic(int dummy)
    {
        bool pause = musicSource.isPlaying;
        
        if (pause)
        {
            musicSource.Pause();
        }
        else
            musicSource.UnPause();
    }

    public void toggleRandom()
    {
        randomPlaylist = !randomPlaylist;
    }

    public void changeSong(int to)
    {
        currentSong = to % playlist.Count;

        if (currentSong < 0)
        {
            currentSong = playlist.Count + currentSong;

            if (currentSong < 0)
                currentSong = 0;
        }

        updateSong();

        StartCoroutine(countForTransition(availableSongs[playlist[currentSong]].length));

        musicSource.Play();
    }

    public void updateSong()
    {
        //Debug.Log(playlist[0] + ", " + playlist[1]);
        musicSource.clip = availableSongs[playlist[currentSong]];
        phoneMusicManager.SendMessage("DisplayCurrentSong", null, SendMessageOptions.DontRequireReceiver);
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
            if (!randomPlaylist)
            {
                currentSong++;
                changeSong(currentSong);
                //Debug.Log(currentSong);
            }
            else
            {
                currentSong = Random.Range(0, availableSongs.Length - 1);
                changeSong(currentSong);
            }
        }
    }

    public void NextSong()
    {
        currentSong++;
        changeSong(currentSong);
    }

    public void PrevSong()
    {
        currentSong--;
        changeSong(currentSong);
    }

    public void stopEverything()
    {

        AudioSource[] SOURCES = FindObjectsOfType<AudioSource>();

        foreach (AudioSource s in SOURCES)
            s.Stop();
    }

    IEnumerator countForTransition(float time)
    {
        readyForTransition = false;
        yield return new WaitForSeconds(time);
        readyForTransition = true;
    }
}
