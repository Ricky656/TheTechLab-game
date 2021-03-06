using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData 
{
    private float[] positionData;
    private string[] inventoryData;
    private bool hasGun;

    public PlayerData(Vector3 position, List<Item> inventory)
    {
        positionData = new float[3];
        positionData[0] = position.x;
        positionData[1] = position.y;
        positionData[2] = position.z;

        inventoryData = new string[inventory.Count];
        int index = 0; 
        foreach(Item item in inventory)
        {
            if(item.identifier == "gun")
            {
                hasGun = true;
            }
            string itemData = $"{item.identifier},{item.itemName},{item.pickupMessage}";
            inventoryData[index] = itemData;
            index++;
        }
    }
    public Vector3 GetPosition()
    {
        return new Vector3(positionData[0], positionData[1], positionData[2]);
    }

    public List<Item> GetInventory()
    {
        List<Item> data = new List<Item>();
        Item[] existingItems = Resources.FindObjectsOfTypeAll<Item>();
        int index = 0;
        foreach (string item in inventoryData)
        {
            string[] values = item.Split(',');
            //identifier = values[0];
            //itemName = values[1];
            //pickupMessage = values[2];
            if (existingItems[index].identifier == values[0])
            {
                data.Add(existingItems[index]);
            }
            index++;
        }
        return data;
    }

    public bool HasGun()
    {
        return hasGun;
    }
}
