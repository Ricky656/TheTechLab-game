using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData 
{
    private float[] positionData;
    private string[] inventoryData;

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
        foreach(string item in inventoryData)
        {
            Item newItem = new Item();
            string[] values = item.Split(',');
            newItem.identifier = values[0];
            newItem.itemName = values[1];
            newItem.pickupMessage = values[2];
            data.Add(newItem);
        }
        return data;
    }
}
