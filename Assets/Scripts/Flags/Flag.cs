using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class Flag : UIItem {
	public Type placeableSpriteType;	
	public bool placeableRemovesMines,
                placeableReplenish;
	public int defaultCount;
    public bool showCount;
	public List<string> allowedFloorTypes;

    public int count;

    protected TMP_Text tmpro;
    [System.NonSerialized]
    public HashSet<Taken> takenBy = new HashSet<Taken>();

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
                            bool? placeableReplenish = null,
                            int? defaultCount = null, 
                            bool? showCount = null,
                            int? initialCount = null,
                            List<string> allowedFloorTypes = null) {
		this.tex2d = tex2d ?? this.tex2d;
		this.tooltipData = tooltipData ?? this.tooltipData;
		this.placeableSpriteType = placeableSpriteType ?? this.placeableSpriteType;
		this.placeableRemovesMines = placeableRemovesMines ?? this.placeableRemovesMines;
        this.placeableReplenish = placeableReplenish ?? this.placeableReplenish;
		this.defaultCount = defaultCount ?? this.defaultCount;
		this.showCount = showCount ?? this.showCount;
        this.count = initialCount ?? this.count;
		this.allowedFloorTypes = allowedFloorTypes ?? this.allowedFloorTypes;
		GetComponent<RawImage>().texture = this.tex2d;
		addTooltip.Init(this.tooltipData);
        UpdateCount(this.count);
        tmpro.enabled = this.showCount;
    }
    public override void Init() {
        if (CatalogManager.s.typeToData.ContainsKey(GetType())) {
            FlagData flagData = CatalogManager.s.typeToData[GetType()] as FlagData;
            Init(tex2d: flagData.tex2d,
                 tooltipData: flagData.tooltipData,
                 placeableSpriteType: flagData.placeableSpriteType,
                 placeableRemovesMines: flagData.placeableRemovesMines,
                 placeableReplenish: flagData.placeableReplenish,
                 defaultCount: flagData.defaultCount,
                 showCount: flagData.showCount,
                 initialCount: flagData.defaultCount,
                 allowedFloorTypes: flagData.allowedFloorTypes);
        }
    }
    protected virtual void Update() {
        if (usable && addTooltip.hovered) {
            UIManager.s.cursorInteract = true;
        }
    }
    public virtual void UpdateCount(int newCount) {
        HelperManager.s.InstantiateBubble(gameObject, (newCount - count >= 0 ? "+" : "-") + Mathf.Abs(newCount - count).ToString(), Color.white);
        count = newCount;
        tmpro.text = count.ToString();
    }
    protected override bool IsUsable() {
        return Player.s.alive && takenBy.Count == 0 && allowedFloorTypes.Contains(Floor.s.floorType);
    }
    protected override void OnDestroy() {
        base.OnDestroy();
        foreach (Taken taken in takenBy) {
            taken.takenFlags.Remove(this); 
        }
        if (GameManager.s.gameState == GameManager.GameState.GAME) {
            PlayerUIItemModule.s.ProcessRemovedFlag(this);
        }
    }
}
