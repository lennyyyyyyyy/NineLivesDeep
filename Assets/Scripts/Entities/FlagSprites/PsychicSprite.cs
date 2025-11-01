using UnityEngine;
using System.Collections.Generic;
public class PsychicSprite : FlagSprite
{
    private PsychicEye eye;
    private float timer = 4;

    protected virtual void Start(){
        base.Start();
        eye = Instantiate(GameManager.s.psychicEye_p, transform).GetComponent<PsychicEye>();
        sr.sprite = Resources.Load<Sprite>("Textures/flag_psychic_sprite");
    }
    protected virtual void Update() {
        base.Update();
        if (state == "dropped") {
            timer += Time.deltaTime;
            if (timer > 4) {
                timer = 0;
                // find nearest mines
                List<Vector2Int> nearestMines = new List<Vector2Int>();
                for (int x = 0; x < Floor.s.width; x++) {
                    for (int y = 0; y < Floor.s.height; y++) {
                        if (Floor.s.GetUniqueMine(x, y) != null) {
                            Vector2Int difference = new Vector2Int(x - GetCoord().x, y - GetCoord().y);
                            if (nearestMines.Count == 0 || difference.magnitude == nearestMines[0].magnitude) {
                                nearestMines.Add(difference);
                            } else if (difference.magnitude < nearestMines[0].magnitude) {
                                nearestMines.Clear();
                                nearestMines.Add(difference);
                            }
                        }
                    }
                }
                eye.mineDirection = nearestMines[Random.Range(0, nearestMines.Count)];
                eye.UpdatePosition();
            }
        }
    }
    protected override void OnPlace() {
        base.OnPlace();
        eye.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        eye.placed = true;
    }
}
