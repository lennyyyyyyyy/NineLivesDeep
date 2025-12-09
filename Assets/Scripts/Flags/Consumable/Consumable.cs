using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Consumable : Flag {
    protected override void Start() {
        base.Start();

		UIManager.s.SetupUIEventTriggers(gameObject,
									     new EventTriggerType[] {EventTriggerType.PointerClick},
										 new Action<PointerEventData>[] {OnPointerClick});
    }
    protected virtual void OnPointerClick(PointerEventData data) {}
}
