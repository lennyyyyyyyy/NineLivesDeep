using UnityEngine;
using Random = UnityEngine.Random;
using System;
using UnityEngine.Rendering.Universal;
using TMPro;
public class PickupSprite : VerticalObject {
	public static readonly int RANDOM = 0, TRIAL = 1, SHOP = 2;
    protected int price, spawnType;
	[System.NonSerialized]
	public int count = 0;
    protected bool hovered = false;
    [System.NonSerialized]
    public Vector2Int coord;
    [System.NonSerialized]
    public Type parentType;
    protected Light2D light;
    protected SpriteRenderer sr;
	protected TMP_Text tmpro;
    public static float droppedScale = 0.6f, hoveredScale = 0.8f, hoveredOffset;
    public virtual void Init(Type pType, int sType, int p, int x, int y) {
        parentType = pType;
		spawnType = sType;
        price = p;
        coord = new Vector2Int(x, y);
    }
    protected override void Start() {
        base.Start();
        marker = transform.Find("Marker").gameObject;
        sr = GetComponent<SpriteRenderer>();
        light = GetComponentInChildren<Light2D>();
		tmpro = GetComponentInChildren<TMP_Text>();
        if (parentType != null && UIManager.s.flagUIVars.ContainsKey(parentType)) {
            sr.sprite = UIManager.s.flagUIVars[parentType].sprite;
            light.color = UIManager.s.flagUIVars[parentType].color;
            GetComponent<AddTooltipScene>().Init(UIManager.s.flagUIVars[parentType].name, 
                                                UIManager.s.flagUIVars[parentType].flavor, 
                                                UIManager.s.flagUIVars[parentType].info,
                                                UIManager.s.flagUIVars[parentType].color, true, price);
			//consumable count numbers based on the way it spawns
			if (typeof(Consumable).IsAssignableFrom(parentType)) {
				tmpro.enabled = true;
				if (spawnType == RANDOM) {
					count = 1;
				} else if (spawnType == TRIAL) {
					count = Random.Range(Mathf.Max(1, UIManager.s.flagUIVars[parentType].consumableDefaultCount/3 - 1), UIManager.s.flagUIVars[parentType].consumableDefaultCount/3 + 2);
				} else if (spawnType == SHOP) {
					count = UIManager.s.flagUIVars[parentType].consumableDefaultCount;
				}
				tmpro.text = count.ToString();
			}
        }
        Floor.s.flags[coord.x, coord.y] = gameObject;
        transform.parent = Floor.s.tiles[coord.x, coord.y].transform;
        transform.localScale = droppedScale * Vector3.one;
        transform.localPosition = Vector3.zero;
        sr.sortingLayerName = "Player";
        Player.s.destroyPrints();
        Player.s.updatePrints();
    }
    protected override void Update() {
        base.Update();
        if (hovered) {
            UIManager.s.floatingHover(transform, hoveredScale, hoveredOffset, Vector3.zero);
        }
    }
    protected virtual void OnMouseEnter() {
        hovered = true;
        hoveredOffset = Random.Range(0f, 1f);
        LeanTween.cancel(gameObject);
        LeanTween.cancel(light.gameObject);
        LeanTween.value(light.gameObject, (float f) => { light.intensity = f; }, light.intensity, 5f, 0.25f).setEase(LeanTweenType.easeInOutCubic);
    }
    protected virtual void OnMouseExit() {
        hovered = false;
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, droppedScale * Vector3.one, 0.15f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.rotateLocal(gameObject, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.cancel(light.gameObject);
        LeanTween.value(light.gameObject, (float f) => { light.intensity = f; }, light.intensity, 3f, 0.25f).setEase(LeanTweenType.easeInOutCubic);
    }
    protected virtual void OnMouseDown() {
        if (Player.s.money >= price) {
            Player.s.UpdateMoney(Player.s.money - price);
            GameObject g = Instantiate(GameManager.s.flag_p, transform.position, Quaternion.identity, UIManager.s.flagGroup.transform);
            Flag f = g.AddComponent(parentType) as Flag;
			// it this has a count then give the item the same count
			if (tmpro.enabled) {
				f.count = count;
			}
            Floor.s.flags[coord.x, coord.y] = null;
            Destroy(gameObject);
            Player.s.destroyPrints();
            Player.s.updatePrints();
        }
    }
}
