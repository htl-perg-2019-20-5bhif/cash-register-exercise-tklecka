using System.Windows;

namespace cash_register_ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel = new MainWindowViewModel();
            Loaded += async (_, __) => await ViewModel.InitAsync();
        }
    }
}
