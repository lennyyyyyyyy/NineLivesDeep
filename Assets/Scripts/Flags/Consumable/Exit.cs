using UnityEngine;
using UnityEngine.EventSystems;

public class Exit : Consumable {
    protected override void OnPointerClick(PointerEventData data) {
        if (usable) {
            EventManager.s.OnGameExit?.Invoke(); 
        }
    }
}
