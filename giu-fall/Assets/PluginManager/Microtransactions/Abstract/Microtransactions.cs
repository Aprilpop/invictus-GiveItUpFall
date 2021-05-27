using UnityEngine;
using System.Collections.Generic;

public abstract class Microtransactions
{
	public class ProductDataResult
	{
		public bool success;
		public string error;
		public List<ProductData> result;

		public ProductDataResult(bool success, string error, List<ProductData> result)
		{
			this.success = success;
			this.error = error;
			this.result = result;
		}
	}

	public class BuyProductResult
	{
		public bool success;
		public string error;

		public BuyProductResult(bool success, string error)
		{
			this.success = success;
			this.error = error;
		}
	}

	public class RestoreResult
	{
		public bool success;
		public string error;

		public RestoreResult(bool success, string error)
		{
			this.success = success;
			this.error = error;
		}
	}

	public class RestoredProductHandler
	{
		public List<string> restoredProductNames;

		public RestoredProductHandler(string restoredProductName)
		{
			this.restoredProductNames = new List<string>() {restoredProductName};
		}

		public RestoredProductHandler(List<string> restoredProductNames)
		{
			this.restoredProductNames = restoredProductNames;
		}
	}

	public virtual void Initialize()
	{
	}

	// productName => productId
	protected Dictionary<string, string> productNameToId = new Dictionary<string, string>();

	public void AddProduct(string productName, string productId)
	{
		if (!productNameToId.ContainsKey(productName))
			productNameToId[productName] = productId;
	}

	public string GetProductId(string productName)
	{
		string id = string.Empty;
		if (!productNameToId.TryGetValue(productName, out id))
			Debug.Log("Nincs ilyen product név felvéve: " + productName);
		return id;
	}

	public string[] GetProductIds(string[] productNames)
	{
		//string[] result = new string[productNames.Length];
		List<string> result = new List<string>(productNames.Length);
		for (int i = 0; i < productNames.Length; i++)
		{
			string itemName = GetProductId(productNames[i]);
			if (itemName != null && itemName != string.Empty)
				result.Add(itemName);
		}
		return result.ToArray();
	}

	public string GetProductName(string productId)
	{
		foreach (KeyValuePair<string, string> pair in productNameToId)
		{
			if (pair.Value == productId)
				return pair.Key;
		}
		//Logger.Log("Nincs ilyen product ID felvéve: " + productId);
		return string.Empty;
	}

	public void RequestProductData(string productIdentifier, System.Action<ProductDataResult> result)
	{
		this.RequestProductData(new string[] { productIdentifier }, result);
	}

	public abstract void RequestProductData(string[] productNames, System.Action<ProductDataResult> result);

	public abstract void BuyProduct(string productName, bool autoConsume, System.Action<BuyProductResult> result);

	public abstract void BuyProduct(string productName, System.Action<BuyProductResult> result);

	public abstract void ConsumeProduct(string productName, System.Action<BuyProductResult> result);

	public abstract void RestorePurchases(System.Action<Microtransactions.RestoreResult> result, System.Action<Microtransactions.RestoredProductHandler> productHandler);
}
