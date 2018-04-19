using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndCreditScript : MonoBehaviour {

    public GameManager manager;
    public Animator creditAnimator;
    public AudioClip creditSong;
    public AudioSource source;
    public TextAsset credits;
    public Text creditText;

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

        creditText.text = finalText;

    }

    public void playCredits()
    {
        string summary = "<b>SUMMARY</b>\n";

        for (int n = 0; n < Constants.familySize; n++)
        {
            summary += manager.Family[n].firstName + " " + manager.Family[n].lastName + ":\n\t";
            if (manager.Family[n].dead)
            {
                summary += "DEAD [" + manager.Family[n].deathCause + "]";
            }
            else if (manager.Family[n].gone && !manager.Family[n].dead)
            {
                summary += "GONE";
            }
            else
            {
                summary += "ALIVE [Morale: " + manager.Family[n].psyche.ToString() + "]";
            }
            summary += "\n";
        }

        creditText.text = summary + creditText.text;

        AudioSource[] SOURCES = FindObjectsOfType<AudioSource>();

        foreach (AudioSource s in SOURCES)
            s.Stop();

        source.PlayOneShot(creditSong);

        creditAnimator.SetTrigger("Play");
    }
    
}
