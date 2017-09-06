using UnityEngine;
using System.Collections;

[System.Serializable]
public class FamilyMembers
{
    public enum famMember {Dad, Mom, Son, Dau, unknown};
    public enum sickness { Healthy, Flu, Porcine, Dengue, Cholera, Rabies }
    public enum emotionalHealth { Healthy, Depressed, Unstable };
    public famMember member;
    public string name;
    public int morale;
    public int health;
    public int medicine;
    public sickness status;
    public emotionalHealth psyche;
    public bool mourning;
    public bool gone;

    public FamilyMembers()
    {
        member = famMember.unknown;
        morale = 100;
        health = 100;
        medicine = 0;
        mourning = false;
        gone = false;
        status = sickness.Healthy;
        psyche = emotionalHealth.Healthy;

    }
    //WIP
}