using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;

namespace CipherCraft.MVVM.ViewModel
{
    public class CypherViewModel : INotifyPropertyChanged
    {
        private string inputText;
        private string outputText;
        private string selectedAlgorithm;

        private static RSAParameters _privateKey;
        private static RSAParameters _publicKey;

        public string InputText
        {
            get => inputText;
            set
            {
                inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        public string OutputText
        {
            get => outputText;
            set
            {
                outputText = value;
                OnPropertyChanged(nameof(OutputText));
            }
        }

        public string SelectedAlgorithm
        {
            get => selectedAlgorithm;
            set
            {
                selectedAlgorithm = value;
                OnPropertyChanged(nameof(SelectedAlgorithm));
            }
        }

        public List<string> Algorithms { get; set; }

        public ICommand EncryptCommand { get; }
        public ICommand DecryptCommand { get; }
        public ICommand ViewHistoryCommand { get; }

        public CypherViewModel()
        {
            Algorithms = new List<string> { "AES", "DES", "RSA" };
            EncryptCommand = new RelayCommand(Encrypt);
            DecryptCommand = new RelayCommand(Decrypt);
            ViewHistoryCommand = new RelayCommand(ViewHistory);
        }

        private void Encrypt(object obj)
        {
            try
            {
                string result = string.Empty;
                switch (SelectedAlgorithm)
                {
                    case "AES":
                        result = AESEncrypt(InputText);
                        break;
                    case "DES":
                        result = DESEncrypt(InputText);
                        break;
                    case "RSA":
                        result = RSAEncrypt(InputText);
                        break;
                    default:
                        OutputText = "Select an algorithm.";
                        return;
                }
                OutputText = result;
                WriteToFile("Encryption", SelectedAlgorithm, InputText, OutputText);
            }
            catch (Exception ex)
            {
                OutputText = $"Encryption error: {ex.Message}";
            }
        }

        private void Decrypt(object obj)
        {
            try
            {
                string result = string.Empty;
                switch (SelectedAlgorithm)
                {
                    case "AES":
                        result = AESDecrypt(InputText);
                        break;
                    case "DES":
                        result = DESDecrypt(InputText);
                        break;
                    case "RSA":
                        result = RSADecrypt(InputText);
                        break;
                    default:
                        OutputText = "Select an algorithm.";
                        return;
                }
                OutputText = result;
                WriteToFile("Decryption", SelectedAlgorithm, InputText, OutputText);
            }
            catch (Exception ex)
            {
                OutputText = $"Decryption error: {ex.Message}";
            }
        }

        private void WriteToFile(string type, string method, string originalText, string outputText)
        {
            string filePath = "encryption_decryption_log.txt";
            string logEntry = $"{type},{method},{originalText},{outputText}";
            File.AppendAllText(filePath, logEntry + Environment.NewLine);
        }
        private void ViewHistory(object obj)
        {
            string filePath = "encryption_decryption_log.txt";
            if (File.Exists(filePath))
            {
                Process.Start(new ProcessStartInfo("notepad.exe", $"/A {filePath}") { UseShellExecute = true });
            }
            else
            {
                OutputText = "History file not found.";
            }
        }


        // AES encryption/decryption methods
        private string AESEncrypt(string plainText)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes key
                    aes.IV = Encoding.UTF8.GetBytes("1234567890123456");  // 16 bytes IV

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return $"AES Encryption error: {ex.Message}";
            }
        }

        private string AESDecrypt(string cipherText)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes key
                    aes.IV = Encoding.UTF8.GetBytes("1234567890123456");  // 16 bytes IV

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"AES Decryption error: {ex.Message}";
            }
        }

        // DES encryption/decryption methods
        private string DESEncrypt(string plainText)
        {
            try
            {
                using (DES des = DES.Create())
                {
                    des.Key = Encoding.UTF8.GetBytes("12345678"); // 8 bytes key
                    des.IV = Encoding.UTF8.GetBytes("87654321");  // 8 bytes IV

                    ICryptoTransform encryptor = des.CreateEncryptor(des.Key, des.IV);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return $"DES Encryption error: {ex.Message}";
            }
        }

        private string DESDecrypt(string cipherText)
        {
            try
            {
                using (DES des = DES.Create())
                {
                    des.Key = Encoding.UTF8.GetBytes("12345678"); // 8 bytes key
                    des.IV = Encoding.UTF8.GetBytes("87654321");  // 8 bytes IV

                    ICryptoTransform decryptor = des.CreateDecryptor(des.Key, des.IV);
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"DES Decryption error: {ex.Message}";
            }
        }

        // RSA encryption/decryption methods
        private void GenerateRSAKeys()
        {
            using (RSA rsa = RSA.Create())
            {
                _privateKey = rsa.ExportParameters(true);
                _publicKey = rsa.ExportParameters(false);
            }
        }

        private string RSAEncrypt(string plainText)
        {
            try
            {
                if (_publicKey.Equals(default(RSAParameters)))
                {
                    GenerateRSAKeys();
                }

                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportParameters(_publicKey);
                    var data = Encoding.UTF8.GetBytes(plainText);
                    var encryptedData = rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
                    return Convert.ToBase64String(encryptedData);
                }
            }
            catch (Exception ex)
            {
                return $"RSA Encryption error: {ex.Message}";
            }
        }

        private string RSADecrypt(string cipherText)
        {
            try
            {
                if (_privateKey.Equals(default(RSAParameters)))
                {
                    GenerateRSAKeys();
                }

                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportParameters(_privateKey);
                    var data = Convert.FromBase64String(cipherText);
                    var decryptedData = rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
                    return Encoding.UTF8.GetString(decryptedData);
                }
            }
            catch (Exception ex)
            {
                return $"RSA Decryption error: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
