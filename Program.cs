using Microsoft.EntityFrameworkCore;
using TestAsyncEnumerable;

const string DbName = "MyItem_DB";
var options = new DbContextOptionsBuilder<MyDbContext>()
    .UseInMemoryDatabase(DbName)
    .Options;

await using (var context = new MyDbContext(options))
{
    await InitAsync(context);

    Console.WriteLine("Display enumerable");
    var elements = FilterEvenItems(await FetchIEnumberableItems(context));
    DisplayIEnumerable(elements);


    var iasyncenumerable = FilterOdd(FetchAsyncEnumerable(context));

    Console.WriteLine("Display iAsyncEnumerable");
    await DisplayIAsyncEnumerable(iasyncenumerable);
}



async Task InitAsync(MyDbContext context)
{
    for(long i = 1; i <= 100; i ++)
    {
        var myItem = new MyItem(i, $"MyItem_{i}");
        Console.WriteLine($"Storing item {myItem}");
        context.Add(myItem);
    }

    await context.SaveChangesAsync();
}

async Task<IEnumerable<MyItem>> FetchIEnumberableItems(MyDbContext context)
{
    await Task.Delay(1000);
    var enumerable = context.Items.AsEnumerable<MyItem>();
    return enumerable;
}

IEnumerable<MyItem> FilterEvenItems(IEnumerable<MyItem> elements)
{
    bool gotError = false;
    IEnumerator<MyItem>? iter = null;
    try
    {
        iter = elements.GetEnumerator();
    }
    catch (Exception)
    {
        gotError = true;
    }

    if (iter == null)
    {
        yield break;
    }

    while (true)
    {
        try
        {
            if (!iter.MoveNext())
            {
                break;
            }
        }
        catch (Exception)
        {
            gotError = true;
            break;
        }

        var element = iter.Current;

        if (element.Id % 2 == 0)
        {
            yield return iter.Current;
        }
    }

    try
    {
        iter.Dispose();
    }
    catch (Exception)
    {
        gotError = true;
    }

    if (gotError)
    {
        yield break;
    }
}

void DisplayIEnumerable(IEnumerable<MyItem> ienumerableItems)
{
    foreach (var item in ienumerableItems)
    {
        Console.WriteLine(item);
    }
}

IAsyncEnumerable<MyItem> FetchAsyncEnumerable(MyDbContext context)
{
    return context.Items.AsAsyncEnumerable();
}

async IAsyncEnumerable<MyItem> FilterOdd(IAsyncEnumerable<MyItem> elements)
{
    bool gotError = false;

    IAsyncEnumerator<MyItem>? iter = null;
    try
    {
        iter = elements.GetAsyncEnumerator();
    }
    catch (Exception)
    {
        gotError = true;
    }

    if (iter == null)
    {
        yield break;
    }

    while (true)
    {
        await Task.Delay(100);
        if (!await iter!.MoveNextAsync())
        {
            break;
        }

        var element = iter.Current;
        if (element.Id % 2 != 0)
        {
            yield return iter.Current;
        }
    }

    try
    {
        await iter!.DisposeAsync().ConfigureAwait(false);
    }
    catch (Exception)
    {
        gotError = true;
    }

    if (gotError)
    {
        yield break;
    }
}

async Task DisplayIAsyncEnumerable(IAsyncEnumerable<MyItem> iasyncenumerable)
{
    await foreach (var item in iasyncenumerable)
    {
        Console.WriteLine(item);
    }
}
