using UnityEngine;
using UnityEngine.UI;
using TMPro;
class Bubble : MonoBehaviour { 
    private Color color;
    private string text;
    private TMP_Text tmp;
    private float time;
    public void Init(Color c, string t, float ti, float scale) {
        color = c;
        text = t;
        time = ti;
        transform.localScale = scale * Vector3.one;
    }
    private void TweenFontSize(float f) {
        tmp.fontSize = f;
    }
    private void Start() {
        tmp = GetComponentInChildren<TMP_Text>();
        tmp.text = text;
        tmp.color = color;
        GetComponent<Outline>().effectColor = color;
        Vector2 size = UIManager.s.WorldSizeFromRT(transform as RectTransform);
        Vector2 canvasSize = UIManager.s.WorldSizeFromRT(UIManager.s.canvasRt);
        float targetX = Mathf.Clamp(transform.position.x, MainCamera.s.transform.position.x - canvasSize.x * 0.51f + size.x/2, MainCamera.s.transform.position.x + canvasSize.x*0.51f - size.x/2);
        float targetY = Mathf.Clamp(transform.position.y, MainCamera.s.transform.position.y - canvasSize.y * 0.51f + size.y/2, MainCamera.s.transform.position.y + canvasSize.y*0.51f - size.y/2);
        transform.position = new Vector3(targetX, targetY, 0);
        LeanTween.value(gameObject, TweenFontSize, 0, 28.6f, 0.5f).setEase(LeanTweenType.easeOutElastic);
        LeanTween.value(gameObject, TweenFontSize, 28.6f, 0, 0.5f).setDelay(time-0.5f).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
            Destroy(gameObject);
        });
    }
}