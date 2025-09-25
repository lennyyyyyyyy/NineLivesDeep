using UnityEngine;
using Random = UnityEngine.Random;
using System;
using UnityEngine.Rendering.Universal;
public class PickupSprite : VerticalObject {
    protected int price;
    protected bool hovered = false;
    [System.NonSerialized]
    public Vector2Int coord;
    [System.NonSerialized]
    public Type parentType;
    protected Light2D light;
    protected SpriteRenderer sr;
    public static float droppedScale = 0.6f, hoveredScale = 0.8f, hoveredOffset;
    public virtual void Init(Type pType, int p, int x, int y) {
        parentType = pType;
        price = p;
        coord = new Vector2Int(x, y);
    }
    protected override void Start() {
        base.Start();
        marker = transform.Find("Marker").gameObject;
        sr = GetComponent<SpriteRenderer>();
        light = GetComponentInChildren<Light2D>();
        if (parentType != null && UIManager.s.flagUIVars.ContainsKey(parentType)) {
            sr.sprite = UIManager.s.flagUIVars[parentType].sprite;
            light.color = UIManager.s.flagUIVars[parentType].color;
            GetComponent<AddTooltipScene>().Init(UIManager.s.flagUIVars[parentType].name, 
                                                UIManager.s.flagUIVars[parentType].flavor, 
                                                UIManager.s.flagUIVars[parentType].info,
                                                UIManager.s.flagUIVars[parentType].color, true, price);
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
        if (Player.s.mines >= price) {
            Player.s.UpdateMineCount(Player.s.mines - price);
            GameObject g = Instantiate(GameManager.s.flag_p, transform.position, Quaternion.identity, UIManager.s.flagGroup.transform);
            g.AddComponent(parentType);
            Floor.s.flags[coord.x, coord.y] = null;
            Destroy(gameObject);
            Player.s.destroyPrints();
            Player.s.updatePrints();
        }
    }
}
