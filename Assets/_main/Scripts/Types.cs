using System;

public enum Realm {
    None,
    Mortal,
    Divine,
    Nether,
    Mecha,
    Chaos
}

[Flags]
public enum Role {
    None = 0x00,
    Duelist = 0x01,
    Sorcerer = 0x02,
    Marksman = 0x04,
    Assassin = 0x08,
    Bruiser = 0x10,
    Cultist = 0x20,
}

public static partial class Translator {
    public static string ToName(this Realm realm) {
        return realm switch {
            Realm.Mortal => "Phàm nhân",
            Realm.Divine => "Thánh thần",
            Realm.Nether => "Âm ti",
            Realm.Mecha => "Cơ giới",
            Realm.Chaos => "Hỗn mang",
            _ => throw new ArgumentOutOfRangeException(nameof(realm), realm, null)
        };
    }

    public static string ToName(this Role role) {
        return role switch {
            Role.Duelist => "Kiếm sĩ",
            Role.Sorcerer => "Pháp sư",
            Role.Marksman => "Xạ thủ",
            Role.Assassin => "Thích khách",
            Role.Bruiser => "Quyền Vương",
            Role.Cultist => "Tín đồ",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
}