public static class DestinyProcessorFactory {
    public static DestinyProcessor Create(Role role) {
        return role switch {
            Role.Marksman => new DestinyProcessor_Marksman(),
        };
    }
}