using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class CurseSaveData {
    int typeID, intensifiedTypeID;
}
[Serializable]
public class FlagSaveData {
    int typeID;
    int count;
    bool usable;
}
[Serializable]
public class EntitySaveData {
    int typeID;
    int pickupPrice;
    int pickupCount;
}
[Serializable]
public class TileSaveData {
    int typeID;
    Vector2Int coord;
    List<int> nonPlayerEntities;
}
[Serializable]
public class SaveData {
    //player data
    Vector2Int playerCoord; 
    bool playerAlive;
    float money;
    int tempChanges;
    List<CurseSaveData> curses;
    List<int> mines;
    List<FlagSaveData> flags; 
    List<int> flagsUnseen, consumableFlagsUnseen, cursesUnseen, minesUnseen;
    List<Vector2Int> tilesVisited;

    //floor data
    int floor, width, height, floorDeathCount;
    string floorType;
    List<TileSaveData> tiles;

    //misc
    List<Vector2Int> rainCoords;
}

public class SaveManager : MonoBehaviour {
    public static SaveManager s;
    public static Action OnSave, OnLoad;

    private void Awake() {
        s = this;
    }
    public void Save() {
        OnSave?.Invoke();
    }
    public void Load() {
        OnLoad?.Invoke();
    }
}

