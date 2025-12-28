using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

[Serializable]
public class CurseSaveData {
    public int typeID, intensifiedTypeID;
}
[Serializable]
public class FlagSaveData {
    public int typeID;
    public int count;
    public bool usable;
}
[Serializable]
public class EntitySaveData {
    public int typeID;
    public int pickupPrice;
    public int pickupCount;
}
[Serializable]
public class TileSaveData {
    public int typeID;
    public Vector2Int coord;
    public List<int> nonPlayerEntities = new List<int>();
    public int actionCode;
}
[Serializable]
public class SaveData {
    //player data
    public Vector2Int playerCoord; 
    public bool playerAlive;
    public float money;
    public int tempChanges;
    public List<CurseSaveData> curses = new List<CurseSaveData>();
    public List<int> mines = new List<int>();
    public List<FlagSaveData> flags = new List<FlagSaveData>(); 
    public List<int> flagsUnseen = new List<int>(),
              consumableFlagsUnseen = new List<int>(),
              cursesUnseen = new List<int>(),
              minesUnseen = new List<int>();
    public List<Vector2Int> tilesVisited = new List<Vector2Int>();
    public List<Vector2Int> moveHistory;

    //floor data
    public int floor, width, height, floorDeathCount;
    public string floorType;
    public List<TileSaveData> tiles = new List<TileSaveData>();

    //misc
    public List<Vector2Int> rainCoords;
}

public class SaveManager : MonoBehaviour {
    public static SaveManager s;
    public static Action OnSave, OnLoad;

    public SaveData saveData;
    private void Awake() {
        s = this;
    }
    private void Update() {
        if (CanSave() && Input.GetKeyDown("s")) {
            Save();
        }
    }
    public bool CanSave() {
        return true;
    }
    public void Save() {
        OnSave?.Invoke();
        saveData = new SaveData() {
            playerCoord = Player.s.GetCoord(),
            playerAlive = Player.s.alive,
            money = Player.s.money,
            tempChanges = Player.s.tempChanges,
            floor = Floor.s.floor,
            width = Floor.s.width,
            height = Floor.s.height,
            floorType = Floor.s.floorType,
            floorDeathCount = Floor.s.floorDeathCount,
            rainCoords = Raincloud.rainCoords.ToList(),
            moveHistory = Player.s.moveHistory.ToList(),
        };
        foreach (GameObject g in Player.s.curses) {
            Curse c = g.GetComponent<Curse>();
            CurseSaveData data = new CurseSaveData() {
                typeID = CatalogManager.s.typeToData[c.GetType()].id
            };
            if (c is Intensify) {
                data.intensifiedTypeID = CatalogManager.s.typeToData[((Intensify)c).intensifiedCurse.GetType()].id;
            }
            saveData.curses.Add(data);
        }
        foreach (GameObject g in Player.s.mines) {
            Mine m = g.GetComponent<Mine>();
            saveData.mines.Add(CatalogManager.s.typeToData[m.GetType()].id);
        }
        foreach (GameObject g in Player.s.flags) {
            Flag f = g.GetComponent<Flag>();
            FlagSaveData data = new FlagSaveData() {
                typeID = CatalogManager.s.typeToData[f.GetType()].id,
                count = f.count,
                usable = f.usable
            };
            saveData.flags.Add(data);
        }
        foreach (Type t in Player.s.flagsUnseen) {
            saveData.flagsUnseen.Add(CatalogManager.s.typeToData[t].id);
        }
        foreach (Type t in Player.s.consumableFlagsUnseen) {
            saveData.consumableFlagsUnseen.Add(CatalogManager.s.typeToData[t].id);
        }
        foreach (Type t in Player.s.cursesUnseen) {
            saveData.cursesUnseen.Add(CatalogManager.s.typeToData[t].id);
        }
        foreach (Type t in Player.s.minesUnseen) {
            saveData.minesUnseen.Add(CatalogManager.s.typeToData[t].id);
        }
        foreach (GameObject tile in Player.s.tilesVisited) {
            saveData.tilesVisited.Add(tile.GetComponent<Tile>().coord);
        }
        foreach (GameObject tile in Floor.s.tiles.Values) {
            Tile t = tile.GetComponent<Tile>();
            TileSaveData data = new TileSaveData() {
                typeID = CatalogManager.s.typeToData[t.GetType()].id,
                coord = t.coord,
            };
            if (t is ActionTile) {
                data.actionCode = ((ActionTile)t).actionCode;
            }
            foreach (GameObject entity in t.entities) {
                if (entity != Player.s.gameObject) {
                    Entity e = entity.GetComponent<Entity>();
                    data.nonPlayerEntities.Add(CatalogManager.s.typeToData[e.GetType()].id);
                }
            }
            saveData.tiles.Add(data);
        }
        string json = JsonUtility.ToJson(saveData, true);
        string filepath = Application.persistentDataPath + "/savefile.json";
        File.WriteAllText(filepath, json);
        Debug.Log("Saved to " + filepath);
    }
    public void Load() {
        OnLoad?.Invoke();
    }
}

