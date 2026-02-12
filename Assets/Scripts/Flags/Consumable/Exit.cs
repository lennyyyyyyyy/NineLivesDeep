using UnityEngine;
using UnityEngine.EventSystems;

public class Exit : Consumable {
    protected override void OnPointerClick(PointerEventData data) {
        if (!usable) return;
        EventManager.s.OnGameExit?.Invoke(); 
    }
}
