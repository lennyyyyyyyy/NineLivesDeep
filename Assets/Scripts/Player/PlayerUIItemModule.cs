using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

/*
 * This module keeps track of the player's UI items (flags, curses, mines)
 * It also keeps track of seen and unseen for spawning purposes
 * Being associated with the Player, it is reloaded per run
 */
public class PlayerUIItemModule : MonoBehaviour {
    public static PlayerUIItemModule s;

    // saved data
    [System.NonSerialized]
    public List<GameObject> flags = new List<GameObject>(), 
		   				    notFlags = new List<GameObject>(), 
							curses = new List<GameObject>(),
							mines = new List<GameObject>();
    public HashSet<Type> flagsSeen = new HashSet<Type>(), 
		   				 cursesSeen = new HashSet<Type>(),
						 minesSeen = new HashSet<Type>(),
                         minesDefused = new HashSet<Type>(); 
	public List<Type> flagsUnseen,
                      consumableFlagsUnseen,
                      cursesUnseen,
                      minesUnseen,
                      minesUndefused;

    // unsaved data
    public Dictionary<Type, List<GameObject>> typeToInstances = new Dictionary<Type, List<GameObject>>();

    private void Awake() {
        s = this;
		flagsUnseen = CatalogManager.s.allFlagTypes.ToList();
		consumableFlagsUnseen = CatalogManager.s.allFlagTypes.Where(type => typeof(Consumable).IsAssignableFrom(type)).ToList();
		cursesUnseen = CatalogManager.s.allCurseTypes.ToList();
		minesUnseen = CatalogManager.s.allMineTypes.ToList();
        minesUndefused = CatalogManager.s.allMineTypes.ToList();
    }
    public void LoadUIItems(LoadData loadData) {
        foreach (Type type in loadData.flagsSeen) {
            NoticeFlag(type);
        }
        foreach (Type type in loadData.cursesSeen) {
            NoticeCurse(type);
        }
        foreach (Type type in loadData.minesSeen) {
            NoticeMine(type);
        }
        foreach (Type type in loadData.minesDefused) {
            NoticeMineDefused(type);
        }
        foreach (Type mineType in loadData.mines) {
            GameObject g = Instantiate(PrefabManager.s.minePrefab);
            Mine mine = g.AddComponent(mineType) as Mine;
        }
        foreach (FlagLoadData flagData in loadData.flags) {
            GameObject g = Instantiate(PrefabManager.s.flagPrefab);
            Flag flag = g.AddComponent(flagData.type) as Flag;
            flag.Init(initialCount: flagData.count);
            flag.usable = flagData.usable;
            if (typeof(Map).IsAssignableFrom(flagData.type)) {
                Map mapFlag = flag as Map;
                foreach (NumberLoadData numberData in flagData.numbers) {
                    mapFlag.SetNumber(numberData.coord.x, numberData.coord.y, numberData.num);
                }
            }
        }
        foreach (CurseLoadData curseData in loadData.curses) {
            GameObject g = Instantiate(PrefabManager.s.cursePrefab);
            Curse curse = g.AddComponent(curseData.type) as Curse;
            if (curseData.type == typeof(Intensify)) {
                Intensify intensify = curse as Intensify;
                if (curseData.intensifiedCurseIndex != -1) {
                    intensify.SetIntensifiedCurse(curses[curseData.intensifiedCurseIndex].GetComponent<Curse>());
                }
            } else if (curseData.type == typeof(Taken)) {
                Taken taken = curse as Taken;
                List<Flag> takenFlags = new List<Flag>();
                foreach (int index in curseData.takenFlagIndices) {
                    takenFlags.Add(flags[index].GetComponent<Flag>());
                }
                taken.SetTakenFlags(takenFlags);
            }
        }
    }
    public bool HasUIItem(Type type) {
        return typeToInstances.ContainsKey(type) && typeToInstances[type].Count > 0;
    }
    public void ProcessAddedUIItem(UIItem uiItem) {
        Type type = uiItem.GetType();
        if (!typeToInstances.ContainsKey(type)) {
            typeToInstances[type] = new List<GameObject>();
        }
        typeToInstances[type].Add(uiItem.gameObject);
    }
    public void ProcessRemovedUIItem(UIItem uiItem) {
        Type type = uiItem.GetType();
        if (typeToInstances.ContainsKey(type)) {
            typeToInstances[type].Remove(uiItem.gameObject);
            if (typeToInstances[type].Count == 0) {
                typeToInstances.Remove(type);
            }
        }
    }
    public void OrganizeFlags() {
        foreach (GameObject f in flags) {
            Flag flag = f.GetComponent<Flag>();
            flag.UpdateUsable();
        }
		Player.s.RecalculateModifiers();
        GameUIManager.s.OrganizeFlags();
    }
    public void OrganizeNotFlags() {
		//counting sort in the right order
		List<GameObject>[] sortedNotFlags = new List<GameObject>[4];
		for (int i = 0; i < 4; i++) {
			sortedNotFlags[i] = new List<GameObject>();
		}
		foreach (GameObject notFlag in notFlags) {
			UIItem uiItem = notFlag.GetComponent<UIItem>();
			if (uiItem is MineUIItem) {
				sortedNotFlags[0].Add(notFlag);
			} else if (uiItem is Mine) {
				sortedNotFlags[1].Add(notFlag);
			} else if (uiItem is Curse){
				sortedNotFlags[2].Add(notFlag);
			} else if (uiItem is Intensify) {
				sortedNotFlags[3].Add(notFlag);
			}
        }
		notFlags.Clear();
		for (int i = 0; i < sortedNotFlags.Length; i++) {
			foreach (GameObject g in sortedNotFlags[i]) {
				notFlags.Add(g);
			}
		}
		Player.s.RecalculateModifiers();
        GameUIManager.s.OrganizeNotFlags();
    }
    public void NoticeFlag(Type type) {
		flagsSeen.Add(type);
		flagsUnseen.Remove(type);
		consumableFlagsUnseen.Remove(type);
    }
    public void ProcessAddedFlag(Flag flag) {
        flags.Add(flag.gameObject);
        Type type = flag.GetType();
        NoticeFlag(type);
        OrganizeFlags();
    }
    public void ProcessRemovedFlag(Flag flag) {
        flags.Remove(flag.gameObject);
        OrganizeFlags();
    }
    public void NoticeCurse(Type type) {
        cursesSeen.Add(type);
        cursesUnseen.Remove(type);
    }
    public void ProcessAddedCurse(Curse curse) {
        curses.Add(curse.gameObject);
        notFlags.Add(curse.gameObject);
        Type type = curse.GetType();
        NoticeCurse(type);
        OrganizeNotFlags();
    }
    public void ProcessRemovedCurse(Curse curse) {
        curses.Remove(curse.gameObject);
        OrganizeNotFlags();
    }
    public void NoticeMine(Type type) {
        minesSeen.Add(type);
        minesUnseen.Remove(type);
    }
    public void NoticeMineDefused(Type type) {
        minesDefused.Add(type);
        minesUndefused.Remove(type);
    } 
    public void ProcessAddedMine(Mine mine) {
        mines.Add(mine.gameObject);
        notFlags.Add(mine.gameObject);
        Type type = mine.GetType();
        minesSeen.Add(type);
        minesUnseen.Remove(type);
        OrganizeNotFlags();
    }
    public void ProcessRemovedMine(Mine mine) {
        mines.Remove(mine.gameObject);
        OrganizeNotFlags();
    }
    public void DestroyAllUIItemsWithoutProcessing() {
        foreach (GameObject f in flags.ToList()) {
            Destroy(f);
        }
        foreach (GameObject nf in notFlags.ToList()) {
            Destroy(nf);
        }
    }
    private void OnEnable() {
        EventManager.s.OnPlayerAliveChange += OrganizeFlags;
    }
    private void OnDisable() {
        EventManager.s.OnPlayerAliveChange -= OrganizeFlags;
    }
}
