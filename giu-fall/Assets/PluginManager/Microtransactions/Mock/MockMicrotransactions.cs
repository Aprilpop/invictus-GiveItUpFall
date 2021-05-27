using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MockMicrotransactions : Microtransactions
{
	public override void RequestProductData(string[] productNames, System.Action<Microtransactions.ProductDataResult> result)
	{
		if (result != null)
		{
			List<ProductData> list = new List<ProductData>();
			foreach (string name in productNames)
				list.Add(new MockProductData(GetProductId(name)));
			
			result(new ProductDataResult(true, null, list));
			//result(new ProductDataResult(false, "Platform nem támogatott", null));
		}
	}

	public override void BuyProduct(string productName, System.Action<Microtransactions.BuyProductResult> result)
	{
		if (result != null)
		{
			result (new BuyProductResult(true, null));
			//result(new BuyProductResult(false, "Platform nem támogatott"));
		}
	}

	public override void ConsumeProduct(string productName, System.Action<Microtransactions.BuyProductResult> result)
	{
		if (result != null)
		{
			result(new BuyProductResult(true, null));
			//result(new BuyProductResult(false, "Platform nem támogatott"));
		}
	}

	public override void BuyProduct(string productName, bool autoConsume, System.Action<Microtransactions.BuyProductResult> result)
	{
		if (result != null)
		{
			result(new BuyProductResult(true, null));
			//result(new BuyProductResult(false, "Platform nem támogatott"));
		}
	}


	public override void RestorePurchases(System.Action<Microtransactions.RestoreResult> result, System.Action<Microtransactions.RestoredProductHandler> productHandler)
	{
		if (result != null)
		{
			result(new RestoreResult(false, "Platform nem támogatott"));
		}

	}
}
