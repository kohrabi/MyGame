using System;

namespace MyEngine.Utils.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class SerializeField : Attribute
{
}