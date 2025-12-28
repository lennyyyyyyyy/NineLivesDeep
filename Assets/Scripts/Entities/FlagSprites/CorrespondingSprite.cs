using UnityEngine;
using Random = UnityEngine.Random;
using System;

// abstract class implementing setting a sprite texture and tooltip based on a corresponding ui type
public class CorrespondingSprite : Entity {
	public Type correspondingUIType;
	protected override void Start() {
		base.Start();
	}
	public virtual void SetInitialData(Type correspondingUIType) {
		setInitialData = true;
		this.correspondingUIType = correspondingUIType;
		UIItemData uiItemData;
		// cataract curse
		if (typeof(Flag).IsAssignableFrom(correspondingUIType) && Random.value < Player.s.modifiers.cataractConfuseChance) {
			uiItemData = CatalogManager.s.typeToData[CatalogManager.s.allFlagTypes[Random.Range(0, CatalogManager.s.allFlagTypes.Count)]] as UIItemData;
		} else {
			uiItemData = CatalogManager.s.typeToData[this.correspondingUIType] as UIItemData;
		}
		base.SetInitialData(uiItemData.sprite, uiItemData.tooltipData);
	}
	protected override void ApplyInitialData() {
		base.ApplyInitialData();
	}
	public virtual void SetData(Type correspondingUIType) {
		SetInitialData(correspondingUIType);
		ApplyInitialData();
	}
	protected override void SetDefaultData() {
		base.SetDefaultData();
	}
}

