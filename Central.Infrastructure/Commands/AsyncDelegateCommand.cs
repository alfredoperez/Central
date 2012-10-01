//New BSD License (BSD)
//Copyright ©2012, Alfredo Perez
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the
//following conditions are met:

//* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

//* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following
//disclaimer in the documentation and/or other materials provided with the distribution.

//* Neither the name of Alfredo Perez nor the names of its contributors may be used to endorse or promote products
//derived from this software without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
//IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
//STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
//EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Central.Infrastructure.Commands
{
  public class AsyncDelegateCommand : ICommand
  {
    readonly BackgroundWorker _worker=new BackgroundWorker();
    readonly Func<bool> _canExecute;  
  
    /// <summary>  
    /// The constructor  
    /// </summary>  
    /// <param name="action">The action to be executed</param>  
    /// <param name="canExecute">Will be used to determine if the action can be executed</param>  
    /// <param name="completed">Will be invoked when the action is completed</param>  
    /// <param name="error">Will be invoked if the action throws an error</param>  
    public AsyncDelegateCommand(Action action,   
                                Func<bool> canExecute=null,  
                                Action<object> completed =null,  
                                Action<Exception> error=null  
                                )  
    {  
  
        _worker.DoWork += (s, e) =>  
            {  
                CommandManager.InvalidateRequerySuggested();  
                action();  
            };  
  
        _worker.RunWorkerCompleted += (s, e) =>  
            {  
  
                if (completed != null && e.Error == null)  
                    completed(e.Result);  
  
                if (error != null && e.Error != null)  
                    error(e.Error);  
  
                CommandManager.InvalidateRequerySuggested();  
            };  
  
        _canExecute = canExecute;  
    }  
  
  
    /// <summary>  
    /// To cancel an ongoing execution  
    /// </summary>  
    public void Cancel()  
    {  
        if (_worker.IsBusy)  
            _worker.CancelAsync();   
    }  
  
    /// <summary>  
    /// Note that this will return false if the worker is already busy  
    /// </summary>  
    /// <param name="parameter"></param>  
    /// <returns></returns>  
    public bool CanExecute(object parameter)  
    {  
        return (_canExecute == null) ?   
                !(_worker.IsBusy) : !(_worker.IsBusy)   
                    && _canExecute();      
    }  
  
    /// <summary>  
    /// Let us use command manager for thread safety  
    /// </summary>  
    public event EventHandler CanExecuteChanged  
    {  
        add { CommandManager.RequerySuggested += value; }  
        remove { CommandManager.RequerySuggested -= value; }  
    }  
  
    /// <summary>  
    /// Here we'll invoke the background worker  
    /// </summary>  
    /// <param name="parameter"></param>  
    public void Execute(object parameter)  
    {  
        _worker.RunWorkerAsync();   
    }  
}  
  
}
