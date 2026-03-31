using Microsoft.AspNetCore.Components;

namespace Rcl.Views.Components;

public partial class EnumPicker<T> where T : struct, Enum
{
    [Parameter] public bool Disabled { get; set; }

    [Parameter]
    public T Value
    {
        get;
        set
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            ValueChanged.InvokeAsync(value);
        }
    }

    [Parameter] public EventCallback<T> ValueChanged { get; set; }

    [Parameter]
    public string Label
    {
        get;
        set
        {
            if (EqualityComparer<string>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            LabelChanged.InvokeAsync(value);
        }
    } = "";

    [Parameter] public EventCallback<string> LabelChanged { get; set; }

    [Parameter]
    public Func<T, bool> Filter
    {
        get;
        set
        {
            if (EqualityComparer<Func<T, bool>>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            FilterChanged.InvokeAsync(value);
        }
    } = _ => true;

    public EventCallback<Func<T, bool>> FilterChanged { get; set; }
}
