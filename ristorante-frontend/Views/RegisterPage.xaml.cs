using ristorante_frontend.Services;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ristorante_frontend.Views
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterBtnClick(object sender, RoutedEventArgs e)
        {
            string email = EmailTxt.Text;
            string password = PasswordTxt.Password;
            string confirmPassword = ConfirmPasswordTxt.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Inserisci tutti i campi.", "Errore", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Le password non coincidono.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Utilizzo combinato di ApiService e HttpClient per la gestione errori
            try
            {
                ApiService.Email = email;
                ApiService.Password = password;
                var apiResult = await ApiService.Register();

                if (apiResult.IsConnectionSuccess && apiResult.Data)
                {
                    MessageBox.Show("Registrazione avvenuta con successo!", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService?.Navigate(new Uri("Views/LoginPage.xaml", UriKind.Relative));
                }
                else
                {
                    // Chiamata aggiuntiva per recuperare il messaggio di errore completo
                    using HttpClient client = new HttpClient();
                    var response = await client.PostAsync($"{ApiService.API_URL}/Account/Register",
                        JsonContent.Create(new { Email = email, Password = password }));

                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Errore: {errorContent}", "Dettaglio errore", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore di connessione: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnNavigateToLoginClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Views/LoginPage.xaml", UriKind.Relative));
        }
    }
}
