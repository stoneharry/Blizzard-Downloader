using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace TinyTracker
{
	class Program
	{
		static byte[] GetCompactPeerAddress(string ip, string port)
		{
			byte[] buffer = new byte[6];
			IPAddress add = IPAddress.Parse(ip);
			add.GetAddressBytes().CopyTo(buffer, 0);
			BitConverter.GetBytes(ushort.Parse(port)).CopyTo(buffer, 4);

			return buffer;
		}

		static void Main(string[] args)
		{
			HttpListener listener = new HttpListener();
			listener.Prefixes.Clear();
            listener.Prefixes.Add("http://*:3724/announce/");

			listener.Start();

			while (true)
			{
				HttpListenerContext context = listener.GetContext();

				HttpListenerRequest req = context.Request;

				string ip = req.QueryString["ip"];
				string port = req.QueryString["port"];
				string public_ip = req.RemoteEndPoint.Address.ToString();
				string info_hash = req.QueryString["info_hash"];

				//string directDownloadURL = "http://blizzard.vo.llnwd.net/o16/content/ptr/WoW-4.0.0.12319-to-4.0.0.12479-enUS-patch.exe";
                //string directDownloadURL = "http://eoc.dispersion-wow.com/resources/Wow.exe";
                string directDownloadURL = "http://eoc.dispersion-wow.com/resources/Repair.exe";
				string s = "d6:directd3:url" + directDownloadURL.Length + ":" + directDownloadURL +
					"9:thresholdi10000000ee8:intervali1800e2:ip" + public_ip.Length + ":" + public_ip + "5:peers6:";
				
				//	Get response string and add 1 peer (compact address) wich actually is requester himself
				byte[] buffer = Encoding.UTF8.GetBytes(s);
				Array.Resize(ref buffer, buffer.Length + 7);
				GetCompactPeerAddress(ip, port).CopyTo(buffer, buffer.Length - 7);
				buffer[buffer.Length - 1] = (byte)'e';

				//	Set up response
				HttpListenerResponse resp = context.Response;
				resp.ContentType = null;
				resp.StatusCode = 200;
				resp.ContentLength64 = buffer.Length;

				//	send response string
				System.IO.Stream output = resp.OutputStream;
				output.Write(buffer, 0, buffer.Length);
			}
		}
	}
}
