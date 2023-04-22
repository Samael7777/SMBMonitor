using System.ComponentModel;
using SmbMonitor.Base;
using SmbMonitor.Base.DTO.Nodes;

namespace SmbMonitor.App.ViewModels;

public class NodeViewModel : BaseNotification
{
    public NodeViewModel(HostNode node)
    {
        LinkedNode = node;
    }

    public HostNode LinkedNode { get; }

    //todo
    public void NodeChangesHandler(object? sender, PropertyChangedEventArgs arg)
    {
        OnPropertyChanged(arg.PropertyName ?? "");
    }
}
