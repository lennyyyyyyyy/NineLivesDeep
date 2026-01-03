using UnityEngine;

class RaincloudSprite : FlagSprite {
    protected override void OnPlace() {
        base.OnPlace();
        Floor.s.rainCoords.Add(GetCoord());
    }
}
