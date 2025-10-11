using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Placeable : Flag {
    [System.NonSerialized]
    public GameObject sprite;
    protected virtual void OnPointerDown(PointerEventData data) {
        if (usable) {
            sprite = Instantiate(GameManager.s.flagSprite_p, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
            sprite.AddComponent(spriteType);
            sprite.GetComponent<FlagSprite>().parent = this;
            base.OnPointerExit(null);
        }
    }
    protected override bool IsUsable() {
        return base.IsUsable() && count > 0 && (sprite == null || sprite.GetComponent<FlagSprite>().state != "held");
    }
    public override void UpdateCount(int newCount) {
        base.UpdateCount(newCount);
        UpdateUsable();
    }
    protected override void Start()
    {
        base.Start();
        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerDown};
        entry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }
}
