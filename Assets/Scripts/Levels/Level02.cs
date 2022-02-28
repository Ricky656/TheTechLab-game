using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level02 : Level
{
    public NPC emo;
    public GameObject player;

    public void Start()
    {
        emo.AddActiveConversation("01", this.GetType().ToString());
        emo.AddDoneConversation("done01", this.GetType().ToString());
        emo.AddDoneConversation("done02", this.GetType().ToString());
        emo.AddAttackedConversation("attacked01", this.GetType().ToString());
        emo.AddDeathConversation("death01", this.GetType().ToString());

        EventController.StartListening(EventController.EventType.DoorOpen, LevelComplete);
    }

    private void LevelComplete()
    {
        EventController.StopListening(EventController.EventType.DoorOpen, LevelComplete);
        emo.ClearActiveConversations();
        emo.ClearDoneConversations();
        emo.ClearAttackedConversations();
        emo.AddAttackedConversation("attacked02", this.GetType().ToString());
        emo.AddDoneConversation("done03", this.GetType().ToString());
    }
}
