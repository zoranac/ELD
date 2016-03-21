using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class MySaveGame : SaveGame
{
    public List<GameObject> Objects = new List<GameObject>();
}

