using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameSaveData 
{

    public PlayerData playerData;
    public List<ObjectData> objectData;
    public List<NPCData> npcData;
    public List<LevelData> levelData;

    public GameSaveData(PlayerData player, List<ObjectData> objects, List<NPCData> npcs, List<LevelData> levels)
    {
        playerData = player;
        objectData = objects;
        npcData = npcs;
        levelData = levels;
    }

}
