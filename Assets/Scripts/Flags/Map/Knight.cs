using UnityEngine;

class Knight : Map {
    public override void OnDiscover(int x, int y) {
        int count = 0;
        int[] dxs = new int[]{-2, -2, -1, -1, 1, 1, 2, 2};
        int[] dys = new int[]{-1, 1, -2, 2, -2, 2, -1, 1};
        for (int k=0; k<8; k++) {
            int dx = dxs[k];
            int dy = dys[k];
            if (Floor.s.within(x + dx, y + dy)) {
                count += (Floor.s.GetUniqueMine(x+dx, y+dy)!=null)?1:0;
            }
        }
		TrySetNumber(x, y, count);
    }
    protected override void Start() {
        base.Start();
    }
}
