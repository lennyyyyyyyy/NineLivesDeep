using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Consumable : Flag {
    protected override void Start() {
        base.Start();

		EventTrigger trigger;
		EventTrigger.Entry entry;
        trigger = GetComponent<EventTrigger>() == null ? gameObject.AddComponent<EventTrigger>() : GetComponent<EventTrigger>();
        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerClick};
        entry.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }
    protected virtual void OnPointerClick(PointerEventData data) {}
}
