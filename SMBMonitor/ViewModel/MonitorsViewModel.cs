using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using SMBMonitor.Exceptions;
using SMBMonitor.Model;
using SMBMonitor.View;
using SmbMonitorLib;
using SmbMonitorLib.Wifi;

namespace SMBMonitor.ViewModel;

public class MonitorsViewModel : BaseViewModel
{
    private readonly MonitorsModel _mainModel;
    
    public MonitorsViewModel()
    {
        ItemsList = new ObservableCollection<MonitorListItem>();

        _mainModel = ModelsContainer.Instance.MonitorsModel;
        _mainModel.MonitorsList.CollectionChanged += OnModelItemsChanged;

        InitializeCommands();
        AddCurrentItemsFromModel();
    }

    [MemberNotNull(nameof(AddCommand), nameof(RemoveCommand), nameof(StartCommand), 
        nameof(StopCommand), nameof(InfoCommand), nameof(ExitCommand))]
    private void InitializeCommands()
    {
        AddCommand = new RelayCommand(_ => AddMonitor());
        RemoveCommand = new RelayCommand(_ => RemoveSelectedItemsFromModel());
        StartCommand = new RelayCommand(_ => StartSelected());
        StopCommand = new RelayCommand(_ => StopSelected());
        InfoCommand = new RelayCommand(_ => throw new NotImplementedException());
        ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());
    }
   
    #region Model Interface

    public ObservableCollection<MonitorListItem> ItemsList { get; set; }

    public IList? SelectedItems { get; set; }

    //TODO ADD СДЕЛАНО ДЛЯ ТЕСТИРОВАНИЯ, ПЕРЕДЕЛАТЬ!!!!
    public RelayCommand AddCommand { get; private set; }

    public RelayCommand RemoveCommand { get; private set; }

    public RelayCommand StartCommand { get; private set; }

    public RelayCommand StopCommand { get; private set; }

    public RelayCommand ExitCommand { get; private set; }


    //TODO сделать информацию о мониторе
    public RelayCommand InfoCommand { get; private set; }

    #endregion
   

    //TODO Отредактировать после тестирования
    private void AddMonitor()
    {
        var mp = CreateTestMonitoringPoint();

        try
        {
            _mainModel.AddMonitor(mp);
        }
        catch (DuplicateException e)
        {
            DialogCoordinator.Instance.ShowMessageAsync(this,"Ошибка!", e.Message);
        }
    }
   
    private void StartSelected()
    {
        ActionOnSelectedItems(_mainModel.StartMonitorByIndex);
    }

    private void StopSelected()
    {
        ActionOnSelectedItems(_mainModel.StopMonitorByIndex);
    }

    private void RemoveSelectedItemsFromModel()
    {
        ActionOnSelectedItems(_mainModel.DeleteMonitorByIndex);
    }

    private void AddCurrentItemsFromModel()
    {
        AddListBoxItems(_mainModel.MonitorsList);
    }

    private void AddListBoxItems(IList? itemList)
    {
        if (itemList is null)
            return;

        foreach (var item in itemList)
        {
            if (item is not SmbServerMonitor monitor) 
                continue;

            AddListBoxItemBySmbServer(monitor);
        }
    }

    private void AddListBoxItemBySmbServer(SmbServerMonitor monitor)
    {
        var elementVM = new MonitorListItemViewModel(monitor);
        var elementView = new MonitorListItem { DataContext = elementVM };
        
        ItemsList.Add(elementView);

        OnPropertyChanged(nameof(ItemsList));
    }
    
    private void RemoveListBoxItems(IList? itemList)
    {
        if (itemList is null)
            return;

        foreach (var item in itemList)
        {
            if (item is SmbServerMonitor smbItem)
                DeleteListBoxItemBySmbServer(smbItem);
        }
    }

    private void DeleteListBoxItemBySmbServer(SmbServerMonitor monitor)
    {
        var itemToRemove = ItemsList.FirstOrDefault(i
            => (i.DataContext as MonitorListItemViewModel)?.Equals(monitor) ?? false);
        
        if (itemToRemove is null)
            return;

        ItemsList.Remove(itemToRemove);
        (itemToRemove.DataContext as MonitorListItemViewModel)?.Dispose();

        OnPropertyChanged(nameof(ItemsList));
    }

    private void ActionOnSelectedItems(Action<int> action)
    {
        if (SelectedItems == null) return;

        foreach (var item in SelectedItems)
        {
            if (item is not MonitorListItem monitorListItem) continue;
            var index = ItemsList.IndexOf(monitorListItem);
            if (index >=0)
                action.Invoke(index);
        }
    }

    //TODO Только для тестов!!!
    private MonitoringPoint CreateTestMonitoringPoint()
    {
        var ssid = new byte[] { 77, 105, 32, 65, 49 }; //Mi A1
        var credentials = new Credentials("Phoenix", "Samael7777");
      
        var mp = new MonitoringPoint
        {
            Credentials = credentials,
            MonitoringObject = new WifiNetworkIdentifier(ssid).ToString(),
            MonitoringPointType = MonitoringPointType.WifiAP
        };

        return mp;
    }

    private void OnModelItemsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddListBoxItems(args.NewItems);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveListBoxItems(args.OldItems);
                break;
            case NotifyCollectionChangedAction.Replace:
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(args));
        }
    }
}