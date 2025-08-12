using System.Collections.Generic;

public abstract class DestinyProcessor {
    public string Description { get; protected set; }

    protected DestinyProcessor() { }
    public abstract void Activate(List<BattleHero> heroes, int stage);
}