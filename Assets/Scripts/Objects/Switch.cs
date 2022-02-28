using UnityEngine.Events;
using UnityEngine;

public class Switch : MonoBehaviour, IInteractable
{
    public void Interact(GameObject obj)
    {
        GetComponent<Animator>().SetTrigger("switch");
        EventController.TriggerEvent(EventController.EventType.SwitchFlipped);
    }
}
