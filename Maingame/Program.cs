
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;

TcpListener listener = null;
BinaryReader br = null;
BinaryWriter bw = null;
List<TcpClient> clients = [];

var ip = IPAddress.Parse("127.0.0.1");
var port = 27001;
var endPoint = new IPEndPoint(ip, port);
listener = new TcpListener(endPoint);
listener.Start();
Console.WriteLine($"Listening on {listener.LocalEndpoint}.");

bool isX = true;
byte[] arr = new byte[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

while (true)
{
	var client = listener.AcceptTcpClient();
	clients.Add(client);

	Console.WriteLine($"{client.Client.RemoteEndPoint} connected...");

	var reader = Task.Run(() =>
	{
		foreach (var client in clients)
		{
			Task.Run(() =>
			{
				while (true)
				{
					var stream = client.GetStream();
					br = new BinaryReader(stream);

					try
					{
						var msg = br.ReadString();

						byte indx = JsonSerializer.Deserialize<byte>(msg);

						if (isX)
						{
							arr[indx] = 1;
						}
						else
						{
							arr[indx] = 2;
						}
						isX = !isX;

					}
					catch (Exception ex)
					{
						Console.WriteLine($"{client.Client.RemoteEndPoint} removed.");
						clients.Remove(client);
					}
				}
			}).Wait(50);
		}
	});

	var writer = Task.Run(() =>
	{
		while (true)
		{
			foreach (var client in clients)
			{
				var stream = client.GetStream();
				bw = new BinaryWriter(stream);
				var msg = "";

				if (isX)
				{
					msg = "X";
				}
				else
				{
					msg = "O";
				}
				var cubicSize = 3;
				for (int i = 0; i < cubicSize; i++)
				{
					if ((arr[i * cubicSize] == arr[i * cubicSize + 1]) &&
						(arr[i * cubicSize] == arr[i * cubicSize + 2]) &&
						(arr[i * cubicSize] != ' '))
					{
						msg = arr[i * cubicSize] == 1 ? "X" : "O";
						break;
					}
				}

				for (int i = 0; i < cubicSize; i++)
				{
					if ((arr[i] == arr[i + 3]) &&
						(arr[i] == arr[i + 6]) &&
						(arr[i] != ' '))
					{
						msg = arr[i] == 1 ? "X" : "O";
						break;
					}
				}

				if (((arr[0] == arr[4]) && (arr[0] == arr[8]) && (arr[0] != ' ')) ||
					((arr[2] == arr[4]) && (arr[2] == arr[6]) && (arr[2] != ' ')))
				{
					msg = arr[4] == 1 ? "X" : "O";
					break;
				}

				bw.Write(msg);
			}
		}
	});

	Task.WaitAll(reader, writer);
}
