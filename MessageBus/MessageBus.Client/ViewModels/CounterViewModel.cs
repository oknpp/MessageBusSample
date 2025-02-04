using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace MessageBusSample.Client.ViewModels;

public class CounterViewModel : ReactiveObject
{
    [Reactive] public int Count { get; set; }
    
    public ReactiveCommand<Unit, Unit> IncrementCountCommand { get; set; }

    public CounterViewModel()
    {
        IncrementCountCommand = ReactiveCommand.Create(IncrementCount);
    }

    private void IncrementCount()
    {
        Count++;
    }
}
