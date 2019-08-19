using System.Text;
using System;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.CompilerServices;


	public sealed class CryptoTool
	{
		private readonly object cryptLock = new object();
		private readonly string pw;
		private readonly byte[] salt;
		private Aes aes;
		private ICryptoTransform encryptor
		{
			get
			{
				if (currentEncryptor == null || currentEncryptor.CanReuseTransform == false)
					currentEncryptor = aes.CreateEncryptor();
				return currentEncryptor;
			}
		}
		private ICryptoTransform decryptor
		{
			get
			{
				if (currentDecryptor == null || currentDecryptor.CanReuseTransform == false)
					currentDecryptor = aes.CreateDecryptor();
				return currentDecryptor;
			}
		}

		private ICryptoTransform currentEncryptor;
		private ICryptoTransform currentDecryptor;

		public CryptoTool(string pw, string salt)
		{
			if (salt.Length != 20)
				throw new Exception("Invalid salt length. Must be 20");
			this.pw = pw;
			this.salt = Encoding.ASCII.GetBytes(salt);
			Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(this.pw, this.salt);
			aes = new AesManaged();
			aes.Key = pdb.GetBytes(aes.KeySize / 8);
			aes.IV = pdb.GetBytes(aes.BlockSize / 8);
		}
		public byte[] Encrypt(byte[] input)
		{
			lock (cryptLock)
			{
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
					{
						cs.Write(input, 0, input.Length);
					}
					return ms.ToArray();
				}
			}
		}
		public byte[] Decrypt(byte[] input)
		{
			lock (cryptLock)
			{
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
					{
						cs.Write(input, 0, input.Length);
					}
					return ms.ToArray();
				}
			}
		}
	}
