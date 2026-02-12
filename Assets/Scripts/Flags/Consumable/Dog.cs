using UnityEngine;
using UnityEngine.EventSystems;

public class Dog : Consumable {
    protected override void OnPointerClick(PointerEventData data) {
        if (!usable) return;
        if (Player.s.dogFlagActive) {
            HelperManager.s.InstantiateBubble(gameObject, "Already active!", Color.white);
        } else {
            Player.s.dogFlagActive = true;
            UpdateCount(count - 1);
        }
    }
}
