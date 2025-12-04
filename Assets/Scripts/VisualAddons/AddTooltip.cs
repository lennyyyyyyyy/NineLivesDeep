// This file is the base class for AddTooltipScene and AddTooltipUI. Change this class to modify both. 
using UnityEngine;

public class AddTooltip : MonoBehaviour {
	public bool setDefaults = false;
	public string name, flavor, info;
	public Color color;
	public bool showPrice;
	public int price;
	public bool addHoverEffect;
	[System.NonSerialized]
	public GameObject tooltip;
	protected bool hovered = false;
	protected Material savedMaterial;

	protected virtual void Start() {
		if (setDefaults) {
			Init(name, flavor, info, color, showPrice, price);
		}
		SaveMaterial();
	}
	protected virtual void Update() {}
	public virtual void Init(string n, string f, string i, Color c, bool sp=false, int p=0) {
        tooltip = Instantiate(GameManager.s.tooltip_p, UIManager.s.tooltipGroup.transform);
        tooltip.GetComponent<Tooltip>().Init(n, f, i, c, sp, p);
        tooltip.active = false;
	}
	protected virtual void SaveMaterial() {}
	protected virtual void MouseEnter() {
		hovered = true;
		tooltip.active = true;
		if (addHoverEffect) {
			HoverEffectOn();
		}
	}
	protected virtual void MouseExit() {
		hovered = false;
		tooltip.active = false;
		if (addHoverEffect) {
			HoverEffectOff();
		}
	}
	protected virtual void HoverEffectOn() {}
	protected virtual void HoverEffectOff() {}
    protected virtual void OnDestroy() {
        if (tooltip != null) {
            Destroy(tooltip);
        }
    }
}
