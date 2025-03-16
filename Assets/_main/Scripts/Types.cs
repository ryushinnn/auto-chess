using System;

public enum Realm {
    Divine,
    Mortal,
    Nether,
    Chaos
}

[Flags]
public enum Role {
    Duelist = 0x01,
    Sorcerer = 0x02,
    Marksman = 0x04,
    Assassin = 0x08
}