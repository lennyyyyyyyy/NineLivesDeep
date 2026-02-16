using UnityEngine;
using Random = UnityEngine.Random;
using System;
using UnityEngine.Rendering.Universal;
using TMPro;
public class PickupSprite : CorrespondingSprite {
    public int price = 0;
	public SpawnType spawnType = SpawnType.RANDOM;
	public Vector2Int spawnCoord = Floor.INVALID_COORD;

	[System.NonSerialized]
	public int count = 0;

	public enum SpawnType { RANDOM, TRIAL, SHOP };

    protected Light2D light;
	protected TMP_Text tmpro;
    public static float hoveredScale = 0.8f, hoveredOffset;

    protected override void BeforeInit() {
        base.BeforeInit();
        marker = transform.Find("Marker").gameObject;
        light = GetComponentInChildren<Light2D>();
		tmpro = GetComponentInChildren<TMP_Text>();
    }
    protected override void Update() {
        base.Update();
        if (hovered) {
            HelperManager.s.FloatingHover(transform, hoveredScale, hoveredOffset, Vector3.zero);
        }
    }
    public override void Init() {
        base.Init();
        defaultScale = ConstantsManager.s.flagSpriteDroppedScale;
    }
    public virtual void Init(Type correspondingUIType, int? price = null, SpawnType? spawnType = null, Vector2Int? spawnCoord = null, int? count = null) {
		this.price = price ?? this.price;
		this.spawnType = spawnType ?? this.spawnType;
		this.spawnCoord = spawnCoord ?? this.spawnCoord;
		base.Init(correspondingUIType);
		this.tooltipData.showPrice = true;
		this.tooltipData.price = this.price;
        base.Init(tooltipData: this.tooltipData);
        this.count = count ?? this.count;
		//consumable count numbers based on the way it spawns
        if (count == null && typeof(Consumable).IsAssignableFrom(this.correspondingUIType)) {
			FlagData parentFlagData = CatalogManager.s.typeToData[this.correspondingUIType] as FlagData;
			if (this.spawnType == SpawnType.RANDOM) {
				this.count = 1;
			} else if (this.spawnType == SpawnType.TRIAL) {
				this.count = Random.Range(Mathf.Max(1, parentFlagData.defaultCount/3 - 1), parentFlagData.defaultCount/3 + 2);
			} else if (spawnType == SpawnType.SHOP) {
				this.count = parentFlagData.defaultCount;
			}
		}
		light.color = this.tooltipData.color;
		Move(this.spawnCoord.x, this.spawnCoord.y);
		if (this.count > 0) {
			tmpro.enabled = true;
			tmpro.text = this.count.ToString();
		}
        
    }
    protected override void OnMouseEnterCustom() {
		base.OnMouseEnterCustom();
        hoveredOffset = Random.Range(0f, 1f);
        LeanTween.cancel(gameObject);
        LeanTween.cancel(light.gameObject);
        LeanTween.value(light.gameObject, (float f) => { light.intensity = f; }, light.intensity, 5f, 0.25f).setEase(LeanTweenType.easeInOutCubic);
    }
    protected override void OnMouseExitCustom() {
		base.OnMouseExitCustom();
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, ConstantsManager.s.flagSpriteDroppedScale * Vector3.one, 0.15f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.rotateLocal(gameObject, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.cancel(light.gameObject);
        LeanTween.value(light.gameObject, (float f) => { light.intensity = f; }, light.intensity, 3f, 0.25f).setEase(LeanTweenType.easeInOutCubic);
    }
	public override bool IsInteractable() {	
		return base.IsInteractable() && Player.s.money >= price;
	}
	public override void Interact() {
		Player.s.UpdateMoney(Player.s.money - price);
		GameObject g = Instantiate(PrefabManager.s.flagPrefab, transform.position, Quaternion.identity);
		Flag f = g.AddComponent(correspondingUIType) as Flag;
		// it this has a count then give the item the same count
		if (tmpro.enabled) {
			f.count = count;
		}
		Remove();
		Player.s.destroyPrints();
		Player.s.updatePrints();
	}
}
