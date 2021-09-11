using GithubInfos.code;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Dynamic;
using System.Collections.Generic;

namespace GithubInfos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private List<JObject> jObject = new List<JObject>();

		public MainWindow()
        {
            InitializeComponent();
		}

		private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Close();
		}

		private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Mouse.LeftButton == MouseButtonState.Pressed) // Fix crash when dragging by holding right click and prevent it
			{
				DragMove();
			}
		}

		private void Label_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			loadingGif.Visibility = Visibility.Visible;

			if (string.IsNullOrWhiteSpace(usernameTextBox.Text) || usernameTextBox.Text == "Github Username")
			{
				MessageBox.Show("Set github username", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			jObject = new List<JObject>();
			repoComboBox.Items.Clear();

			Http http = new();
			string stringFromUrl = http.GetDownloadString("https://api.github.com/users/" + usernameTextBox.Text + "/repos");

			JArray ja = (JArray)JsonConvert.DeserializeObject(stringFromUrl);

			foreach (JObject jr in ja)
			{
				jObject.Add((JObject)jr);
			}

			for (int i = 0; i < jObject.Count; i++)
			{
				repoComboBox.Items.Add((string)jObject[i]["name"]);
			}

			startButton.IsEnabled = true;
			repoComboBox.IsDropDownOpen = true;
			loadingGif.Visibility = Visibility.Hidden;
		}

		private void usernameTextBox_GotFocus(object sender, RoutedEventArgs e){ if (usernameTextBox.Text == "Github Username") usernameTextBox.Text = ""; }
		private void usernameTextBox_LostFocus(object sender, RoutedEventArgs e) { if (string.IsNullOrWhiteSpace(usernameTextBox.Text)) usernameTextBox.Text = "Github Username"; }

		private void startButton_Click(object sender, RoutedEventArgs e)
		{
			Console.Beep();
			MessageBox.Show(usernameTextBox.Text + "/" + (string)jObject[repoComboBox.SelectedIndex]["name"]);

			Http http = new();
			string stringFromUrl = http.GetDownloadString("https://api.github.com/repos/" + usernameTextBox.Text + "/" + (string)jObject[repoComboBox.SelectedIndex]["name"] + "/git/trees/master");
			GithubAPI.Repository repoArray = JsonConvert.DeserializeObject<GithubAPI.Repository>(stringFromUrl);

			int totalLines = 0;
			int totalSize = 0;
			foreach (GithubAPI.Repository.Tree a in repoArray.tree)
			{
				stringFromUrl = http.GetDownloadString(a.url);
				GithubAPI.File fileArray = JsonConvert.DeserializeObject<GithubAPI.File>(stringFromUrl);
				string fileTextSrc;
				try
				{
					fileTextSrc = Encoding.UTF8.GetString(Convert.FromBase64String(fileArray.content)); // File source
				}
				catch
				{
					continue;
				}
				totalLines += fileTextSrc.Split('\n').Length;
				totalSize += fileArray.size;
			}
			MessageBox.Show("Total size: " + BytesFormatted(totalSize));
			MessageBox.Show("Total lines: " + totalLines.ToString());
		}

		private string BytesFormatted(int bytes)
		{
			double result = bytes / 1024d;
			if (result < 1000) return $"{Math.Round(bytes / 1024d, 2)}ko";
			else if (result < 1000000) return $"{Math.Round(bytes / 1024d / 1024d, 2)}mo";
			else if (result < 1000000000) return $"{Math.Round(bytes / 1024d / 1024d / 1024d, 2)}go";
			else return $"{Math.Round(bytes / 1024d / 1024d / 1024d / 1024d, 2)}to";
		}
	}
}
