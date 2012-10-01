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
using System.IO;
using System.Windows.Input;
using Central.Infrastructure.Commands;

namespace Central.Infrastructure.AvalonDock.ViewModels
{
  public class DocumentViewModel : PaneViewModel
  {
    public DocumentViewModel(string filePath)
    {
      FilePath = filePath;
      Title = FileName;
    }

    public DocumentViewModel()
    {
      IsDirty = true;
      Title = FileName;
    }

    #region FilePath

    private string _filePath;

    public string FilePath
    {
      get { return _filePath; }
      set
      {
        if (_filePath != value)
        {
          _filePath = value;
          RaisePropertyChanged("FilePath");
          RaisePropertyChanged("FileName");
          RaisePropertyChanged("Title");

          ContentId = _filePath;
        }
      }
    }

    #endregion FilePath

    public string FileName
    {
      get
      {
        if (FilePath == null)
          return "Noname" + (IsDirty ? "*" : "");

        return Path.GetFileName(FilePath) + (IsDirty ? "*" : "");
      }
    }

    public override Uri IconSource
    {
      get
      {
        return new Uri("/AvalonDock.MVVMTestApp;component/Images/document.png", UriKind.RelativeOrAbsolute);
      }
    }

    #region TextContent

    private string _textContent = string.Empty;

    public string TextContent
    {
      get { return _textContent; }
      set
      {
        if (_textContent != value)
        {
          _textContent = value;
          RaisePropertyChanged("TextContent");
          IsDirty = true;
        }
      }
    }

    #endregion TextContent

    #region IsDirty

    private bool _isDirty;

    public bool IsDirty
    {
      get { return _isDirty; }
      set
      {
        if (_isDirty != value)
        {
          _isDirty = value;
          RaisePropertyChanged("IsDirty");
          RaisePropertyChanged("FileName");
        }
      }
    }

    #endregion IsDirty

    #region SaveCommand

    private RelayCommand _saveCommand;

    public ICommand SaveCommand
    {
      get { return _saveCommand ?? (_saveCommand = new RelayCommand(OnSave, CanSave)); }
    }

    private bool CanSave(object parameter)
    {
      return IsDirty;
    }

    private void OnSave(object parameter)
    {
      Workspace.This.Save(this);
    }

    #endregion SaveCommand

    #region SaveAsCommand

    private RelayCommand _saveAsCommand;

    public ICommand SaveAsCommand
    {
      get { return _saveAsCommand ?? (_saveAsCommand = new RelayCommand(OnSaveAs, CanSaveAs)); }
    }

    private bool CanSaveAs(object parameter)
    {
      return IsDirty;
    }

    private void OnSaveAs(object parameter)
    {
      Workspace.This.Save(this, true);
    }

    #endregion SaveAsCommand

    #region CloseCommand

    private RelayCommand _closeCommand;

    public ICommand CloseCommand
    {
      get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => OnClose(), p => CanClose())); }
    }

    private bool CanClose()
    {
      return true;
    }

    private void OnClose()
    {
      Workspace.This.Close(this);
    }

    #endregion CloseCommand
  }
}