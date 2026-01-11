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

    protected virtual void Awake() {
        transform.SetParent(GameUIManager.s.flagGroup.transform, false);
    }
    protected override void Start() {
		base.Start();
		
        tmpro = GetComponentInChildren<TMP_Text>(); // enable or disable count
        UpdateCount(count);
        tmpro.enabled = showCount;
    
        PlayerUIItemModule.s.ProcessAddedFlag(this);
    }
	public virtual void SetInitialData(Texture2D? tex2d = null,
										TooltipData tooltipData = null,
										Type placeableSpriteType = null,
										bool? placeableRemovesMines = null, 
										int? consumableDefaultCount = null, 
										bool? showCount = null,
										List<string> allowedFloorTypes = null) {
		setInitialData = true;
		this.tex2d = tex2d ?? this.tex2d;
		this.tooltipData = tooltipData ?? this.tooltipData;
		this.placeableSpriteType = placeableSpriteType ?? this.placeableSpriteType;
		this.placeableRemovesMines = placeableRemovesMines ?? this.placeableRemovesMines;
		this.consumableDefaultCount = consumableDefaultCount ?? this.consumableDefaultCount;
		this.showCount = showCount ?? this.showCount;
		this.allowedFloorTypes = allowedFloorTypes ?? this.allowedFloorTypes;
	}
	public virtual void SetData(Texture2D? tex2d = null,
								 TooltipData tooltipData = null,
								 Type placeableSpriteType = null,
								 bool? placeableRemovesMines = null, 
								 int? consumableDefaultCount = null, 
								 bool? showCount = null,
								 List<string> allowedFloorTypes = null) {
		SetInitialData(tex2d, tooltipData, placeableSpriteType, placeableRemovesMines, consumableDefaultCount, showCount, allowedFloorTypes);
		ApplyInitialData();
    }
	protected override void SetDefaultData() {
		if (CatalogManager.s.typeToData.ContainsKey(GetType())) {
			FlagData flagData = CatalogManager.s.typeToData[GetType()] as FlagData;
			SetData(flagData.tex2d, flagData.tooltipData, flagData.placeableSpriteType, flagData.placeableRemovesMines, flagData.consumableDefaultCount, flagData.showCount, flagData.allowedFloorTypes);
		}
	}
    public virtual void UpdateCount(int newCount) {
        HelperManager.s.InstantiateBubble(gameObject, (newCount - count >= 0 ? "+" : "-") + Mathf.Abs(newCount - count).ToString(), Color.white);
        count = newCount;
        tmpro.text = count.ToString();
    }
    protected virtual bool IsUsable() {
        return Player.s.alive && !Taken.takenFlags.Contains(gameObject) && allowedFloorTypes.Contains(Floor.s.floorType);
    }
    public virtual void UpdateUsable() {
        usable = IsUsable();
    } 
    protected override void OnDestroy() {
        base.OnDestroy();
        PlayerUIItemModule.s.ProcessRemovedFlag(this);
    }
}
