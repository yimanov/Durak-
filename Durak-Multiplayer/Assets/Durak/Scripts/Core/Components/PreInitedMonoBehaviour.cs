using UnityEngine;

public abstract class PreInitedMonoBehaviour : MonoBehaviour
{
    public virtual void Awake()
    {
        PreInitialize();
    }

    public virtual void OnDestroy()
    {
        Dispose();
    }

    /// <summary>
    /// The main goal is pre init all parameters, links, and things that neccesary for correct work of specific component.
    /// </summary>
    protected abstract void PreInitialize();

    /// <summary>
    /// The main goal is do smth when object should destroy.
    /// </summary>
    protected virtual void Dispose()
    { 
    
    }
}
