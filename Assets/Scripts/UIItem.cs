using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIItem : MonoBehaviour {
	public bool setInitialData = false;
	public Texture2D tex2d;
	public TooltipData tooltipData;

	protected AddTooltipUI addTooltip;

	[System.NonSerialized]
	public RectTransform rt;

	protected virtual void Start() {
		rt = (transform as RectTransform);
		addTooltip = (GetComponent<AddTooltipUI>() == null ? gameObject.AddComponent(typeof(AddTooltipUI)) as AddTooltipUI : GetComponent<AddTooltipUI>());

		EventTrigger trigger;
		EventTrigger.Entry entry;
        trigger = GetComponent<EventTrigger>() == null ? gameObject.AddComponent<EventTrigger>() : GetComponent<EventTrigger>();
        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerEnter};
        entry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerExit};
        entry.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        trigger.triggers.Add(entry);

		if (setInitialData) {
			ApplyInitialData();
		} else {
			SetDefaultData();
		}

		if (UIManager.s.uiTypeToData.ContainsKey(GetType())) {
			UIManager.s.uiTypeToData[GetType()].instances.Add(gameObject);
		}
	}		
	public virtual void SetInitialData(Texture2D? tex2d = null, TooltipData tooltipData = null) {
		setInitialData = true;
		this.tex2d = tex2d ?? this.tex2d;
		this.tooltipData = tooltipData ?? this.tooltipData; 
	}
	protected virtual void ApplyInitialData() {
		GetComponent<RawImage>().texture = tex2d;
		addTooltip.SetData(tooltipData);
	}
    public virtual void SetData(Texture2D? tex2d = null, TooltipData tooltipData = null) {
		SetInitialData(tex2d, tooltipData);
		ApplyInitialData();
    }
	protected virtual void SetDefaultData() {
		if (UIManager.s.uiTypeToData.ContainsKey(GetType())) {
			UIItemData uiItemData = UIManager.s.uiTypeToData[GetType()];
			SetData(uiItemData.tex2d, uiItemData.tooltipData);
		}
	}
	protected virtual void OnPointerEnter(PointerEventData data) {
		//amensia curse passive
		bool amnesiaApplies = false;
		foreach (Type t in Player.s.modifiers.amnesiaUITypes) {
			if (t.IsAssignableFrom(GetType())) {
				amnesiaApplies = true;
				break;
			}
		}
		if (amnesiaApplies && addTooltip.tooltipData.name != "???") {
			addTooltip.SetData(new TooltipData("???", "???", "???", color: UIManager.s.uiTypeToData[GetType()].tooltipData.color), true);
			addTooltip.MouseEnter();
		} else if (!amnesiaApplies && addTooltip.tooltipData.name == "???") {
			addTooltip.SetData(UIManager.s.uiTypeToData[GetType()].tooltipData, true);
			addTooltip.MouseEnter();
		}
	}
	protected virtual void OnPointerExit(PointerEventData data) {
	}
	public virtual void Modify(ref Modifiers modifiers) {
	}
}
