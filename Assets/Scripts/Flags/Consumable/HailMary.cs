using UnityEngine;
using UnityEngine.EventSystems;

public class HailMary : Consumable {
    protected override void OnPointerClick(PointerEventData data) {
        if (usable) {
            GameObject unvisitedTile = Player.s.tilesUnvisited[Random.Range(0, Player.s.tilesUnvisited.Count)];
            Player.s.Move(unvisitedTile, animate: false);
            UpdateCount(count - 1);
        }
    }
}
