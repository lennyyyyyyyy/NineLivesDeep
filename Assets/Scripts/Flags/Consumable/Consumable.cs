using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Consumable : Flag
{
	protected int defaultCount = 0;
    protected virtual void OnPointerClick(PointerEventData data) {
        
    }
    protected override void init() {
        if (UIManager.s.flagUIVars.ContainsKey(GetType())) {
			FlagUIVars uivars = UIManager.s.flagUIVars[GetType()];
            init(uivars.tex2d, uivars.name, uivars.flavor, uivars.info, uivars.color, uivars.showCount, uivars.consumableDefaultCount);
        }
    }
    protected virtual void init(Texture2D tex2d, string n, string f, string i, Color c, bool showC, int consumableDefaultCount) {
        init(tex2d, n, f, i, c, showC);
		defaultCount = consumableDefaultCount;
    }
    protected override void Start()
    {
        base.Start();
        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerClick};
        entry.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

}
