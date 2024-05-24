namespace OrderManager.ViewModel;
public static class FocusMessenger
{
    public delegate void FocusRequestHandler(string target);
    public static event FocusRequestHandler FocusRequested;

    public static void RequestFocus(string target)
    {
        FocusRequested?.Invoke(target);
    }
}
