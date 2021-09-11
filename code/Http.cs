using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GithubInfos.code
{
	class Http
	{
		public string GetDownloadString(string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			request.ContentType = "application/json";
			request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36"; // Hack to fix error code 403 (maybe changed later for a better way?)
			using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			using Stream stream = response.GetResponseStream();
			using StreamReader reader = new(stream);
			return reader.ReadToEnd();
		}
	}
}
