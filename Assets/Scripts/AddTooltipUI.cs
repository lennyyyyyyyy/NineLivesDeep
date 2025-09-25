using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class AddTooltipUI : MonoBehaviour
{
    public bool setDefaults = false;
    public string name, flavor, info;
    public Color color;
    public bool showPrice;
    public int price;
	[System.NonSerialized]
    public GameObject tooltip;
    private EventTrigger trigger;
    private EventTrigger.Entry entry;
    private RawImage image;
    private static MaterialPropertyBlock hoveredMpb, defaultMpb;
    private void Start() {
        image = GetComponent<RawImage>();
        // setup trigger pointer enter
        trigger = GetComponent<EventTrigger>() == null ? gameObject.AddComponent<EventTrigger>() : GetComponent<EventTrigger>(); 
        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerEnter};
        entry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerExit};
        entry.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        trigger.triggers.Add(entry);

        if (setDefaults) {
            Init(name, flavor, info, color, showPrice, price);
        }
    }
    private void Update() {
        if (tooltip != null && tooltip.active) {
            tooltip.GetComponent<Tooltip>().Position(transform.position.x, transform.position.y, UIManager.s.WorldSizeFromRT((transform as RectTransform)).x);
        }
    }
    public void Init(string n, string f, string i, Color c, bool sp=false, int p=0) {
        tooltip = Instantiate(GameManager.s.tooltip_p, UIManager.s.tooltipGroup.transform);
        tooltip.GetComponent<Tooltip>().Init(n, f, i, c, sp, p);
        if (n == "") {
            Debug.LogError("error");
        }
        tooltip.active = false;
    }
    public void OnPointerEnter(PointerEventData data) {
        tooltip.active = true;
        image.material = UIManager.s.alphaEdgeBlueMat;
    }
    public void OnPointerExit(PointerEventData data) {
        tooltip.active = false;
        image.material = null;
    }
    private void OnDestroy() {
        if (tooltip != null) {
            Destroy(tooltip);
        }
    }
}
