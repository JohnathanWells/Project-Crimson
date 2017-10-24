using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class houseClass{

        //Inventory
        int money;
        int foodQuantity;
        int cleaningItems;
        int hygieneItems;
        int medicineQuantity;

        //Stats
        public bool servicesPaid = true;
        public float plagueRate = 0;
        public int timeLeftForPayment;


        //Constructor
        public houseClass()
        {
            money = 100;
            foodQuantity = 100;
            cleaningItems = 5;
            hygieneItems = 5;
            medicineQuantity = 5;
        }

        //For loading
        public void copyData(houseClass to)
        {
            to.money = money;
            to.foodQuantity = foodQuantity;
            to.cleaningItems = cleaningItems;
            to.hygieneItems = hygieneItems;
            to.medicineQuantity = medicineQuantity;
            to.servicesPaid = servicesPaid;
            to.plagueRate = plagueRate;
            to.timeLeftForPayment = timeLeftForPayment;
        }

        //Service related
        public void payServices(int serviceCost, DayClass currentDay)
        {
            if (money >= serviceCost)
            {
                servicesPaid = true;
                money -= serviceCost;
            }
            else
            {
                servicesPaid = false;
            }

            timeLeftForPayment = calculateDaysLeftForServices(currentDay);
        }

        public int calculateDaysLeftForServices(DayClass currentDay)
        {
            int finalDay = 31;

            if (currentDay.month == 10 || currentDay.month == 12 || currentDay.month == 1 || currentDay.month == 3)
            {
                finalDay = 31;
            }
            else if (currentDay.month == 11)
            {
                finalDay = 30;
            }
            else if (currentDay.month == 2)
            {
                finalDay = 28;
            }

            int daysLeft = finalDay - currentDay.day;

            while ((currentDay.dayCount + daysLeft) % 7 != 0)
            {
                daysLeft++;
            }

            return daysLeft;

        }


        //Money Related

        public int getMoney()
        {
            return money;
        }

        public void addMoney(int quantity)
        {
            money += quantity;

            if (money < 0)
                money = 0;

            return;
        }

        public void calculatePlagueRate(cityClass city)
        {
            float accumulative = 0;

            if (hygieneItems == 0)
                accumulative += 20;

            if (cleaningItems == 0)
                accumulative += 20;

            if (!servicesPaid)
                accumulative += 20;

            accumulative += (city.currentFilth * 4);


            plagueRate = accumulative;
        }

        public float getPlagueRate(cityClass city)
        {
            calculatePlagueRate(city);
            return plagueRate;
        }

        //Food Related

        public void modFood(int qty)
        {
            foodQuantity += qty;

            if (foodQuantity < 0)
                foodQuantity = 0;
        }

        public int getFoodQ()
        {
            return foodQuantity;
        }


        //Cleaning Related

        public void modCleaning(int qty)
        {
            cleaningItems += qty;

            if (cleaningItems < 0)
                cleaningItems = 0;
        }

        public int getCleaningQ()
        {
            return cleaningItems;
        }


        //Hygiene Related

        public void modHygiene(int qty)
        {
            hygieneItems += qty;

            if (hygieneItems < 0)
                hygieneItems = 0;
        }

        public int getHygiene()
        {
            return hygieneItems;
        }


        //Medicine Related

        public void modMed(int qty)
        {
            medicineQuantity += qty;

            if (medicineQuantity < 0)
                medicineQuantity = 0;
        }

        public int getMedicines()
        {
            return medicineQuantity;
        }
}
