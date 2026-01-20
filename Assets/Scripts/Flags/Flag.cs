using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class Flag : UIItem {
	public Type placeableSpriteType;	
	public bool placeableRemovesMines;
	public int consumableDefaultCount;
    public bool showCount;
	public List<string> allowedFloorTypes;

    [System.NonSerialized]
    public bool usable = true;
    public int count;
    protected TMP_Text tmpro;

    protected override void BeforeInit() {
        base.BeforeInit();
        transform.SetParent(GameUIManager.s.flagGroup.transform, false);
        tmpro = GetComponentInChildren<TMP_Text>(); // enable or disable count
    }
    protected override void AfterInit() {
        base.AfterInit();
        PlayerUIItemModule.s.ProcessAddedFlag(this);
    }
    public virtual void Init(Texture2D? tex2d = null,
                            TooltipData tooltipData = null,
                            Type placeableSpriteType = null,
                            bool? placeableRemovesMines = null, 
                            int? consumableDefaultCount = null, 
                            bool? showCount = null,
                            int? initialCount = null,
                            List<string> allowedFloorTypes = null) {
		this.tex2d = tex2d ?? this.tex2d;
		this.tooltipData = tooltipData ?? this.tooltipData;
		this.placeableSpriteType = placeableSpriteType ?? this.placeableSpriteType;
		this.placeableRemovesMines = placeableRemovesMines ?? this.placeableRemovesMines;
		this.consumableDefaultCount = consumableDefaultCount ?? this.consumableDefaultCount;
		this.showCount = showCount ?? this.showCount;
        this.count = initialCount ?? this.count;
		this.allowedFloorTypes = allowedFloorTypes ?? this.allowedFloorTypes;
		GetComponent<RawImage>().texture = this.tex2d;
		addTooltip.Init(this.tooltipData);
        UpdateCount(this.count);
        tmpro.enabled = this.showCount;
    }
    public override void Init() {
        Debug.Log("new flag init");
        if (CatalogManager.s.typeToData.ContainsKey(GetType())) {
            FlagData flagData = CatalogManager.s.typeToData[GetType()] as FlagData;
            Init(tex2d: flagData.tex2d,
                 tooltipData: flagData.tooltipData,
                 placeableSpriteType: flagData.placeableSpriteType,
                 placeableRemovesMines: flagData.placeableRemovesMines,
                 consumableDefaultCount: flagData.consumableDefaultCount,
                 showCount: flagData.showCount,
                 initialCount: flagData.consumableDefaultCount,
                 allowedFloorTypes: flagData.allowedFloorTypes);
        }
    }
    public virtual void UpdateCount(int newCount) {
        HelperManager.s.InstantiateBubble(gameObject, (newCount - count >= 0 ? "+" : "-") + Mathf.Abs(newCount - count).ToString(), Color.white);
        count = newCount;
        tmpro.text = count.ToString();
    }
    protected virtual bool IsUsable() {
        return Player.s.alive && !Player.s.modifiers.takenFlags.Contains(gameObject) && allowedFloorTypes.Contains(Floor.s.floorType);
    }
    public virtual void UpdateUsable() {
        usable = IsUsable();
    } 
    protected override void OnDestroy() {
        base.OnDestroy();
        if (GameManager.s.gameState == GameManager.GameState.GAME) {
            PlayerUIItemModule.s.ProcessRemovedFlag(this);
        }
    }
}
