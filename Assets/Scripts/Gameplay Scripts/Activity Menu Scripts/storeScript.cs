using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class storeScript : MonoBehaviour {

    public TextMesh storeName;
    public GameManager manager;
    public ShopClass store;
    public amountButton[] shoppingButtons = new amountButton[4];
    public TextMesh[] stockText = new TextMesh[4];
    public TextMesh[] totalCosts = new TextMesh[4];
    public TextMesh[] itemNames = new TextMesh[4];
    public TextMesh receipt;
    public Transform purchaseButton;
    int[] selectedUnits = new int[4];
    int[] totalCostN = new int[4];

    public void setInitialValues(ShopClass shop)
    {
        store = shop;

        storeName.text = store.shopName;
        
        for (int n = 0; n < 4; n++)
        {
            stockText[n].text = "/ " + store.inventory[n].getStock();
            itemNames[n].text = store.inventory[n].getType().ToString();
            selectedUnits[n] = 0;
            totalCosts[n].text = 0 + "$";
            shoppingButtons[n].SendMessage("updateNumber", 0, SendMessageOptions.RequireReceiver);
        }

        receipt.text = manager.houseStats.getMoney() + "\n -0$\n " + manager.houseStats.getMoney();
    }

    public void changeValues(arrowButtonClass caller)
    {
        int ID = caller.id;
        int tempCost;
        int tempTotal;

        if (selectedUnits[ID] + caller.change <= store.inventory[ID].getStock())
        {
            selectedUnits[ID] += caller.change;
            shoppingButtons[ID].SendMessage("updateNumber", selectedUnits[ID]);
            stockText[ID].text = "/ " + (store.inventory[ID].getStock() - selectedUnits[ID]);
            totalCostN[ID] = selectedUnits[ID] * store.inventory[ID].getCost();
            totalCosts[ID].text = totalCostN[ID] + "$";


            tempTotal = calculateTotal();
            tempCost = manager.houseStats.getMoney() - tempTotal;

            if (tempCost < 0)
            {
                receipt.text = manager.houseStats.getMoney() + "\n -" + tempTotal + "$\n <color=red>" + tempCost + "</color>";
                purchaseButton.gameObject.SetActive(false);
            }
            else
            {
                receipt.text = manager.houseStats.getMoney() + "\n -" + tempTotal + "$\n <color=white>" + tempCost + "</color>";
                purchaseButton.gameObject.SetActive(true);
            }            
        }


    }

    public void purchaseItems(int justbecause)
    {

        manager.houseStats.modFood(selectedUnits[0]);
        store.inventory[0].buyItem(selectedUnits[0]);

        manager.houseStats.modCleaning(selectedUnits[1]);
        store.inventory[1].buyItem(selectedUnits[1]);

        manager.houseStats.modHygiene(selectedUnits[2]);
        store.inventory[2].buyItem(selectedUnits[2]);

        manager.houseStats.modMed(selectedUnits[3]);
        store.inventory[3].buyItem(selectedUnits[3]);

        manager.houseStats.modMoney(-calculateTotal());

        manager.SendMessage("finishShopping");
    }

    int calculateTotal()
    {
        int total = 0;

        for (int n = 0; n < selectedUnits.Length; n++)
        {
            total += (selectedUnits[n] * store.inventory[n].getCost());
        }

        return total;
    }

    
}
