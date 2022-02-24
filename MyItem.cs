namespace TestAsyncEnumerable;

public class MyItem
{
    public MyItem(long id, string description)
        => (Id, Description) = (id, description);

    public long Id { get; }

    public string Description { get; }

    public override string ToString()
    {
        return $"{Id} - {Description}";
    }
}
