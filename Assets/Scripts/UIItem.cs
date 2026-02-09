using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIItem : MonoBehaviour {
	public Texture2D tex2d;
	public TooltipData tooltipData;

	protected AddTooltipUI addTooltip;

    [System.NonSerialized]
    public bool usable = true;
	[System.NonSerialized]
	public RectTransform rt;
    
    protected virtual void BeforeInit() {
		rt = (transform as RectTransform);
		addTooltip = (GetComponent<AddTooltipUI>() == null ? gameObject.AddComponent(typeof(AddTooltipUI)) as AddTooltipUI : GetComponent<AddTooltipUI>());
		HelperManager.s.SetupUIEventTriggers(gameObject,
                                             new EventTriggerType[] {EventTriggerType.PointerEnter, EventTriggerType.PointerExit},
                                             new Action<PointerEventData>[] {OnPointerEnter, OnPointerExit});
    }
    protected virtual void AfterInit() {
        PlayerUIItemModule.s.ProcessAddedUIItem(this);
    }
    protected virtual void Awake() {
        BeforeInit();
        Init();
        AfterInit();
    }
    public virtual void Init(Texture2D? tex2d = null, TooltipData tooltipData = null) {
		this.tex2d = tex2d ?? this.tex2d;
		this.tooltipData = tooltipData ?? this.tooltipData; 
		GetComponent<RawImage>().texture = this.tex2d;
		addTooltip.Init(this.tooltipData);
	}
    public virtual void Init() {
        if (CatalogManager.s.typeToData.ContainsKey(GetType())) {
            UIItemData uiItemData = CatalogManager.s.typeToData[GetType()] as UIItemData;
            Init(tex2d: uiItemData.tex2d, tooltipData: uiItemData.tooltipData);
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
			addTooltip.Init(new TooltipData("???", "???", "???", color: (CatalogManager.s.typeToData[GetType()] as UIItemData).tooltipData.color), true);
			addTooltip.MouseEnter();
		} else if (!amnesiaApplies && addTooltip.tooltipData.name == "???") {
			addTooltip.Init((CatalogManager.s.typeToData[GetType()] as UIItemData).tooltipData, true);
			addTooltip.MouseEnter();
		}
	}
	protected virtual void OnPointerExit(PointerEventData data) {
	}
	public virtual void Modify(ref Modifiers modifiers) {
	}
    protected virtual bool IsUsable() {
        return true;
    }
    public void UpdateUsable() {
        usable = IsUsable();
    }
    protected virtual void OnDestroy() {
        if (GameManager.s.gameState == GameManager.GameState.GAME) {
            PlayerUIItemModule.s.ProcessRemovedUIItem(this); }
    }
}
