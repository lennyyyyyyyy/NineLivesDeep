using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Consumable : Flag
{
    protected virtual void OnPointerClick(PointerEventData data) {
        
    }
    protected override void Start()
    {
        base.Start();
        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerClick};
        entry.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

}
