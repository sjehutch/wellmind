using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace WellMind.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string TodayDisplay => $"Today • {DateTime.Now.ToString("dddd, MMMM d", CultureInfo.CurrentCulture)}";

    protected void SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
        {
            return;
        }

        backingField = value;
        RaisePropertyChanged(propertyName);
    }

    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
