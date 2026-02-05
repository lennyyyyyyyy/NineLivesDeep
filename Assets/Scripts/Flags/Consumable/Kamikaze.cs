using UnityEngine;
using UnityEngine.EventSystems;

public class Kamikaze : Consumable {
    protected override void OnPointerClick(PointerEventData data) {
        if (usable) {
            Vector2Int playerCoord = Player.s.GetCoord();
            for (int dx = -1; dx <= 1; dx++) {
                for (int dy = -1; dy <= 1; dy++) {
                    EventManager.s.OnExplosionAtCoord?.Invoke(playerCoord.x + dx, playerCoord.y + dy);
                }
            }
            UpdateCount(count - 1);
        }
    }
}
