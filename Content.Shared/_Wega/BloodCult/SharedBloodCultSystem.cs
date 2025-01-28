namespace Content.Shared.Blood.Cult;

public abstract class SharedBloodCultSystem : EntitySystem
{
    private static int _offerings = 3;

    public static void IncrementOfferingsCount()
    {
        _offerings++;
    }

    public static void SubtractOfferingsCount()
    {
        _offerings -= 3;
    }

    public static int GetOfferingsCount()
    {
        return _offerings;
    }
}
