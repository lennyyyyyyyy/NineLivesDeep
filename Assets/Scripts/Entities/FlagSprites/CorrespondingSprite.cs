using UnityEngine;
using Random = UnityEngine.Random;
using System;

// abstract class implementing setting a sprite texture and tooltip based on a corresponding ui type
public class CorrespondingSprite : Entity {
	public Type correspondingUIType;
    public virtual void Init(Type correspondingUIType) {
        Debug.Log("Initializing with corresponding UI type: " + correspondingUIType);
		this.correspondingUIType = correspondingUIType;
		UIItemData uiItemData;
		// cataract curse
		if (typeof(Flag).IsAssignableFrom(correspondingUIType) && Random.value < Player.s.modifiers.cataractConfuseChance) {
			uiItemData = CatalogManager.s.typeToData[CatalogManager.s.allFlagTypes[Random.Range(0, CatalogManager.s.allFlagTypes.Count)]] as UIItemData;
		} else {
			uiItemData = CatalogManager.s.typeToData[this.correspondingUIType] as UIItemData;
		}
		Init(uiItemData.sprite, uiItemData.tooltipData);
	}
}

