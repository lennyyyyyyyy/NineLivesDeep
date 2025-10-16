using UnityEngine;

public class Vertical : Map
{
    public override void OnDiscover(int x, int y) {
        int count = 0;
        for (int j = 0; j < Floor.s.height; j++) {
            count += (Floor.s.GetUniqueMine(x, j)!=null)?1:0;
        }
		TrySetNumber(x, y, count);
    }
    protected override void Start() {
        base.Start();
    }
}
