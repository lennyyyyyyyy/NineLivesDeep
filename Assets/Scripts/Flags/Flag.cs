using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class Flag : UIItem {
	public Type placeableSpriteType;	
	public int consumableDefaultCount;
    public bool showCount;
	public List<string> allowedFloorTypes;

    [System.NonSerialized]
    public bool usable = true;
    public int count;
    protected TMP_Text tmpro;

    protected override void Start() {
		base.Start();
		
        tmpro = GetComponentInChildren<TMP_Text>(); // enable or disable count
        UpdateCount(count);
        tmpro.enabled = showCount;
        
		EventTrigger trigger;
		EventTrigger.Entry entry;
        trigger = GetComponent<EventTrigger>() == null ? gameObject.AddComponent<EventTrigger>() : GetComponent<EventTrigger>();
        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerEnter};
        entry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerExit};
        entry.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        trigger.triggers.Add(entry);
    
        Player.s.flags.Add(gameObject); // add to player flags
        Player.s.NoticeFlag(GetType()); // mark flag type as seen
        UIManager.s.AddPaw(); // add paw to ui
        UIManager.s.OrganizeFlags(); // organize flags in ui
    }
	public virtual void SetInitialData(Texture2D tex2d, TooltipData tooltipData, Type placeableSpriteType, int consumableDefaultCount, bool showCount, List<string> allowedFloorTypes) {
		setInitialData = true;
		this.tex2d = tex2d;
		this.tooltipData = tooltipData;
		this.placeableSpriteType = placeableSpriteType;
		this.consumableDefaultCount = consumableDefaultCount;
		this.showCount = showCount;
		this.allowedFloorTypes = allowedFloorTypes;
	}
	public virtual void SetData(Texture2D tex2d, TooltipData tooltipData, Type placeableSpriteType, int consumableDefaultCount, bool showCount, List<string> allowedFloorTypes) {
		SetInitialData(tex2d, tooltipData, placeableSpriteType, consumableDefaultCount, showCount, allowedFloorTypes);

		GetComponent<RawImage>().texture = tex2d;
		addTooltip.SetData(tooltipData, true);
    }
	protected override void SetDefaultData() {
		if (UIManager.s.uiTypeToData.ContainsKey(GetType())) {
			FlagData flagData = UIManager.s.uiTypeToData[GetType()] as FlagData;
			SetData(flagData.tex2d, flagData.tooltipData, flagData.placeableSpriteType, flagData.consumableDefaultCount, flagData.showCount, flagData.allowedFloorTypes);
		}
	}
    protected virtual void OnPointerEnter(PointerEventData data) {
		//amensia curse passive
		if (Player.s.modifiers.amnesia && addTooltip.tooltipData.name != "???") {
			addTooltip.SetData(new TooltipData("???", "???", "???", color: UIManager.s.uiTypeToData[GetType()].tooltipData.color), true);
		} else if (Player.s.modifiers.amnesia && addTooltip.tooltipData.name == "???") {
			addTooltip.SetData(UIManager.s.uiTypeToData[GetType()].tooltipData, true);
		}
    } 
    protected virtual void OnPointerExit(PointerEventData data) {
    }
    public virtual void UpdateCount(int newCount) {
        UIManager.s.InstantiateBubble(gameObject, (newCount - count >= 0 ? "+" : "-") + Mathf.Abs(newCount - count).ToString(), Color.white);
        count = newCount;
        tmpro.text = count.ToString();
    }
    protected virtual bool IsUsable() {
        return Player.s.alive && !(Player.s.modifiers.taken && Player.s.takenDisabledFlag == gameObject) && allowedFloorTypes.Contains(Floor.s.floorType);
    }
    public virtual void UpdateUsable() {
        usable = IsUsable();
    } 
}
