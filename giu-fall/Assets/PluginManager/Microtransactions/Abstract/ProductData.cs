using UnityEngine;
using System.Collections;

public abstract class ProductData
{
	public abstract string ProductId
	{
		get;
	}

	public abstract string Title
	{
		get;
	}

	public abstract string Description
	{
		get;
	}

	public abstract string FormattedPrice
	{
		get;
	}

}
