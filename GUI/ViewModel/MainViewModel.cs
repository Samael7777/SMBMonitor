using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using SMBMonitor.Annotations;
using SMBMonitor.Exceptions;
using SMBMonitor.Model;
using SMBMonitor.View;
using SmbMonitorLib;
using SmbMonitorLib.Wifi;

namespace SMBMonitor.ViewModel;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly MainModel _mainModel;
    private readonly ConcurrentBag<int> _selectedItemsIndexes;

    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public MainViewModel(MainModel mainModel)
    {
       ItemsList = new ObservableCollection<MonitorListItem>();
        _selectedItemsIndexes = new ConcurrentBag<int>();
        SelectedItems = new ObservableCollection<MonitorListItem>();

        _mainModel = mainModel;
        _mainModel.ServerMonitors.CollectionChanged += OnModelItemsChanged;

        AddCommand = new RelayCommand(_ => AddMonitor());
        RemoveCommand = new RelayCommand(_ => RemoveItemsFromModel());
        StartCommand = new RelayCommand(_ => StartSelected());
        StopCommand = new RelayCommand(_ => StopSelected());
        SettingsCommand = new RelayCommand(_ => ShowSettings());
        InfoCommand = new RelayCommand(_ => throw new NotImplementedException());
        ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());
        ItemsChanged = new RelayCommand(OnSelectionChanged);
    }

    public ObservableCollection<MonitorListItem> ItemsList { get; set; }

    public ObservableCollection<MonitorListItem> SelectedItems { get; set; }

   //TODO ADD СДЕЛАНО ДЛЯ ТЕСТИРОВАНИЯ, ПЕРЕДЕЛАТЬ!!!!
    public RelayCommand AddCommand { get; }

    public RelayCommand RemoveCommand { get; }

    public RelayCommand StartCommand { get; }

    public RelayCommand StopCommand { get; }

    public RelayCommand ExitCommand { get; }

    public RelayCommand SettingsCommand { get; }

    public RelayCommand ItemsChanged { get; }
    
    //TODO сделать информацию о мониторе
    public RelayCommand InfoCommand { get; }

    public void OnModelItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddListBoxItems(e.NewItems);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveListBoxItems(e.OldItems);
                break;
            case NotifyCollectionChangedAction.Replace:
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnSelectionChanged(object? param)
    {
        _selectedItemsIndexes.Clear();

        if (param is not IList selectedItems) return;

        foreach (var item in selectedItems)
        {
            if (item is not MonitorListItem listItem)
                continue;

            _selectedItemsIndexes.Add(ItemsList.IndexOf(listItem));
        }
    }
    
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
            MessageBox.Show(e.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
   
    private void StartSelected()
    {
        _mainModel.StartSelected(_selectedItemsIndexes);
    }

    private void StopSelected()
    {
        _mainModel.StopSelected(_selectedItemsIndexes);
    }

    private void RemoveItemsFromModel()
    {
        foreach (var item in _selectedItemsIndexes)
        {
            _mainModel.RemoveMonitorByIndex(item);
        }
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
        var elementVM = new MonitorListItemVM(monitor);
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
            => (i.DataContext as MonitorListItemVM)?.Equals(monitor) ?? false);
        
        if (itemToRemove is null)
            return;

        ItemsList.Remove(itemToRemove);
        (itemToRemove.DataContext as MonitorListItemVM)?.Dispose();

        OnPropertyChanged(nameof(ItemsList));
    }

    private void ShowSettings()
    {
        WindowsContainer.SettingsWindow.Show();
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    //TODO Только для тестов!!!
    private MonitoringPoint CreateTestMonitoringPoint()
    {
        var ssid = new byte[] { 77, 105, 32, 65, 49 }; //Mi A1
        var credentials = new Credentials()
        {
            User = "Phoenix",
            Password = "Samael7777"
        };

        var mp = new MonitoringPoint()
        {
            MonitoringObject = new WifiNetworkIdentifier(ssid),
            Credentials = credentials
        };

        return mp;
    }
}