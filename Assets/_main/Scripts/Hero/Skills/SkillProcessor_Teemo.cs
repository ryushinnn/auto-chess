using System;

public class SkillProcessor_Teemo : SkillProcessor {
    public SkillProcessor_Teemo(Hero hero) : base(hero) {
        events = new Action[]{ThrowBomb, ThrowBigBomb};
    }
    
    void ThrowBomb() {
        
    }

    void ThrowBigBomb() {
        
    }
}