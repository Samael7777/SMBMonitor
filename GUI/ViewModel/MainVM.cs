using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using SMBMonitor.Annotations;
using SMBMonitor.Exceptions;
using SMBMonitor.Model;
using SMBMonitor.View;
using SmbMonitorLib;
using SmbMonitorLib.Wifi;

namespace SMBMonitor.ViewModel;

public class MainVM : INotifyPropertyChanged
{
    private readonly MainModel _model;
    private readonly MainWindow _mainWindow;
    private readonly ConcurrentBag<int> _selectedItemsIndexes;


    private RelayCommand? _addCommand;
    private RelayCommand? _removeCommand;
    private RelayCommand? _startCommand;
    private RelayCommand? _stopCommand;
    private RelayCommand? _exitCommand;
    private RelayCommand? _settingsCommand;
    private RelayCommand? _infoCommand;
    private RelayCommand? _selectAllCommand;
    private RelayCommand? _SelectNoneCommand;

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainVM(MainModel model, MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        _mainWindow.DataContext = this;

        ItemsList = new ObservableCollection<MonitorListItem>();
        _selectedItemsIndexes = new ConcurrentBag<int>();

        _model = model;
        _model.ServerMonitors.CollectionChanged += OnModelItemsChanged;
    }

    public ObservableCollection<MonitorListItem> ItemsList { get; set; }

    public void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedItemsIndexes.Clear();
        var items = (sender as ListBox)?.SelectedItems;
        if (items == null) 
            return;

        foreach (var item in items)
        {
            if (item is not MonitorListItem listItem)
                continue;

            _selectedItemsIndexes.Add(ItemsList.IndexOf(listItem));
        }
    }

    //TODO ADD СДЕЛАНО ДЛЯ ТЕСТИРОВАНИЯ, ПЕРЕДЕЛАТЬ!!!!
    public RelayCommand AddCommand 
        => _addCommand ??= new RelayCommand(_ => AddMonitor());
   
    public RelayCommand RemoveCommand 
        => _removeCommand ??= new RelayCommand(_ => RemoveItemsFromModel());

    public RelayCommand StartCommand
        => _startCommand ??= new RelayCommand(_ => StartSelected());

    public RelayCommand StopCommand
        => _stopCommand ??= new RelayCommand(_ => StopSelected());

    public RelayCommand ExitCommand
        => _exitCommand ??= new RelayCommand(_ => Application.Current.Shutdown());

    //TODO сделать настройки
    public RelayCommand SettingsCommand
        => _settingsCommand ??= new RelayCommand(_ => throw new NotImplementedException());
    
    //TODO сделать информацию о мониторе
    public RelayCommand InfoCommand
        => _infoCommand ??= new RelayCommand(_ => throw new NotImplementedException());

    public RelayCommand SelectAllCommand
        => _selectAllCommand ??= new RelayCommand(_ => SelectAll());

    public RelayCommand SelectNoneCommand
        => _SelectNoneCommand ??= new RelayCommand(_ => SelectNone());

    //TODO Отредактировать после тестирования
    private void AddMonitor()
    {
        var mp = CreateTestMonitoringPoint();

        try
        {
            _model.AddMonitor(mp);
        }
        catch (DuplicateException e)
        {
            MessageBox.Show(e.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void SelectAll()
    {
        _mainWindow.MonitorsList.SelectAll();
    }

    private void SelectNone()
    {
        _mainWindow.MonitorsList.UnselectAll();
    }
    
    private void StartSelected()
    {
        _model.StartSelected(_selectedItemsIndexes);
    }

    private void StopSelected()
    {
        _model.StopSelected(_selectedItemsIndexes);
    }

    private void RemoveItemsFromModel()
    {
        foreach (var item in _selectedItemsIndexes)
        {
            _model.RemoveMonitorByIndex(item);
        }
    }

    private void OnModelItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
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