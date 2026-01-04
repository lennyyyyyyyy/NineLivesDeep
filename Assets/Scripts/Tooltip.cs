using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
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
	public bool setInitialData = false;
	public TooltipData tooltipData;

    public TMP_Text name, flavor, info, price;
    public GameObject mineIcon;
    private static float padding = 0.01f, power = 0.75f;
    private static Vector3 lastTooltipPos = Vector3.zero;
    private Vector3 targetPosition;
	
	private void Start() {
		if (setInitialData) {
			SetData(tooltipData);
		}
	}
    private void Update() {
        transform.position = Vector3.Lerp(transform.position, targetPosition, 1 - Mathf.Pow(1 - power, Time.deltaTime / .15f));
        lastTooltipPos = transform.position;
    }
	public void SetInitialData(TooltipData tooltipData) {
		setInitialData = true;
		this.tooltipData = tooltipData;
	}
    public void SetData(TooltipData tooltipData) {
		SetInitialData(tooltipData);
		
        name.text = tooltipData.name;
        flavor.text = tooltipData.flavor;
        info.text = tooltipData.info;
		price.text = tooltipData.showPrice ? (tooltipData.price == 0 ? "Free!" : "x" + tooltipData.price.ToString()) : "";
		mineIcon.GetComponent<Image>().enabled = tooltipData.showPrice;

		name.color = flavor.color = info.color = price.color = tooltipData.color;
        GetComponent<Outline>().effectColor = tooltipData.color;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
    public void Position(float x, float y, float width) {
        transform.position = lastTooltipPos;
        Vector2 size = HelperManager.s.WorldSizeFromRT(transform as RectTransform);
        Vector2 canvasSize = HelperManager.s.WorldSizeFromRT(UIManager.s.canvasRt);
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
}
