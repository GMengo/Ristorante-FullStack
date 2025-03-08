using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Input;
using ristorante_frontend.Models;
using Newtonsoft.Json;
using System.Text;
using System.Windows;
using System; // Aggiunto per Exception e Console
using System.Collections.Generic; // Aggiunto per List<T>
using System.Threading.Tasks;
using ristorante_frontend.Services; // Aggiunto per Task

namespace ristorante_frontend.ViewModels
{
    public class PiattoViewModel : INotifyPropertyChanged
    {
        private Jwt _token;

        public PiattoViewModel()
        {
            Piatti = new ObservableCollection<Piatto>();
            SelectedPiatto = new Piatto();
            LoadPiattiCommand = new MyCommand(async () => await LoadPiatti());
            AddPiattoCommand = new MyCommand(async () => await AddPiatto());
            DeletePiattoCommand = new MyCommand(async () => await DeletePiatto());
            UpdatePiattoCommand = new MyCommand(async () => await UpdatePiatto());
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            // Richiedo il JWT
            var tokenApiResult = await ApiService.GetJwtToken();
            if (tokenApiResult.Data == null)
            {
                MessageBox.Show($"ERRORE di login! {tokenApiResult.ErrorMessage}");
                return;
            }
            _token = tokenApiResult.Data;
            await LoadPiatti();
        }

        private ObservableCollection<Piatto> _piatti;
        public ObservableCollection<Piatto> Piatti
        {
            get => _piatti;
            set
            {
                _piatti = value;
                OnPropertyChanged(nameof(Piatti));
            }
        }

        private Piatto _selectedPiatto;
        public Piatto SelectedPiatto
        {
            get => _selectedPiatto;
            set
            {
                _selectedPiatto = value;
                OnPropertyChanged(nameof(SelectedPiatto));
                (DeletePiattoCommand as MyCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand LoadPiattiCommand { get; }
        public ICommand AddPiattoCommand { get; }
        public ICommand DeletePiattoCommand { get; }
        public ICommand UpdatePiattoCommand { get; }

        private async Task LoadPiatti()
        {
            try
            {
                ApiServiceResult<List<Piatto>> createApiResult = await ApiService.GetPiatto();
                Piatti = new ObservableCollection<Piatto>(createApiResult.Data);
                SelectedPiatto = new Piatto();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nel caricamento dei piatti: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void VerificaPiatto(Piatto piatto)
        {
            // di default il SelectPiatto ha prezzo 0, valore che un piatto reale non può avere, deve valere minimo 10 centesimi,
            // quindi mi manda in errore quando provo a utilizzare la POST PUT o DELETE senza avere selezionato niente,
            // errore che poi verrà gestito nel catch dei rispettivi metodi
            if (piatto.Prezzo == 0)
            {
                throw new Exception("INSERIRE UN PIATTO!");
            }
        }

        private async Task AddPiatto()
        {
            try
            {
                VerificaPiatto(SelectedPiatto);

                // chiamo l' API per inserire il piatto nel DB
                ApiServiceResult<int> createApiResult = await ApiService.CreatePiatto(SelectedPiatto, _token);
                if (!_token.Roles.Any())
                {
                    MessageBox.Show($"Non hai i permessi di admin per poter eseguire la creazione", "Errore", MessageBoxButton.OK, MessageBoxImage.Warning);
                    await LoadPiatti();
                    return;
                }

                if (createApiResult.Data == null)
                {
                    MessageBox.Show($"Errore nell'aggiunta del piatto: {createApiResult.ErrorMessage}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // Aggiorno la view solo in caso di successo (altrimenti sarei finito nel return di prima)
                MessageBox.Show("Piatto creato con successo!", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadPiatti();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nell'aggiunta del piatto: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeletePiatto()
        {

            // questa era la gestione vecchia, ora il SelectedPiatto non potrà mai essere null perchè viene creato un oggetto vuoto sia dal costruttore che ogni volta che viene chiamato il LoadPiatto();
            //if (SelectedPiatto == null)
            //{
            //    MessageBox.Show("Seleziona un piatto da eliminare", "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            try
            {
                VerificaPiatto(SelectedPiatto);
                var deleteApiResult = await ApiService.DeletePiatto(SelectedPiatto.Id, _token);
                if (!_token.Roles.Any())
                {
                    MessageBox.Show($"Non hai i permessi di admin per poter eseguire l' eliminazione", "Errore", MessageBoxButton.OK, MessageBoxImage.Warning);
                    await LoadPiatti();
                    return;
                }
                if (deleteApiResult.Data == 0)
                {
                    MessageBox.Show($"Errore nell'eliminazione del piatto: {deleteApiResult.ErrorMessage}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBox.Show("Piatto eliminato con successo!", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadPiatti(); // Aggiorno il view model solo in caso di successo
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nell'eliminazione del piatto: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdatePiatto()
        {

            // questa era la gestione vecchia, ora il SelectedPiatto non potrà mai essere null perchè viene creato un oggetto vuoto sia dal costruttore che ogni volta che viene chiamato il LoadPiatto();
            //if (SelectedPiatto == null)
            //{
            //    MessageBox.Show("Seleziona un piatto da modificare", "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            try
            {
                VerificaPiatto(SelectedPiatto);
                var updateApiResult = await ApiService.UpdatePiatto(SelectedPiatto, _token);
                if (!_token.Roles.Any())
                {
                    MessageBox.Show($"Non hai i permessi di admin per poter eseguire l' update", "Errore", MessageBoxButton.OK, MessageBoxImage.Warning);
                    await LoadPiatti();
                    return;
                }
                if (updateApiResult.Data == 0)
                {
                    MessageBox.Show($"Errore nella modifica del piatto: {updateApiResult.ErrorMessage}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBox.Show("Piatto aggiornato con successo!", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadPiatti(); // Aggiorno il view model solo in caso di successo

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nella modifica del piatto: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}