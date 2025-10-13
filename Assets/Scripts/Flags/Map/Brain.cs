using UnityEngine;

public class Brain : Map
{
    public override void OnDiscover(int x, int y) {
        bool hasAromatic = Player.s.hasFlag(typeof(Aromatic));
        int count = 0;
        for (int dx=-1; dx<=1; dx++) {
            for (int dy=-1; dy<=1; dy++) {
                if (Floor.s.within(x + dx, y + dy)) {
                    count += (Floor.s.GetUniqueMine(x+dx, y+dy)!=null && (Floor.s.GetUniqueMine(x+dx, y+dy).GetComponent<MineSprite>().detectable || hasAromatic))?1:0;
                }
            }
        }
        numbers[x, y].GetComponent<Number>().setNum(count);
    }
    protected override void Start() {
        base.Start();
    }
}
