using UnityEngine;

public class AddTooltipScene : AddTooltip {
    protected SpriteRenderer sr;
    protected override void Awake() {
        sr = GetComponent<SpriteRenderer>();
		base.Awake();
    }
    protected override void Update() {
        if (tooltip != null && tooltip.activeSelf) {
            tooltip.GetComponent<Tooltip>().Position(transform.position.x, transform.position.y, sr.bounds.size.x);
        }
    }
	protected override void HoverEffectOn() {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        sr.GetPropertyBlock(mpb);
        mpb.SetColor(ShaderManager.s.EdgeColorID, ConstantsManager.s.cyanTransparent);
        sr.SetPropertyBlock(mpb);
	}
	protected override void HoverEffectOff() {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        sr.GetPropertyBlock(mpb);
        mpb.SetColor(ShaderManager.s.EdgeColorID, new Color(0, 0, 0, 0));
        sr.SetPropertyBlock(mpb);
	}
    protected void OnMouseEnterCustom() {
		base.MouseEnter();
    }
    protected void OnMouseExitCustom() {
		base.MouseExit();
    }
}
