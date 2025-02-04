using ReactiveUI;
using System.Reactive.Disposables;

namespace MessageBusSample.Client.Components.Actions;

public class DeleteActionViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator => new ViewModelActivator();

    public DeleteActionViewModel()
    {
        this.WhenActivated(disposables =>
        {
            ReactiveUI.MessageBus.Current.Listen<InitDeleteActionMessage>().Subscribe(action =>
            {
                // Do something with the action
            }).DisposeWith(disposables);
        });

    }
}
