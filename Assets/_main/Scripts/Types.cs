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

public static partial class Translator {
    public static string ToString(this Realm realm) {
        return realm switch {
            Realm.Divine => "Thánh thần",
            Realm.Mortal => "Phàm nhân",
            Realm.Nether => "Âm ti",
            Realm.Chaos => "Hỗn mang",
            _ => throw new ArgumentOutOfRangeException(nameof(realm), realm, null)
        };
    }

    public static string ToString(this Role role) {
        return role switch {
            Role.Duelist => "Kiếm sĩ",
            Role.Sorcerer => "Pháp sư",
            Role.Marksman => "Xạ thủ",
            Role.Assassin => "Thích khách",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
}