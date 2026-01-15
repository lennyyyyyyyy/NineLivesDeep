using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Placeable : Flag {
    [System.NonSerialized]
    public GameObject sprite;
    protected override void Start() {
        base.Start();

		HelperManager.s.SetupUIEventTriggers(gameObject,
                                             new EventTriggerType[] {EventTriggerType.PointerDown},
                                             new Action<PointerEventData>[] {OnPointerDown});
    }
    protected virtual void OnPointerDown(PointerEventData data) {
        if (usable) {
            sprite = Instantiate(PrefabManager.s.flagSpritePrefab, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
            FlagSprite flagSprite = sprite.AddComponent(placeableSpriteType) as FlagSprite;
			FlagData flagData = CatalogManager.s.typeToData[GetType()] as FlagData;
			flagSprite.Init(this); 
            base.OnPointerExit(null);
        }
    }
    protected override bool IsUsable() {
        return base.IsUsable() && count > 0 && (sprite == null || sprite.GetComponent<FlagSprite>().state != "held");
    }
    public override void UpdateCount(int newCount) {
        base.UpdateCount(newCount);
        UpdateUsable();
    }
}
