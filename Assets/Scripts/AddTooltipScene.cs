using UnityEngine;

public class AddTooltipScene : MonoBehaviour {
    public bool setDefaults = false;
    public string name, flavor, info;
    public Color color;
    public bool showPrice;
    public int price;
    private SpriteRenderer sr;
    private GameObject tooltip;
    private void Start() {
        sr = GetComponent<SpriteRenderer>();
        if (setDefaults) {
            Init(name, flavor, info, color, showPrice, price);
        }
    }
    private void Update() {
        if (tooltip != null && tooltip.active) {
            tooltip.GetComponent<Tooltip>().Position(transform.position.x, transform.position.y, sr.bounds.size.x);
        }
    }
    public void Init(string n, string f, string i, Color c, bool sp=false, int p=0) {
        tooltip = Instantiate(GameManager.s.tooltip_p, UIManager.s.tooltipGroup.transform);
        tooltip.GetComponent<Tooltip>().Init(n, f, i, c, sp, p);
        tooltip.active = false;
    }
    private void OnMouseEnter() {
        tooltip.active = true;
    }
    private void OnMouseExit() {
        tooltip.active = false;
    }
    private void OnDestroy() {
        if (tooltip != null) {
            Destroy(tooltip);
        }
    }
}