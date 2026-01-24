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
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
