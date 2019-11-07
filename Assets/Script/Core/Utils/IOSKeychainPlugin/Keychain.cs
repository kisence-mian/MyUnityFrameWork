#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
#define KEYCHAIN_AVAILABLE
#endif
using UnityEngine;
#if KEYCHAIN_AVAILABLE
using System.Runtime.InteropServices;
#else
using System.IO;
using System.Text;
#endif

	public static class Keychain
	{
#if KEYCHAIN_AVAILABLE
		[DllImport("__Internal")]
		private static extern string getKeyChainValue(string key);
		[DllImport("__Internal")]
		private static extern void setKeyChainValue(string key, string value);
		[DllImport("__Internal")]
		private static extern void deleteKeyChainValue(string key);
#else
		private static CryptoTool mCryptoInstance = null;
		static Keychain()
		{
			mCryptoInstance = new CryptoTool("oPXJN744LGH5v2pX3BVj", "KlUiCgYcoHBzB8sjYA4z");
		}
		private static string GetPath(string key)
		{
			return Path.Combine(Application.persistentDataPath, string.Format("keychain-{0}.dat", key));
		}
#endif
		public static string GetValue(string key)
		{
			try
			{
#if KEYCHAIN_AVAILABLE
				return getKeyChainValue(key);
#else
				string path = GetPath(key);
				if (File.Exists(path))
				{
					var bytes = File.ReadAllBytes(path);
					return Encoding.UTF8.GetString(mCryptoInstance.Decrypt(bytes));
				}
				return string.Empty;
#endif
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
				return string.Empty;
			}
		}
		public static void SetValue(string key, string value)
		{
			try
			{
#if KEYCHAIN_AVAILABLE
				setKeyChainValue(key, value);
#else
				string path = GetPath(key);
				File.WriteAllBytes(path, mCryptoInstance.Encrypt(Encoding.UTF8.GetBytes(value)));
#endif
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		public static void DeleteValue(string key)
		{
			try
			{
#if KEYCHAIN_AVAILABLE
				deleteKeyChainValue(key);
#else
				string path = GetPath(key);
				if (File.Exists(path))
					File.Delete(path);
#endif
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}
		}
}
