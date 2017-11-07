using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newGameClass{

    public FamilyMembers[] family;
    public bool newGame;


    public newGameClass()
    {
        newGame = false;
        family = new FamilyMembers[4];
    }

    public void setNewGame(string[] names, string lastName)
    {
        newGame = true;

        for (int n = 0; n < 4; n++)
        {
            family[n] = new FamilyMembers((FamilyMembers.famMember)n);
            family[n].firstName = names[n];
            family[n].lastName = lastName;
        }

        family[0].firstName += " (YOU)";
    }
}
