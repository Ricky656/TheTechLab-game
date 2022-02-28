using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class NPCData : ObjectData
{
    public int[] activeConvoPointers;
    public int[] doneConvoPointers;
    public int[] attackedConvoPointers;
    public int[] deathConvoPointers;

    public NPCData(string name, Vector3 position, bool active) : base(name, position, active)
    {

    }

}
