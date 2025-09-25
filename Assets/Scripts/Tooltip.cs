using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public TMP_Text name, flavor, info, price;
    public GameObject mineIcon;
    private static float padding = 0.01f, power = 0.75f;
    private static Vector3 lastTooltipPos = Vector3.zero;
    private Vector3 targetPosition;
    public void Init(string n, string f, string i, Color c, bool showPrice=false, int p=0) {
        name.text = n;
        flavor.text = f;
        info.text = i;
        if (showPrice) {
			if (p == 0) {
				price.text = "Free!";
			} else {
				price.text = "x" + p.ToString();
			}
            mineIcon.GetComponent<Image>().enabled = true;
        } else {
            price.text = "";
            mineIcon.GetComponent<Image>().enabled = false;
        }
        name.color = c;
        flavor.color = c;
        info.color = c;
        price.color = c;
        GetComponent<Outline>().effectColor = c;
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
