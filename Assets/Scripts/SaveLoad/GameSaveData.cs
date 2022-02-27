using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameSaveData 
{
    public PlayerData playerData;
    public List<ObjectData> objectData;

    public GameSaveData(PlayerData player, List<ObjectData> objects)
    {
        playerData = player;
        objectData = objects;
    }

}
