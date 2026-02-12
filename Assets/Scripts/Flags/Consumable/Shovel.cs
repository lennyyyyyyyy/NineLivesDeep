using UnityEngine;
using UnityEngine.EventSystems;

public class Shovel : Consumable {
    protected override void OnPointerClick(PointerEventData data) {
        if (!usable) return;
        Floor.s.ExitFloor("shop", Floor.s.floor + 1);
        UpdateCount(count - 1);
    }
}
