namespace Meta.Settings
{
    public abstract class Settings
    {
        abstract protected string SaveDirectory { get; }
        abstract protected string SaveFileName { get; }
        protected abstract void LoadSettings();
        protected abstract void CreateNewSettings();
    }
}
