using System;
using UnityEngine;

[Serializable]
public class ObjectData
{
    private string nameData;
    private float[] positionData;
    private bool activeData;
    public ObjectData(string name, Vector3 position, bool active)
    {
        activeData = active;
        nameData = name;
        positionData =  new float[3];
        positionData[0] = position.x;
        positionData[1] = position.y;
        positionData[2] = position.z; 
    }

    public Vector3 GetPosition()
    {
        return new Vector3(positionData[0], positionData[1], positionData[2]);
    }
    public string GetName()
    {
        return nameData;
    }

    public bool GetActive()
    {
        return activeData;
    }

}
