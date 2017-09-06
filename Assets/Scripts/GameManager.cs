using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public enum timeOfDay { Morning, Afternoon, Evening };

    //References
    public phoneScript Phone;
    public HouseScript House;
    public ActivityMenuScript Activities;

    //Data to pass
    public FamilyMembers[] Family = new FamilyMembers[4];
    public bool houseNotification = false;
    public bool phoneNotification = false;
    
    //Day stuff
    public DayClass currentDay;
    public timeOfDay currentTime;


    void Start()
    {
        //TODO
        //Remember to change this with a load thing once I get to that
        Family[0].member = FamilyMembers.famMember.Dad;
        Family[1].member = FamilyMembers.famMember.Mom;
        Family[2].member = FamilyMembers.famMember.Son;
        Family[3].member = FamilyMembers.famMember.Dau;
        ActivityClass[] temp = new ActivityClass[5];
        currentDay = new DayClass(1, 5, 3);
        currentTime = timeOfDay.Afternoon;
        temp[0] = new ActivityClass("AAAAA", ActivityClass.sector.A, ActivityClass.category.Work);
        temp[1] = new ActivityClass("EEEEE", ActivityClass.sector.B, ActivityClass.category.Family);
        temp[2] = new ActivityClass("IIIII", ActivityClass.sector.C, ActivityClass.category.Shopping);
        temp[3] = new ActivityClass("OOOOO", ActivityClass.sector.D, ActivityClass.category.Shopping);
        temp[4] = new ActivityClass("UUUUU", ActivityClass.sector.E, ActivityClass.category.Work);
        currentDay.MorningActivities = temp;
        currentDay.AfternoonActivities = temp;
        currentDay.EveningActivities = temp;
        //currentTime = timeOfDay.Afternoon;

        transitionDay();
    }

    void transitionDay()
    {
        //TODO
        //Everything else

        Phone.SendMessage("updatePhone");
        House.SendMessage("familyUpdate", Family);
        Activities.SendMessage("updateMenu");
    }
}
