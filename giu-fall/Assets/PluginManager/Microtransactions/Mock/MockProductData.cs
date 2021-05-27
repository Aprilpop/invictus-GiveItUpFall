using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MockProductData : ProductData
{
	public override string ProductId
	{
		get { return productId; }
	}

	public override string Title
	{
		get { return "Title"; }
	}

	public override string Description
	{
		get { return "Description"; }
	}

	public override string FormattedPrice
	{
		get { return "USD 0.99"; }
	}

	private string productId;

	public MockProductData(string productId)
	{
		this.productId = productId;
	}
}
