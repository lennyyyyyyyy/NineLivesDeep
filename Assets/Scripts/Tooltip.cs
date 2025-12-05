using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipData {
	public string name, 
		  		  flavor = "",
				  info = "";
	public Color color = Color.white;
	public bool showPrice = false;
	public int price = 0;

	public TooltipData(string name,
					   string flavor = null,
					   string info = null,
					   Color? color = null,
					   bool? showPrice = null,
					   int? price = null) {
		this.name = name;
		this.flavor = flavor ?? this.flavor;
		this.info = info ?? this.info;
		this.color = color ?? this.color;
		this.showPrice = showPrice ?? this.showPrice;
		this.price = price ?? this.price;
	}
}

public class Tooltip : MonoBehaviour {
    public TMP_Text name, flavor, info, price;
    public GameObject mineIcon;
	[System.NonSerialized]
	public TooltipData data;
    private static float padding = 0.01f, power = 0.75f;
    private static Vector3 lastTooltipPos = Vector3.zero;
    private Vector3 targetPosition;
    public void SetData(TooltipData data) {
		this.data = data;
        name.text = data.name;
        flavor.text = data.flavor;
        info.text = data.info;
		price.text = showPrice ? (data.price == 0 ? "Free!" : "x" + data.price.ToString()) : "";
		mineIcon.GetComponent<Image>().enabled = data.showPrice;

		name.color = flavor.color = info.color = price.color = data.color;
        GetComponent<Outline>().effectColor = data.color;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
    public void Position(float x, float y, float width) {
        transform.position = lastTooltipPos;
        Vector2 size = UIManager.s.WorldSizeFromRT(transform as RectTransform);
        Vector2 canvasSize = UIManager.s.WorldSizeFromRT(UIManager.s.canvasRt);
        float targetY = Mathf.Clamp(y, MainCamera.s.transform.position.y - canvasSize.y/2 + size.y/2, MainCamera.s.transform.position.y + canvasSize.y/2 - size.y/2);
        
        if (x - width/2 - size.x - 2 * padding * canvasSize.x > MainCamera.s.transform.position.x-canvasSize.x/2 && 
            x + width/2 + size.x + 2 * padding * canvasSize.x > MainCamera.s.transform.position.x+canvasSize.x/2) {
            targetPosition = new Vector3(x - width/2 - size.x/2 - padding * canvasSize.x, targetY, 0);
            name.alignment = TMPro.TextAlignmentOptions.Right;
            flavor.alignment = TMPro.TextAlignmentOptions.Right;
            info.alignment = TMPro.TextAlignmentOptions.Right;
            name.gameObject.transform.SetAsLastSibling();
        } else {
            targetPosition = new Vector3(x + width/2 + size.x/2 + padding * canvasSize.x, targetY, 0);
            name.alignment = TMPro.TextAlignmentOptions.Left;
            flavor.alignment = TMPro.TextAlignmentOptions.Left;
            info.alignment = TMPro.TextAlignmentOptions.Left;
            name.gameObject.transform.SetAsFirstSibling();
        }
    }
    public void Update() {
        transform.position = Vector3.Lerp(transform.position, targetPosition, 1 - Mathf.Pow(1 - power, Time.deltaTime / .15f));
        lastTooltipPos = transform.position;
    }
}
