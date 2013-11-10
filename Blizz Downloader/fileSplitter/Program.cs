using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileSplitter
{
	class Program
	{
		static int Main(string[] args)
		{
			if (args.Length != 3)
				Console.WriteLine("usage: fileSplitter <file> <blocksize> <outDir>");

			System.IO.FileStream fs = null;
			int blockSize = 0;
			string outDir = null;

			try
			{
				fs = new System.IO.FileStream(args[0], System.IO.FileMode.Open);

				if (args[1].StartsWith("0x"))
					blockSize = Convert.ToInt32(args[1], 16);
				else
					blockSize = Convert.ToInt32(args[1], 10);

				if (!System.IO.Directory.Exists(args[2]))
					outDir = System.IO.Directory.CreateDirectory(args[2]).FullName + '\\';
				else
					outDir = new System.IO.DirectoryInfo(args[2]).FullName + "\\";
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return 1;
			}

			Console.WriteLine("Splitting {0} to {1} into {2} bytes blocks.", fs.Name, outDir, blockSize);

			long remaining = fs.Length;
			int packetNbr = 0;

			System.IO.StreamWriter hashFile = new System.IO.StreamWriter(outDir + "hash.txt");

			int qzdqzdqd = 0;

			while (remaining > 0)
			{
				//	Write packet file
				System.IO.FileStream packetFile = new System.IO.FileStream(outDir + packetNbr.ToString(), System.IO.FileMode.Create);
				
				byte[] buffer = new byte[blockSize];
				int BytesRead = fs.Read(buffer, 0, blockSize);

				Console.WriteLine("Writting {0} to {1}.", BytesRead, packetFile.Name);
				packetFile.Write(buffer, 0, BytesRead);
				packetFile.Flush();

				//	Calculate sha1 hash
				System.Security.Cryptography.SHA1 sha1CSP = new System.Security.Cryptography.SHA1CryptoServiceProvider();
				byte[] hash = sha1CSP.ComputeHash(buffer, 0, BytesRead);

				string sHash = BitConverter.ToString(hash);
				sHash = sHash.Replace("-", "");
				qzdqzdqd += sHash.Length;

				//	write hash
				hashFile.Write(sHash);

				remaining -= BytesRead;
				packetNbr++;
			}

			hashFile.Flush();

			return 0;
		}
	}
}
