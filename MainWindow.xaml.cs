using GithubInfos.code;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Buffers.Text;
using System.Text;

namespace GithubInfos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
			if (string.IsNullOrWhiteSpace(usernameTextBox.Text) || usernameTextBox.Text == "Github Username")
			{
				MessageBox.Show("Set github username", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			else if (string.IsNullOrWhiteSpace(reponameTextBox.Text) || reponameTextBox.Text == "Repository Name")
			{
				MessageBox.Show("Set repository name", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			Http http = new();
			string stringFromUrl = http.GetDownloadString("https://api.github.com/repos/" + usernameTextBox.Text  + "/" + reponameTextBox.Text + "/git/trees/master");
			GithubAPI.Repository repoArray = JsonConvert.DeserializeObject<GithubAPI.Repository>(stringFromUrl);

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
				int numLines = fileTextSrc.Split('\n').Length;
				MessageBox.Show(numLines.ToString());
			}
		}

		private void usernameTextBox_GotFocus(object sender, RoutedEventArgs e){ if (usernameTextBox.Text == "Github Username") usernameTextBox.Text = ""; }
		private void usernameTextBox_LostFocus(object sender, RoutedEventArgs e) { if (string.IsNullOrWhiteSpace(usernameTextBox.Text)) usernameTextBox.Text = "Github Username"; }
		private void reponameTextBox_GotFocus(object sender, RoutedEventArgs e) { if (reponameTextBox.Text == "Repository Name") reponameTextBox.Text = ""; }
		private void reponameTextBox_LostFocus(object sender, RoutedEventArgs e) { if (string.IsNullOrWhiteSpace(reponameTextBox.Text)) reponameTextBox.Text = "Repository Name"; }
	}
}
