using UnityEngine;
using System.Linq;
public class Print : MonoBehaviour
{
    public Vector2Int d;
    public static Color defaultColor = new Color(.05f, .05f, .05f, 1), hoveredColor = new Color(.6f, 1, 1, 1);
    public static float defaultScale = 0.9f, hoverOffset, hoverPeriod;
    private bool hovered = false;
    private Vector3 defaultRotation;
    private Collider2D c;
    public void init(int dx, int dy) {
        transform.parent = Floor.s.tiles[Player.s.coord.x + dx, Player.s.coord.y + dy].transform;
        transform.localScale = Vector3.one * defaultScale;
        transform.localPosition = Vector3.zero;
        d = new Vector2Int(dx, dy);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(dx, dy, 0));
        defaultRotation = transform.localEulerAngles;
        c = GetComponent<Collider2D>();
    }
    private void OnMouseEnterCustom() {
        hoverOffset = Random.Range(0f, 1f);
        hoverPeriod = 0.65f + 0.15f * Random.Range(-1f, 1f);
        GetComponent<SpriteRenderer>().color = hoveredColor;
    }
    private void OnMouseExitCustom() {
        LeanTween.scale(gameObject, Vector3.one * defaultScale, 0.15f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.rotateLocal(gameObject, defaultRotation, 0.15f).setEase(LeanTweenType.easeInOutCubic);
        GetComponent<SpriteRenderer>().color = defaultColor;
    }
    private void OnMouseDownCustom() {
        Player.s.setCoord(Player.s.coord.x + d.x, Player.s.coord.y + d.y);
		Player.s.lastMovement = d;
    }
    void Update()
    {
        bool newHovered = GameManager.s.mouseColliders.Contains(c);
        if (hovered) {
            UIManager.s.floatingHover(transform, 1f, hoverOffset, defaultRotation, 0.1f, 10f, hoverPeriod);
            if (Input.GetMouseButtonDown(0)) {
                OnMouseDownCustom();
            }
            if (!newHovered) {
                OnMouseExitCustom();
            }
        } else if (newHovered) {
            OnMouseEnterCustom();
        }
        hovered = newHovered;
    }
}
