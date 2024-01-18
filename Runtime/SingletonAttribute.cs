using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class SingletonAttribute : System.Attribute
{
	public enum AssetSource
	{
		Resources,
		Addressables // TODO: not implemented
	}

	public string Path { get; set; } = string.Empty;

	public AssetSource Source { get; set; } = AssetSource.Resources;

	public SingletonAttribute() : this(string.Empty)
	{
	}

	public SingletonAttribute(string path)
	{
		Path = path;
	}
}
