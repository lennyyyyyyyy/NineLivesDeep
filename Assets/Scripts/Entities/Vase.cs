using UnityEngine;
using System;
using System.Collections.Generic;

public class Vase : Entity {
    public override bool IsInteractable() {
        return false;
    }
    private void OnExplosionAtCoord(int x, int y, GameObject source) {
        if (Mathf.Abs(x - GetCoord().x) <= 1 && Mathf.Abs(y - GetCoord().y) <= 1) {
            Floor.s.PlacePickupSprite(
                new FlagPool() {
                    chooseAny = new List<Type>() { typeof(Consumable) }
                },
                PickupSprite.SpawnType.RANDOM, 0, GetCoord()
            );
            Remove();
        }
    }
    private void OnEnable() {
        EventManager.s.OnExplosionAtCoord += OnExplosionAtCoord;
    }
    private void OnDisable() {
        EventManager.s.OnExplosionAtCoord -= OnExplosionAtCoord;
    }
}
