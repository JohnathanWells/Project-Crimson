using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCreditScript : MonoBehaviour {

    public Animator creditAnimator;
    public AudioClip creditSong;
    public AudioSource source;
    public TextAsset credits;
    public TextMesh creditText;

    void Start()
    {
        string finalText = "";
        string[] lines = creditText.text.Split('\n');
        string[] bits;
        
        foreach (string line in lines)
        {
            bits = line.Split('\t');

            finalText += "\n\n" + bits[0];

            for (int n = 1; n < bits.Length; n++)
            {
                finalText += "\n" + bits[n];
            }
        }

        AudioSource[] SOURCES = FindObjectsOfType<AudioSource>();

        foreach (AudioSource s in SOURCES)
            s.Stop();

        source.PlayOneShot(creditSong);

        creditAnimator.SetTrigger("Play");
    }
    
}
