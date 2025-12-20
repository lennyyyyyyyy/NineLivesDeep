using UnityEngine;

public class Vase : Entity {
    private void OnExplosionAtCoord(int x, int y) {
        if (Mathf.Abs(x - GetCoord().x) <= 1 && Mathf.Abs(y - GetCoord().y) <= 1) {
            Floor.s.PlacePickupSprite(UIManager.s.allConsumableFlagTypes, PickupSprite.SpawnType.RANDOM, 0, GetCoord());
            Remove();
        }
    }
    private void OnEnable() {
        Floor.onExplosionAtCoord += OnExplosionAtCoord;
    }
    private void OnDisable() {
        Floor.onExplosionAtCoord -= OnExplosionAtCoord;
    }
}
