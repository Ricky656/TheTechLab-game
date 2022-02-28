using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level03 : MonoBehaviour
{
    public NPC janitor;
    public GameObject player;
    void Start()
    {
        janitor.AddActiveConversation("01", this.GetType().ToString());
    }


}
