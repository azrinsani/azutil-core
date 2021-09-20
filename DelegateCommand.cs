using System;
using System.Diagnostics;
using System.Windows.Input;

namespace AzUtil.Core
{
    /// <summary>
    /// This class allows delegating the commanding logic to methods passed as parameters,
    /// and enables a View to bind commands to objects that are not part of the element tree.
    /// </summary>
    /// <typeparam name="T">Type of the parameter passed to the delegates</typeparam>
    public class AzCommand<T> : ICommand
    {
        /// <summary>
        /// The execute
        /// </summary>
        private readonly Action<T> _execute;

        /// <summary>
        /// The can execute
        /// </summary>
        private readonly Predicate<T> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}" /> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <exception cref="System.ArgumentNullException">execute</exception>
        public AzCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}" /> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute predicate.</param>
        /// <exception cref="System.ArgumentNullException">execute</exception>

        [DebuggerStepThrough]
        public AzCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");

            if (canExecute != null)
            {
                _canExecute = canExecute;
            }
        }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raise <see cref="RelayCommand{T}.CanExecuteChanged" /> event.
        /// </summary>

        [DebuggerStepThrough]
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Determines whether this instance can execute the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.</returns>

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute.Invoke((T)parameter);
        }

        /// <summary>
        /// Executes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// 
        [DebuggerStepThrough]
        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _execute((T)parameter);
            }
        }
    }




    public class AzCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Action<object> _execute2;
        private readonly Predicate<object> _canExecute;

        public AzCommand(Action<object> execute) : this(execute, null) { }
        public AzCommand(Action execute) : this(execute, null) { }
        public AzCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute2 = execute ?? throw new ArgumentNullException("execute");

            if (canExecute != null)
            {
                _canExecute = canExecute;
            }
        }

        [DebuggerStepThrough]
        public AzCommand(Action execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");

            if (canExecute != null)
            {
                _canExecute = canExecute;
            }
        }
        public event EventHandler CanExecuteChanged;
        [DebuggerStepThrough]
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute.Invoke(parameter);
        }

        [DebuggerStepThrough]
        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                if (parameter == null)
                {
                    _execute();
                }
                else _execute2(parameter);
            }
        }
    }


    //public class AzCommand : ICommand
    //{
    //    private readonly Action<object> _execute;
    //    private readonly Predicate<object> _canExecute;

    //    public AzCommand(Action<object> execute) : this(execute, null) { }
    //    public AzCommand(Action<object> execute, Predicate<object> canExecute)
    //    {
    //        _execute = execute ?? throw new ArgumentNullException("execute");

    //        if (canExecute != null)
    //        {
    //            _canExecute = canExecute;
    //        }
    //    }       
    //    public event EventHandler CanExecuteChanged;
    //    public void RaiseCanExecuteChanged()
    //    {
    //        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    //    }
    //    public bool CanExecute(object parameter)
    //    {
    //        return _canExecute == null || _canExecute.Invoke(parameter);
    //    }
    //    public virtual void Execute(object parameter)
    //    {
    //        if (CanExecute(parameter))
    //        {
    //            _execute(parameter);
    //        }
    //    }
    //}
}
