﻿public static class DestinyProcessorFactory {
    public static DestinyProcessor Create(Role role) {
        var destiny = DestinyDB.Instance.Find(role);
        
        return role switch {
            Role.Marksman => new DestinyProcessor_Marksman(destiny),
        };
    }

    public static DestinyProcessor Create(Realm realm) {
        var destiny = DestinyDB.Instance.Find(realm);
        
        return realm switch {
            _ => null,
        };
    }
}