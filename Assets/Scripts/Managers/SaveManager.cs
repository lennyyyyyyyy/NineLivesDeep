using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

[Serializable]
public class CurseSaveData {
    public int typeID;
    public int intensifiedCurseIndex = -1;
}
[Serializable]
public class NumberSaveData {
    public Vector2Int coord;
    public int num;
}
[Serializable]
public class FlagSaveData {
    public int typeID;
    public int count;
    public bool usable;
    public List<NumberSaveData> numbers = new List<NumberSaveData>();
}
[Serializable]
public class EntitySaveData {
    public int typeID;
    // flag sprites - nothing extra
    // mine sprites - nothing extra
    // pickup sprites
    public int correspondingUITypeID;
    public int pickupPrice;
    public int pickupCount;
    // misc entities     
    public bool crankDirection;
}
[Serializable]
public class TileSaveData {
    public int typeID;
    public Vector2Int coord;
    public List<EntitySaveData> nonPlayerEntities = new List<EntitySaveData>();
    public int oneUpCount;
    // action tiles
    public ActionTile.ActionCode actionCode;
    public int amount;
}
[Serializable]
public class SaveData {
    //player data
    public Vector2Int playerCoord; 
    public bool playerAlive;
    public float money;
    public int tempChanges;
    public int floorDeathCount;
    public List<CurseSaveData> curses = new List<CurseSaveData>();
    public List<int> mines = new List<int>();
    public List<FlagSaveData> flags = new List<FlagSaveData>(); 
    public List<int> flagsSeen = new List<int>(),
                     cursesSeen = new List<int>(),
                     minesSeen = new List<int>(),
                     minesDefused = new List<int>(); 
    public List<Vector2Int> tilesVisited = new List<Vector2Int>();
    public List<Vector2Int> moveHistory;
    public List<Vector2Int> rainCoords;
    public bool tunneledLastMove;

    //floor data
    public int floor, width, height;
    public string floorType;
    public List<TileSaveData> tiles = new List<TileSaveData>();
    public float floorStartTime;
}
public class CurseLoadData {
    public Type type;
    public int intensifiedCurseIndex;
}
public class NumberLoadData {
    public Vector2Int coord;
    public int num;
}
public class FlagLoadData {
    public Type type;
    public int count;
    public bool usable;
    public List<NumberLoadData> numbers = new List<NumberLoadData>();
}
public class EntityLoadData {
    public Type type;
    // flag sprites - nothing extra
    // mine sprites - nothing extra
    // pickup sprites
    public Type correspondingUIType;
    public int pickupPrice;
    public int pickupCount;
    // misc entities
    public bool crankDirection;
}
public class TileLoadData {
    public Type type;
    public Vector2Int coord;
    public List<EntityLoadData> nonPlayerEntities = new List<EntityLoadData>();
    public int oneUpCount;
    // action tiles
    public ActionTile.ActionCode actionCode;
    public int amount;
}
public class LoadData {
    //player data
    public Vector2Int playerCoord; 
    public bool playerAlive;
    public float money;
    public int tempChanges;
    public int floorDeathCount;
    public List<CurseLoadData> curses = new List<CurseLoadData>();
    public List<Type> mines = new List<Type>();
    public List<FlagLoadData> flags = new List<FlagLoadData>(); 
    public List<Type> flagsSeen = new List<Type>(),
                         cursesSeen = new List<Type>(),
                         minesSeen = new List<Type>(),
                         minesDefused = new List<Type>();
    public List<Vector2Int> tilesVisited = new List<Vector2Int>();
    public List<Vector2Int> moveHistory;
    public List<Vector2Int> rainCoords;
    public bool tunneledLastMove;

    //floor data
    public int floor, width, height;
    public string floorType;
    public List<TileLoadData> tiles = new List<TileLoadData>();
    public float floorStartTime;
}
public class SaveManager : MonoBehaviour {
    public static SaveManager s;

    [System.NonSerialized]
    public bool saveDataValid = false;
    public SaveData saveData = null;

    private string filepath;

    private void Awake() {
        s = this;
        filepath = Application.persistentDataPath + "/saveData.json";
    }
    private void Start() {
        CheckSaveDataValidity();
    }
    private void CheckSaveDataValidity() {
        saveDataValid = false;
        if (File.Exists(filepath)) {
            string json = File.ReadAllText(filepath);
            try {
                saveData = JsonUtility.FromJson<SaveData>(json);
                saveDataValid = true;
            } catch (Exception e) {}
        }
        ContinueButton.s.gameObject.SetActive(saveDataValid);
    }
    public void Save() {
        saveData = new SaveData() {
            playerCoord = Player.s.GetCoord(),
            playerAlive = Player.s.alive,
            money = Player.s.money,
            tempChanges = Player.s.tempChanges,
            floorDeathCount = Player.s.floorDeathCount,
            floor = Floor.s.floor,
            width = Floor.s.width,
            height = Floor.s.height,
            floorType = Floor.s.floorType,
            rainCoords = Floor.s.rainCoords.ToList(),
            moveHistory = Player.s.moveHistory.ToList(),
            tunneledLastMove = Player.s.tunneledLastMove,
            floorStartTime = Time.time - Floor.s.floorStartTime
        };
        foreach (GameObject g in PlayerUIItemModule.s.curses) {
            Curse c = g.GetComponent<Curse>();
            CurseSaveData data = new CurseSaveData() {
                typeID = CatalogManager.s.typeToData[c.GetType()].id
            };
            if (c is Intensify) {
                Intensify intensify = c as Intensify;
                if (intensify.intensifiedCurse != null) {
                    data.intensifiedCurseIndex = PlayerUIItemModule.s.curses.IndexOf(intensify.intensifiedCurse.gameObject);
                } else {
                    data.intensifiedCurseIndex = -1;
                }
            }
            saveData.curses.Add(data);
        }
        foreach (GameObject g in PlayerUIItemModule.s.mines) {
            Mine m = g.GetComponent<Mine>();
            saveData.mines.Add(CatalogManager.s.typeToData[m.GetType()].id);
        }
        foreach (GameObject g in PlayerUIItemModule.s.flags) {
            Flag f = g.GetComponent<Flag>();
            FlagSaveData data = new FlagSaveData() {
                typeID = CatalogManager.s.typeToData[f.GetType()].id,
                count = f.count,
                usable = f.usable
            };
            if (f is Map) {
                foreach (GameObject number in (f as Map).numbers.Values) {
                    Number n = number.GetComponent<Number>();
                    NumberSaveData nData = new NumberSaveData() {
                        coord = n.coord,
                        num = n.num
                    };
                    data.numbers.Add(nData);
                }
            }
            saveData.flags.Add(data);
        }
        foreach (Type t in PlayerUIItemModule.s.flagsSeen) {
            saveData.flagsSeen.Add(CatalogManager.s.typeToData[t].id);
        }
        foreach (Type t in PlayerUIItemModule.s.cursesSeen) {
            saveData.cursesSeen.Add(CatalogManager.s.typeToData[t].id);
        }
        foreach (Type t in PlayerUIItemModule.s.minesSeen) {
            saveData.minesSeen.Add(CatalogManager.s.typeToData[t].id);
        }
        foreach (Type t in PlayerUIItemModule.s.minesDefused) {
            saveData.minesDefused.Add(CatalogManager.s.typeToData[t].id);
        }
        foreach (GameObject tile in Player.s.tilesVisited) {
            saveData.tilesVisited.Add(tile.GetComponent<Tile>().coord);
        }
        foreach (GameObject tile in Floor.s.tiles.Values) {
            Tile t = tile.GetComponent<Tile>();
            TileSaveData data = new TileSaveData() {
                typeID = CatalogManager.s.typeToData[t.GetType()].id,
                coord = t.coord,
                oneUpCount = t.oneUpCount
            };
            if (t is ActionTile) {
                data.actionCode = ((ActionTile)t).actionCode;
                data.amount = ((ActionTile)t).amount;
            }
            foreach (GameObject entity in t.entities) {
                if (entity != Player.s.gameObject) {
                    Entity e = entity.GetComponent<Entity>();
                    EntitySaveData edata = new EntitySaveData() {
                        typeID = CatalogManager.s.typeToData[e.GetType()].id
                    };
                    if (e is PickupSprite) {
                        PickupSprite ps = (PickupSprite) e;
                        edata.correspondingUITypeID = CatalogManager.s.typeToData[ps.correspondingUIType].id;
                        edata.pickupPrice = ps.price;
                        edata.pickupCount = ps.count;
                    } else if (e is Crank) {
                        edata.crankDirection = ((Crank) e).direction;
                    }
                    data.nonPlayerEntities.Add(edata);
                }
            }
            saveData.tiles.Add(data);
        }
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(filepath, json);
        Debug.Log("Saved to " + filepath);
    }
    public LoadData GetLoadData() {
        if (!saveDataValid) {
            Debug.LogError("No valid save data to load!");
            return null;
        }
        LoadData loadData = new LoadData() {
            playerCoord = saveData.playerCoord,
            playerAlive = saveData.playerAlive,
            money = saveData.money,
            tempChanges = saveData.tempChanges,
            floorDeathCount = saveData.floorDeathCount,
            tilesVisited = saveData.tilesVisited.ToList(),
            moveHistory = saveData.moveHistory.ToList(),
            tunneledLastMove = saveData.tunneledLastMove,
            rainCoords = saveData.rainCoords.ToList(),
            floor = saveData.floor,
            width = saveData.width,
            height = saveData.height,
            floorType = saveData.floorType,
            floorStartTime = saveData.floorStartTime
        };
        foreach (CurseSaveData data in saveData.curses) {
            loadData.curses.Add(new CurseLoadData() {
                type = CatalogManager.s.idToData[data.typeID].type,
                intensifiedCurseIndex = data.intensifiedCurseIndex
            });
        }
        foreach (int typeID in saveData.mines) {
            loadData.mines.Add(CatalogManager.s.idToData[typeID].type);
        }
        foreach (FlagSaveData data in saveData.flags) {
            FlagLoadData fdata = new FlagLoadData() {
                type = CatalogManager.s.idToData[data.typeID].type,
                count = data.count,
                usable = data.usable
            };
            foreach (NumberSaveData nData in data.numbers) {
                fdata.numbers.Add(new NumberLoadData() {
                    coord = nData.coord,
                    num = nData.num
                });
            }
            loadData.flags.Add(fdata);
        }
        foreach (int typeID in saveData.flagsSeen) {
            loadData.flagsSeen.Add(CatalogManager.s.idToData[typeID].type);
        }
        foreach (int typeID in saveData.cursesSeen) {
            loadData.cursesSeen.Add(CatalogManager.s.idToData[typeID].type);
        }
        foreach (int typeID in saveData.minesSeen) {
            loadData.minesSeen.Add(CatalogManager.s.idToData[typeID].type);
        }
        foreach (int typeID in saveData.minesDefused) {
            loadData.minesDefused.Add(CatalogManager.s.idToData[typeID].type);
        }
        foreach (TileSaveData data in saveData.tiles) {
            TileLoadData tdata = new TileLoadData() {
                type = CatalogManager.s.idToData[data.typeID].type,
                coord = data.coord,
                oneUpCount = data.oneUpCount,
                actionCode = data.actionCode,
                amount = data.amount
            };
            foreach (EntitySaveData edata in data.nonPlayerEntities) {
                tdata.nonPlayerEntities.Add(new EntityLoadData() {
                    type = CatalogManager.s.idToData[edata.typeID].type,
                    correspondingUIType = CatalogManager.s.idToData[edata.correspondingUITypeID].type,
                    pickupPrice = edata.pickupPrice,
                    pickupCount = edata.pickupCount,
                    crankDirection = edata.crankDirection
                });
            }
            loadData.tiles.Add(tdata);
        }
        return loadData;
    }
    private void OnGameExit() {
        CheckSaveDataValidity();
    }
    private void OnReturnToStart() {
        File.Delete(filepath);
        CheckSaveDataValidity();
    }
    private void OnEnable() {
        EventManager.s.OnGameExit += OnGameExit;
        EventManager.s.OnReturnToStart += OnReturnToStart;
    }
    private void OnDisable() {
        EventManager.s.OnGameExit -= OnGameExit;
        EventManager.s.OnReturnToStart -= OnReturnToStart;
    }
}

