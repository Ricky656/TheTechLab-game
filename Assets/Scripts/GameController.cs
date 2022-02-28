using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class GameController : MonoBehaviour
{
    public GameObject playerCharacter;
    public Checkpoint startPoint;

    private Checkpoint currentCheckpoint;
    private int currentCheckpointIndex;
    private GameSaveData currentSaveData;

    private static GameController controller; 

    public enum GameState 
    {
        MainMenu,
        Playing,
        Paused
    }

    public void Awake()
    {
        if (!controller)
        {
            controller = this; 
        }
    }
    public void StartGame()
    {
        Debug.Log("Start Game");
        GameObject.Find("StartMenu").gameObject.SetActive(false);
        GameObject.Find("Level01").gameObject.GetComponent<Level01>().StartLevel();

        currentCheckpointIndex = 0;
        currentCheckpoint = startPoint;
        EventController.StartListening(EventController.EventType.CheckpointHit, OnCheckpointHit);
        EventController.StartListening(EventController.EventType.PlayerDied, OnPlayerDied);
    }

    public void ExitGame()
    {

    }

    private void OnCheckpointHit(object data)
    {
        Checkpoint checkpoint = (Checkpoint)data;
        Debug.Log($"Checkpoint {checkpoint.checkpointNumber} hit");
        if(checkpoint.checkpointNumber > currentCheckpointIndex)
        {
            currentCheckpoint = checkpoint;
            currentCheckpointIndex = checkpoint.checkpointNumber;
            SaveGame();
        }
    }

    private void OnPlayerDied()
    {
        StartCoroutine(CameraController.CameraFade());
        EventController.TriggerEvent(EventController.EventType.PlayerLocked);
        EventController.StartListening(EventController.EventType.CameraFadeComplete, Respawn);   
    }

    private void Respawn()
    {
        EventController.StopListening(EventController.EventType.CameraFadeComplete, Respawn);
        DialogueController.EndConversation();
        playerCharacter.GetComponent<EntanglementGun>().Disentangle();
        LoadLevel();
        CameraController.CameraMove(playerCharacter.GetComponent<CameraFlag>().gameObject);
        StartCoroutine(CameraController.CameraFade(false));
        EventController.TriggerEvent(EventController.EventType.PlayerUnlocked);
    }


    private void LoadLevel()
    {
        Debug.Log("Loading level");
        playerCharacter.GetComponent<ISaveable<PlayerData>>().Load(currentSaveData.playerData);

        var objects = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<ISaveable<ObjectData>>();
        var npcs = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<ISaveable<NPCData>>();

        foreach (MonoBehaviour obj in objects)//TODO: Refactor
        {
            foreach(ObjectData data in currentSaveData.objectData)
            {
                if(obj.gameObject.name == data.GetName())
                {
                    obj.GetComponent<ISaveable<ObjectData>>().Load(data);
                    break;
                }
            }
        }
        foreach (MonoBehaviour npc in npcs)
        {
            foreach (NPCData data in currentSaveData.npcData)
            {
                if (npc.gameObject.name == data.GetName())
                {
                    npc.GetComponent<ISaveable<NPCData>>().Load(data);
                    break;
                }
            }
        }

    }

    public static void SaveGame()
    {
        PlayerData playerData = new PlayerData(controller.playerCharacter.transform.position, controller.playerCharacter.GetComponent<PlayerController>().GetInventory());

        List<ObjectData> objectsData = new List<ObjectData>();
        List<NPCData> npcData = new List<NPCData>();

        var objects = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<ISaveable<ObjectData>>();
        foreach(ISaveable<ObjectData> obj in objects)
        {
            ObjectData data = obj.Save();
            objectsData.Add(data);
        }

        var npcs = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<ISaveable<NPCData>>();
        foreach (ISaveable<NPCData> npc in npcs)
        {
            NPCData data = npc.Save();
            npcData.Add(data);
        }

        controller.currentSaveData = new GameSaveData(playerData, objectsData, npcData);
    }
}
