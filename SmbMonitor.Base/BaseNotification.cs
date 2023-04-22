using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmbMonitor.Base;

public abstract class BaseNotification : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName]string propertyName="")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName]string propertyName="")
    {
        if (IsValuesEqual(field, value)) return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected bool SetProperty<T>(object target, string propertyName, T value, [CallerMemberName] string callerPropertyName = "")
    {
        var baseProp = target.GetType().GetProperty(propertyName) 
                       ?? throw new ArgumentException(nameof(propertyName));

        var oldValue = baseProp.GetValue(target);

        if (IsValuesEqual(oldValue, value)) 
            return false;

        baseProp.SetValue(target, value);

        OnPropertyChanged(callerPropertyName);
        return true;
    }

    private static bool IsValuesEqual<T>(T x, T y)
    {
        if (x is null) return y == null;
        return y is { } && EqualityComparer<T>.Default.Equals(x, y);
    }
}