using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObject : MonoBehaviour, ISaveable<ObjectData>
{
    

    public ObjectData Save()
    {
        return new ObjectData(gameObject.name, transform.position, gameObject.activeSelf);
    }

    public void Load(ObjectData data)
    {
        transform.position = data.GetPosition();
        gameObject.SetActive(data.GetActive());
    }
}
