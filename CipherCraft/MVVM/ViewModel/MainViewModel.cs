using CipherCraft.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CipherCraft.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand MoveWindowCommand { get; set; }
        public RelayCommand ShutdownWindowCommand { get; set; }
        public RelayCommand MaximizeWindowCommand { get; set; }
        public RelayCommand MinimizeWindowCommand { get; set; }

        public RelayCommand ShowProtectionView {  get; set; }
        public RelayCommand ShowSettingsView { get; set; }
        public RelayCommand ShowCypherView { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public ProtectionViewModel ProtectionVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }
        public CypherViewModel CypherVM { get; set; }

        public MainViewModel()
        {
            ProtectionVM = new ProtectionViewModel();
            SettingsVM = new SettingsViewModel();
            CypherVM = new CypherViewModel();

            CurrentView = ProtectionVM;

            Application.Current.MainWindow.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            MoveWindowCommand = new RelayCommand(o => { Application.Current.MainWindow.DragMove(); });
            ShutdownWindowCommand = new RelayCommand(o => { Application.Current.Shutdown(); });

            MaximizeWindowCommand = new RelayCommand(o => 
            { if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
              else
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
            });

            MinimizeWindowCommand = new RelayCommand(o => { Application.Current.MainWindow.WindowState = WindowState.Minimized; });
            
            ShowProtectionView = new RelayCommand(o => { CurrentView = ProtectionVM; });
            ShowSettingsView = new RelayCommand(o => { CurrentView = SettingsVM; });
            ShowCypherView = new RelayCommand(o => { CurrentView = CypherVM; });
        }
    }
}
