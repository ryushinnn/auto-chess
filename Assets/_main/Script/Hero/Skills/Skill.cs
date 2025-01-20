using System;

public abstract class Skill {
    public Action[] Events => events;
    
    protected Action[] events;
}