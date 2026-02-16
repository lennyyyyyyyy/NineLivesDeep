using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Chocolate : Consumable {
    protected override void OnPointerClick(PointerEventData data) {
        if (!usable) return;
        You you = PlayerUIItemModule.s.typeToInstances[typeof(You)][0].GetComponent<You>();
        you.UpdateCount(you.count/2);
        FlagPool flagPool = new FlagPool() {
            chooseUnseen = new List<Type>() { typeof(Placeable), typeof(Map) },
            chooseAny = new List<Type>() { typeof(Passive), typeof(Consumable) }
        };
        Instantiate(PrefabManager.s.flagPrefab).AddComponent(PlayerUIItemModule.s.GetRandomFlag(flagPool));
        Instantiate(PrefabManager.s.flagPrefab).AddComponent(PlayerUIItemModule.s.GetRandomFlag(flagPool));
        UpdateCount(count - 1);
    }
}
