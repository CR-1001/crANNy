/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.Windows.Input;

    [Serializable] public class CanExecuteEventArgs : EventArgs
    {
        public bool IsExecutable { get; set; }
        public Object Item { get; set; }
        public Func<Object, bool> CanExecute { get; set; }
        public Action<Object> Execute { get; set; }
    }


    [Serializable] public class DelegateCommand : ICommand
    {
        private readonly Func<Object, bool> _CanExecute;
        private readonly Action<Object> _ExecuteAction;

        public DelegateCommand(Action<Object> executeAction, Func<Object, bool> canExecute)
        {

            _ExecuteAction = executeAction;
            _CanExecute = canExecute;
        }

        public DelegateCommand(Action<Object> executeAction)
            : this(executeAction, Allow)
        {
        }

        private static bool Allow(Object obj)
        {
            return true;
        }

        public bool CanExecute(Object parameter)
        {
            return CanExecute(parameter, false);
        }

        public bool CanExecute(Object parameter, bool raiseCanExecuteChanged)
        {
            bool canExecutex = _CanExecute(parameter);

            if (raiseCanExecuteChanged && CanExecuteChanged != null)
            {
                CanExecuteChanged(
                    this,
                    new CanExecuteEventArgs()
                    {
                        CanExecute = _CanExecute,
                        Execute = _ExecuteAction,
                        Item = parameter,
                        IsExecutable = canExecutex
                    });
            }

            return canExecutex;
        }

        public event EventHandler CanExecuteChanged;

        public virtual void Execute(Object parameter)
        {
            _ExecuteAction(parameter);
        }

    }


    [Serializable] public class DelegateCommandBlockable : DelegateCommand
    {
        public static bool IsBlocked { get; set; }


        public DelegateCommandBlockable(Action<Object> executeAction, Func<Object, bool> canExecute)
            : base(executeAction, canExecute) { }

        public DelegateCommandBlockable(Action<Object> executeAction)
            : base(executeAction) { }

        public override void Execute(Object parameter)
        {
            if (IsBlocked)
            {
                return;
            }

            base.Execute(parameter);
        }

    }
}


