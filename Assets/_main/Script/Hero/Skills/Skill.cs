using System;

public abstract class Skill {
    public Action[] Events => events;
    public bool Unstoppable => unstoppable;
    
    protected Action[] events;
    protected bool unstoppable;
}