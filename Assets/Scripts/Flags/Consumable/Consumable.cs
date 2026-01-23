using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Consumable : Flag {
    protected override void Awake() {
		HelperManager.s.SetupUIEventTriggers(gameObject,
                                             new EventTriggerType[] {EventTriggerType.PointerClick},
                                             new Action<PointerEventData>[] {OnPointerClick});
        base.Awake();
    }
    protected virtual void OnPointerClick(PointerEventData data) {}
    protected override bool IsUsable() {
        return base.IsUsable() && count > 0 && GameManager.s.floorGameState == GameManager.GameState.FLOOR_STABLE;
    }
}
