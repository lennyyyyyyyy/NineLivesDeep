using UnityEngine;

public class AddTooltipScene : AddTooltip {
    protected SpriteRenderer sr;
    protected override void Start() {
        sr = GetComponent<SpriteRenderer>();
		base.Start();
    }
    protected override void Update() {
        if (tooltip != null && tooltip.activeSelf) {
            tooltip.GetComponent<Tooltip>().Position(transform.position.x, transform.position.y, sr.bounds.size.x);
        }
    }
	protected override void SaveMaterial() {
		savedMaterial = sr.material;
	}
	protected override void HoverEffectOn() {
		sr.material = UIManager.s.alphaEdgeBlueMat;
	}
	protected override void HoverEffectOff() {
		sr.material = savedMaterial;
	}
    protected void OnMouseEnterCustom() {
		base.MouseEnter();
    }
    protected void OnMouseExitCustom() {
		base.MouseExit();
    }
}
