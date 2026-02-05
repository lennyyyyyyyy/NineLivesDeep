using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Random = UnityEngine.Random;

public class Chocolate : Consumable {
    protected override void OnPointerClick(PointerEventData data) {
        if (usable) {
            You you = PlayerUIItemModule.s.typeToInstances[typeof(You)][0].GetComponent<You>();
            you.UpdateCount(Mathf.CeilToInt(you.count * 0.5f));
            Type randomType = PlayerUIItemModule.s.flagsUnseen[Random.Range(0, PlayerUIItemModule.s.flagsUnseen.Count)]; 
            Instantiate(PrefabManager.s.flagPrefab).AddComponent(randomType);
            randomType = PlayerUIItemModule.s.flagsUnseen[Random.Range(0, PlayerUIItemModule.s.flagsUnseen.Count)];
            Instantiate(PrefabManager.s.flagPrefab).AddComponent(randomType);
            UpdateCount(count - 1);
        }
    }
}
