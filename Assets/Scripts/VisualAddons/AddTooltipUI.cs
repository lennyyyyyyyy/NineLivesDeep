using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddTooltipUI : AddTooltip {
    protected RawImage image;
    protected override void Start() {
        image = GetComponent<RawImage>();

        // setup trigger pointer enter
		EventTrigger trigger;
		EventTrigger.Entry entry;

        trigger = GetComponent<EventTrigger>() == null ? gameObject.AddComponent<EventTrigger>() : GetComponent<EventTrigger>(); 
        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerEnter};
        entry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerExit};
        entry.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        trigger.triggers.Add(entry);

		base.Start();
    }
    protected override void Update() {
        if (tooltip != null && tooltip.active) {
            tooltip.GetComponent<Tooltip>().Position(transform.position.x, transform.position.y, UIManager.s.WorldSizeFromRT((transform as RectTransform)).x);
        }
    }
	protected override void SaveMaterial() {
		savedMaterial = image.material;
	}
	protected override void HoverEffectOn() {
		image.material = UIManager.s.alphaEdgeBlueMat;
	}
	protected override void HoverEffectOff() {
		image.material = savedMaterial;
	}
    protected void OnPointerEnter(PointerEventData data) {
		base.MouseEnter();	
    }
    protected void OnPointerExit(PointerEventData data) {
		base.MouseExit();
    }
}
