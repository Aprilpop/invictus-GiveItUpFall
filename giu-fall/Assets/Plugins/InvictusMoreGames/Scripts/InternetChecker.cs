using System;
using System.Net;
using UnityEngine;

namespace InvictusMoreGames
{
	public class InternetChecker : MonoBehaviour
	{
		private Action<bool> connectionResult = delegate { };

		private static InternetChecker instance;

		public static InternetChecker Instance
		{
			get
			{
				if (instance == null)
					instance = FindObjectOfType<InternetChecker>();

				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
		}

		public void IsConnected(Action<bool> result)
		{
			connectionResult = result;
			IPAddress[] serverIPs = null;

			try
			{
				serverIPs = Dns.GetHostAddresses("example.com");
			}
			catch (Exception e)
			{
				connectionResult(false);
				connectionResult = delegate { };
				if (MoreGamesSettings.logMode)
					Logger.LogException(e);
			}

			try
			{
				if (serverIPs != null && serverIPs.Length > 0)
				{
					connectionResult(true);
					connectionResult = delegate { };
				}
				else
				{
					connectionResult(false);
					connectionResult = delegate { };
				}
			}
			catch (Exception e)
			{
				connectionResult(false);
				connectionResult = delegate { };

				if (MoreGamesSettings.logMode)
					Logger.LogException(e);
			}
		}
	}
}