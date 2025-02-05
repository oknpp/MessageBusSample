using Bogus;
using MessageBusSample.Client.Components.Actions;
using MessageBusSample.Client.Components.Actions.Rename;
using MudBlazor;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;
using System.Reflection;
using System.Security.AccessControl;

namespace MessageBusSample.Client.Services;

public class EntityService : IDisposable
{
    private static readonly List<string> AllIcons = GetAllIcons();

    public List<Entity> DummyItems { get; }

    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public EntityService()
    {
        DummyItems = GenerateRandomEntities(30); // Erzeugt 10 zufällige Einträge

        var deletedSubscription = MessageBus.Current.Listen<InitDeleteActionMessage>().Subscribe(action =>
        {
            DeleteEntity(action.EntityId);
        });


        var renamedSubscription = MessageBus.Current.Listen<InitRenameActionMessage>().Subscribe(action =>
        {
            RenameEntity(action.EntityId, action.NewName);
        });

        _disposables.Add(deletedSubscription);
        _disposables.Add(renamedSubscription);
    }

    private void RenameEntity(int entityId, string newName)
    {
        var toRename = DummyItems.FirstOrDefault(e => e.Id == entityId);
        Console.WriteLine($"Received DeleteMessage for {toRename?.Name}");
        if (toRename != null)
        {
            toRename.Name = newName;
            MessageBus.Current.SendMessage(new ResolveRenameActionMessage(toRename.Id));
        }
    }

    public void DeleteEntity(int entityId)
    {
        var toDelete = DummyItems.FirstOrDefault(e => e.Id == entityId);
        Console.WriteLine($"Received DeleteMessage for {toDelete?.Name}");
        if (toDelete != null)
        {
            toDelete.IsDeleted = true;
            MessageBus.Current.SendMessage(new ResolvedDeleteActionMessage(toDelete.Id));
        }
    }

    private List<Entity> GenerateRandomEntities(int count)
    {
        var icons = new List<string>
        {
            Icons.Material.Filled.Home,
            Icons.Material.Filled.Star,
            Icons.Material.Filled.Favorite,
            Icons.Material.Filled.Settings,
            Icons.Material.Filled.Face,
            Icons.Material.Filled.ThumbUp,
            Icons.Material.Filled.Gamepad,
            Icons.Material.Filled.Flight,
            Icons.Material.Filled.Folder,
            Icons.Material.Filled.Girl,
            Icons.Material.Filled.Group,
            Icons.Material.Filled.House,
            Icons.Material.Filled.Info,
            Icons.Material.Filled.Keyboard,
            Icons.Material.Outlined.GetApp,
            Icons.Material.Outlined.Gif,
            Icons.Material.Outlined.Grade,
            Icons.Material.Outlined.Gite,
        };

        var faker = new Faker<Entity>()
            .RuleFor(e => e.Id, f => f.IndexFaker) // Zähler von 1 bis count
            .RuleFor(e => e.Name, f => f.Commerce.ProductName())  // Zufälliger Produktname
            .RuleFor(e => e.Description, f => f.Lorem.Sentence()) // Zufälliger Satz
            .RuleFor(e => e.IsCheckedOut, f => f.Random.Bool())  // Zufälliges True/False
            .RuleFor(e => e.IsCurrent, f => f.Random.Bool())
            .RuleFor(e => e.IsDeleted, f => f.Random.Bool(0.2f)) // 20% Wahrscheinlichkeit für True
            .RuleFor(e => e.WasViewed, f => f.Random.Bool())
            .RuleFor(e => e.ReadOnly, f => f.Random.Bool(0.3f)) // 30% Wahrscheinlichkeit für True
            .RuleFor(e => e.Icon, f => f.PickRandom(icons)) // Zufälliges Icon
            .RuleFor(e => e.CreatedDate, f => f.Date.Past(5)); // Ein Datum aus den letzten 5 Jahren

        return faker.Generate(count);
    }


    public async Task<List<Entity>> GetCheckedOut()
    {
        var entities = await GetEntitiesAsync();
        return entities.Where(e => e.IsCheckedOut && !e.IsDeleted).ToList();
    }

    public async Task<List<Entity>> GetCurrent() => (await GetEntitiesAsync()).Where(e => e.IsCurrent && !e.IsDeleted).ToList();
    public async Task<List<Entity>> GetDeleted() => (await GetEntitiesAsync()).Where(e => e.IsDeleted).ToList();
    public async Task<List<Entity>> GetViewed() => (await GetEntitiesAsync()).Where(e => e.WasViewed && !e.IsDeleted).ToList();
    public async Task<List<Entity>> GetReadOnly() => (await GetEntitiesAsync()).Where(e => e.ReadOnly && !e.IsDeleted).ToList();

    public async Task<List<Entity>> GetEntitiesAsync()
    {
        var randomDelay = Random.Shared.Next(1000, 3000);
        await Task.Delay(randomDelay);
        return DummyItems.Select(x => x with { }).ToList();
    }

    private static List<string> GetAllIcons()
    {
        return typeof(Icons.Material.Filled)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(string))
            .Select(p => (string)p.GetValue(null)!)
            .ToList();
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}

public record Entity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsCheckedOut { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsDeleted { get; set; }
    public bool WasViewed { get; set; }
    public bool ReadOnly { get; set; }
    public string Icon { get; set; }
    public DateTime CreatedDate { get; set; }
}