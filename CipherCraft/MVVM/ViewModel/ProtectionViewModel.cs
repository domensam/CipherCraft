using CipherCraft.Core;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace CipherCraft.MVVM.ViewModel
{
    class ProtectionViewModel : ObservableObject
    {
        public ObservableCollection<ServerModel> Servers { get; set; }

        private string _connectionStatus;

        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                _connectionStatus = value;
                OnPropertyChanged(nameof(ConnectionStatus));
                UpdateButtonContent();
            }
        }

        private string _buttonContent = "Connect";

        public string ButtonContent
        {
            get { return _buttonContent; }
            set
            {
                _buttonContent = value;
                OnPropertyChanged(nameof(ButtonContent));
            }
        }

        public RelayCommand ConnectCommand { get; set; }

        public ProtectionViewModel()
        {
            Servers = new ObservableCollection<ServerModel>();
            for (int i = 0; i < 10; i++)
            {
                Servers.Add(new ServerModel
                {
                    Country = "USA"
                });
            }

            _connectionStatus = "Status: Disconnected";

            ConnectCommand = new RelayCommand(o =>
            {
                if (ConnectionStatus.Contains("Connected"))
                {
                    DisconnectVPN();
                }
                else
                {
                    ConnectVPN();
                }
            });
        }

        private void ConnectVPN()
        {
            ConnectionStatus = "Status: Connecting...";
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            process.StartInfo.ArgumentList.Add(@"/c rasdial MyServer vpnbook dnx97sa /phonebook:./VPN/VPN.pbk");

            process.Start();
            process.WaitForExit();

            switch (process.ExitCode)
            {
                case 0:
                    ConnectionStatus = "Status: Connected";
                    MessageBox.Show("Success!");
                    break;
                case 691:
                    MessageBox.Show("Wrong Credentials!");
                    break;
                case 623:
                    MessageBox.Show("Error 623: VPN Phonebook Entry Not Found");
                    break;
                default:
                    MessageBox.Show($"Error: {process.ExitCode}");
                    break;
            }
        }

        private void DisconnectVPN()
        {
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            process.StartInfo.Arguments = @"/c rasdial /disconnect";

            process.Start();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                ConnectionStatus = "Status: Disconnected";
                MessageBox.Show("Disconnected!");
            }
            else
            {
                MessageBox.Show($"Error: {process.ExitCode}");
            }
        }

        private void UpdateButtonContent()
        {
            if (ConnectionStatus.Contains("Connected"))
            {
                ButtonContent = "Disconnect";
            }
            else
            {
                ButtonContent = "Connect";
            }
        }

        private void ServerBuilder()
        {
            var address = "US2.vpnbook.com";
            var FolderPath = $"{Directory.GetCurrentDirectory()}/VPN";
            var Pbkpath = $"{FolderPath}/{address}.pbk";

            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);
            if (File.Exists(Pbkpath))
            {
                MessageBox.Show("Connection already exists!");
                return;
            }
            var sb = new StringBuilder();
            sb.AppendLine("[MyServer]");
            sb.AppendLine("MEDIA=rastapi");
            sb.AppendLine("Port=VPN2-0");
            sb.AppendLine("Device=WAN Miniport (IKEv2)");
            sb.AppendLine("DEVICE=vpn");
            sb.AppendLine($"PhoneNumber={address}");
            File.WriteAllText(Pbkpath, sb.ToString());
        }
    }
}
