using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Placeable : Flag {
    [System.NonSerialized]
    public GameObject sprite;
    protected override void Awake() {
		HelperManager.s.SetupUIEventTriggers(gameObject,
                                             new EventTriggerType[] {EventTriggerType.PointerDown},
                                             new Action<PointerEventData>[] {OnPointerDown});
        base.Awake();
    }
    protected virtual void OnPointerDown(PointerEventData data) {
        if (!usable) return;
        sprite = Instantiate(PrefabManager.s.flagSpritePrefab, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
        FlagSprite flagSprite = sprite.AddComponent(placeableSpriteType) as FlagSprite;
        FlagData flagData = CatalogManager.s.typeToData[GetType()] as FlagData;
        flagSprite.Init(this); 
        base.OnPointerExit(null);
    }
    protected override bool IsUsable() {
        return base.IsUsable() && count > 0 && (sprite == null || sprite.GetComponent<FlagSprite>().state != "held") 
            && GameManager.s.floorGameState == GameManager.GameState.FLOOR_STABLE;
    }
    public override void UpdateCount(int newCount) {
        base.UpdateCount(newCount);
        UpdateUsable();
    }
    private void Start() {
        if (PlayerUIItemModule.s.typeToInstances[GetType()].Count > 1 &&
            PlayerUIItemModule.s.typeToInstances[GetType()][0] != gameObject) {
            Placeable existingPlaceable = PlayerUIItemModule.s.typeToInstances[GetType()][0].GetComponent<Placeable>();
            existingPlaceable.UpdateCount(existingPlaceable.count + count);
            Destroy(gameObject);
        }
    }
}
