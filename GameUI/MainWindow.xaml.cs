using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;

namespace GameUI;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = this;

		IsX = true;
		IsStart = false;

		endPoint = new IPEndPoint(ip, port);
	}


	TcpClient client = new TcpClient();

	IPAddress ip = IPAddress.Parse("127.0.0.1");
	int port = 27001;

	IPEndPoint endPoint;

	public bool IsX { get; set; }
	public bool IsStart { get; set; }

	private void ButtonClick(object sender, RoutedEventArgs e)
	{
		if (!IsStart) return;

		var B = sender as Button;

		//	if (IsX) B.Content = "X";
		//else B.Content = chr;

		//	IsX = !IsX;
		B.IsEnabled = false;


		byte currIndex = (byte)(Convert.ToByte(B.Name.Last()) - 49);

		try
		{
			client.Connect(endPoint);

			if (client.Connected)
			{
				var writer = Task.Run(() =>
				{
					while (true)
					{
						var stream = client.GetStream();
						var bw = new BinaryWriter(stream);

						bw.Write(JsonSerializer.Serialize(currIndex));
					}
				});

				var reader = Task.Run(() =>
				{
					while (true)
					{
						var stream = client.GetStream();
						var br = new BinaryReader(stream);

						var chr = br.ReadString();
						
						switch (currIndex)
						{
							case 0:
								b1.Content = chr;
								break;
							case 1:
								b2.Content = chr;
								break;
							case 2:
								b3.Content = chr;
								break;
							case 3:
								b4.Content = chr;
								break;
							case 4:
								b5.Content = chr;
								break;
							case 5:
								b6.Content = chr;
								break;
							case 6:
								b7.Content = chr;
								break;
							case 7:
								b8.Content = chr;
								break;
							case 8:
								b9.Content = chr;
								break;

						}
					}
				});

				Task.WaitAll(writer, reader);
			}
			else
			{
				Console.WriteLine("NOT Connected.\n");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception occoured.\n\t {ex.Message}\n");
		}




	}

	private void ReadyBtn_Click(object sender, RoutedEventArgs e)
	{
		IsStart = true;
	}
}