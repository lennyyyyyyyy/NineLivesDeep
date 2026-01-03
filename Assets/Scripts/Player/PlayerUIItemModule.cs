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
						 minesSeen = new HashSet<Type>(); // not in use but holding on to these
	public List<Type> flagsUnseen, consumableFlagsUnseen, cursesUnseen, minesUnseen;

    // unsaved data
    public Dictionary<Type, List<GameObject>> typeToInstances = new Dictionary<Type, List<GameObject>>();

    private void Awake() {
        s = this;
    }
	public void InitializeUnseenFlags() {
		flagsUnseen = CatalogManager.s.allFlagTypes.ToList();
		consumableFlagsUnseen = CatalogManager.s.allFlagTypes.Where(type => typeof(Consumable).IsAssignableFrom(type)).ToList();
		cursesUnseen = CatalogManager.s.allCurseTypes.ToList();
		minesUnseen = CatalogManager.s.allMineTypes.ToList();
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
    public void NoticeFlag(Type type) {
		flagsSeen.Add(type);
		flagsUnseen.Remove(type);
		consumableFlagsUnseen.Remove(type);
    }
    public void ProcessAddedFlag(Flag flag) {
        flags.Add(flag.gameObject);
        Type type = flag.GetType();
        NoticeFlag(type);
    }
    public void ProcessRemovedFlag(Flag flag) {
        flags.Remove(flag.gameObject);
    }
    public void ProcessAddedCurse(Curse curse) {
        curses.Add(curse.gameObject);
        notFlags.Add(curse.gameObject);
        Type type = curse.GetType();
		cursesSeen.Add(type);
		cursesUnseen.Remove(type);
    }
    public void ProcessRemovedCurse(Curse curse) {
        curses.Remove(curse.gameObject);
    }
    public void ProcessAddedMine(Mine mine) {
        mines.Add(mine.gameObject);
        notFlags.Add(mine.gameObject);
        Type type = mine.GetType();
        minesSeen.Add(type);
        minesUnseen.Remove(type);
    }
    public void ProcessRemovedMine(Mine mine) {
        mines.Remove(mine.gameObject);
    }
}
