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

		UIManager.s.SetupUIEventTriggers(gameObject,
									     new EventTriggerType[] {EventTriggerType.PointerEnter, EventTriggerType.PointerExit},
										 new Action<PointerEventData>[] {OnPointerEnter, OnPointerExit});

		if (setInitialData) {
			ApplyInitialData();
		} else {
			SetDefaultData();
		}

        PlayerUIItemModule.s.ProcessAddedUIItem(this);
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
		if (CatalogManager.s.typeToData.ContainsKey(GetType())) {
			UIItemData uiItemData = CatalogManager.s.typeToData[GetType()] as UIItemData;
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
			addTooltip.SetData(new TooltipData("???", "???", "???", color: (CatalogManager.s.typeToData[GetType()] as UIItemData).tooltipData.color), true);
			addTooltip.MouseEnter();
		} else if (!amnesiaApplies && addTooltip.tooltipData.name == "???") {
			addTooltip.SetData((CatalogManager.s.typeToData[GetType()] as UIItemData).tooltipData, true);
			addTooltip.MouseEnter();
		}
	}
	protected virtual void OnPointerExit(PointerEventData data) {
	}
	public virtual void Modify(ref Modifiers modifiers) {
	}
    protected virtual void OnDestroy() {
        PlayerUIItemModule.s.ProcessRemovedUIItem(this);
    }
}
