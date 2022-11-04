﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmbMonitorLib.Services.Base;

public class ModelItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}