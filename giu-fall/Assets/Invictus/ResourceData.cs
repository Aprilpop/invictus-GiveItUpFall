using System;
using UnityEngine;

public abstract class ResourceDataBase
{
    protected string path;
    protected string name;
    public abstract Type ResourceType { get; }
}


public class ResourceData<T> : ResourceDataBase where T : UnityEngine.Object
{
    public override Type ResourceType { get { return typeof(T); } }
    public T Load()
    {
        if (name == null || name.Length == 0)
            return Resources.Load<T>(path);
        else
        {
            T[] objects = Resources.LoadAll<T>(path);
            foreach (T @object in objects)
                if (@object.name == name)
                    return @object;
            return null;
        }
    }
}
