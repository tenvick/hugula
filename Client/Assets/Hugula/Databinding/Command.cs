using System;
namespace Hugula.Databinding {

    ///<summary>
    /// ICommand的默认实现
    ///</summary>
    public sealed class Command<T> : Command {
        public Command (Action<T> execute) : base (o => {
            if (IsValidParameter (o)) {
                execute ((T) o);
            }
        }) {
            if (execute == null) {
                throw new ArgumentNullException (nameof (execute));
            }
        }

        public Command (Action<T> execute, Func<T, bool> canExecute) : base (o => {
            if (IsValidParameter (o)) {
                execute ((T) o);
            }
        }, o => IsValidParameter (o) && canExecute ((T) o)) {
            if (execute == null)
                throw new ArgumentNullException (nameof (execute));
            if (canExecute == null)
                throw new ArgumentNullException (nameof (canExecute));
        }

        static bool IsValidParameter (object o) {
            if (o != null) {
                return o is T;
            }

            var t = typeof (T);

            if (Nullable.GetUnderlyingType (t) != null) {
                return true;
            }

            return !t.GetType ().IsValueType;
        }
    }

    public class Command : ICommand {
        readonly Func<object, bool> _canExecute;
        readonly Action<object> _execute;
        readonly WeakReference _weakEventManager;

        public Command (Action<object> execute) {
            if (execute == null)
                throw new ArgumentNullException (nameof (execute));

            _execute = execute;
        }

        public Command (Action execute) : this (o => execute ()) {
            if (execute == null)
                throw new ArgumentNullException (nameof (execute));
        }

        public Command (Action<object> execute, Func<object, bool> canExecute) : this (execute) {
            if (canExecute == null)
                throw new ArgumentNullException (nameof (canExecute));

            _canExecute = canExecute;
        }

        public Command (Action execute, Func<bool> canExecute) : this (o => execute (), o => canExecute ()) {
            if (execute == null)
                throw new ArgumentNullException (nameof (execute));
            if (canExecute == null)
                throw new ArgumentNullException (nameof (canExecute));
        }

        public bool CanExecute (object parameter) {
            if (_canExecute != null)
                return _canExecute (parameter);

            return true;
        }

        public void Execute (object parameter) {
            _execute (parameter);
        }

    }

}