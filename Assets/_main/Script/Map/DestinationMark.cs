public class DestinationMark : MapNodeMark {
    public Hero Owner { get; }

    public DestinationMark(Hero owner) {
        Owner = owner;
    }
}