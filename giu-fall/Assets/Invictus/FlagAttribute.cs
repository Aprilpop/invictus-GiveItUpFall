using UnityEngine;

public class FlagAttribute : PropertyAttribute
{
    public string enumName;

    public FlagAttribute() { }

    public FlagAttribute(string name)
    {
        enumName = name;
    }
}