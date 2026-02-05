using UnityEngine;
using UnityEngine.EventSystems;

public class Dog : Consumable {
    protected override void OnPointerClick(PointerEventData data) {
        if (usable && !Player.s.dogFlagActive) {
            Player.s.dogFlagActive = true;
            UpdateCount(count - 1);
        }
    }
}
