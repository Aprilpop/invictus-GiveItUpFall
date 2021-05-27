using UnityEngine;
using System;

public sealed class ResourcePathAttribute : PropertyAttribute
{
    Type resourceType;
    public Type ResourceType{ get{ return resourceType; }}
	public ResourcePathAttribute(Type type) {
        if (type != null)
            resourceType = type;
        else
            throw new NullReferenceException("Type cannot be NULL");
    }
}