using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ristorante_frontend.ViewModels
{
    public class MyCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public MyCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    var task = _execute();
                    task.ContinueWith(t =>
                    {
                        if (t.IsFaulted && t.Exception != null)
                        {
                            Console.WriteLine($"Errore comando: {t.Exception}");
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore comando: {ex}");
                }
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}