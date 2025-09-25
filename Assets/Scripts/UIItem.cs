using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour {
	[System.NonSerialized]
	public RectTransform rt;
	protected Texture2D tex2d;
	protected string name, flavor, info;
	protected Color color;
	protected AddTooltipUI addTooltip;
    protected virtual void init(Texture2D t, string n, string f, string i, Color c) {
		tex2d = t;
		name = n;
		flavor = f;
		info = i;
		color = c;
    }
	protected virtual void Start() {
		rt = (transform as RectTransform);
		addTooltip = (GetComponent<AddTooltipUI>() == null ? gameObject.AddComponent(typeof(AddTooltipUI)) as AddTooltipUI : GetComponent<AddTooltipUI>());
		if (tex2d != null) {
			GetComponent<RawImage>().texture = tex2d;
			addTooltip.Init(name, flavor, info, color);
		}
	}		
	public virtual void Modify(ref Modifiers modifiers) {
	}
}
