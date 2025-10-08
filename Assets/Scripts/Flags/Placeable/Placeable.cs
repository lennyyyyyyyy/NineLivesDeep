using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Placeable : Flag
{
    protected Type flagSprite;
    [System.NonSerialized]
    public GameObject sprite;
    protected override void init() {
        if (UIManager.s.flagUIVars.ContainsKey(GetType())) {
			FlagUIVars uivars = UIManager.s.flagUIVars[GetType()];
            init(uivars.tex2d, uivars.name, uivars.flavor, uivars.info, uivars.color, uivars.showCount, uivars.spriteType);
        }
    }
    protected virtual void init(Texture2D tex2d, string n, string f, string i, Color c, bool showC, Type type) {
        init(tex2d, n, f, i, c, showC);
        flagSprite = type;
    }
    protected virtual void OnPointerDown(PointerEventData data) {
        if (usable) {
            sprite = Instantiate(GameManager.s.flagSprite_p, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
            sprite.AddComponent(flagSprite);
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
