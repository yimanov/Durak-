using System;

public class WindowView : PreInitedMonoBehaviour
{
    /// <summary>
    /// Show window with data process.
    /// </summary>
    /// <param name="data"></param>
    public virtual void Show(WindowData data)
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide window process
    /// </summary>
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    protected override void PreInitialize()
    {
    }

    protected override void Dispose()
    {
    }
}

public class WindowData
{

}
