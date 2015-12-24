namespace BigData.UI.Client
{
    public interface IShellViewModel
    {
        bool IsFlyoutOpen { get; set; }
        string FlyoutTitle { get; set; }
        string StatusBarText { get; set; }
    }
}