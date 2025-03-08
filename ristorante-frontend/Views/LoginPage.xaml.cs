using ristorante_frontend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ristorante_frontend.Views
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private async void OnLoginBtnClickAsync(object sender, RoutedEventArgs e)
        {
            ApiService.Email = EmailTxt.Text;
            ApiService.Password = PasswordTxt.Password;

            var result = await ApiService.GetJwtToken();

            if (result.IsConnectionSuccess && result.Data?.Token != null)
            {
                NavigationService.Navigate(new Uri("Views/HomePage.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show(result.ErrorMessage ?? "Errore durante il login",
                              "Errore",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
        private void OnRegisterBtnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Views/RegisterPage.xaml", UriKind.Relative));
        }
    }
}
