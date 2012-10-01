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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Central.Infrastructure.Commands;
using Central.Infrastructure.ViewModels;
using Microsoft.Win32;

namespace Central.Infrastructure.AvalonDock.ViewModels
{
  public class Workspace : ViewModelBase
  {
    protected Workspace()
    {

    }
    static readonly Workspace _this = new Workspace();

    public static Workspace This
    {
      get { return _this; }
    }

    public string UserName
    {
      get { return Environment.UserName; }
    }

    ObservableCollection<DocumentViewModel> _documents = new ObservableCollection<DocumentViewModel>();
    ReadOnlyObservableCollection<DocumentViewModel> _readonyDocuments;
    public ReadOnlyObservableCollection<DocumentViewModel> Documents
    {
      get { return _readonyDocuments 
                    ?? (_readonyDocuments = new ReadOnlyObservableCollection<DocumentViewModel>(_documents)); }
    }

    ObservableCollection<AnchorableViewModel> _tools  = new ObservableCollection<AnchorableViewModel>();
    private ReadOnlyObservableCollection<AnchorableViewModel> _readOnlyTools;
    public ReadOnlyObservableCollection<AnchorableViewModel> Tools
    {
      get
      {
        return _readOnlyTools
               ?? (_readOnlyTools = new ReadOnlyObservableCollection<AnchorableViewModel>(_tools));
      }
  
    }

    #region OpenCommand
    RelayCommand _openDocumentCommand = null;
    public ICommand OpenCommand
    {
      get { return _openDocumentCommand 
            ?? (_openDocumentCommand = new RelayCommand(OnOpenDocument, CanOpenDocument)); }
    }
    private bool CanOpenDocument(object parameter)
    {
      return true;
    }
    private void OnOpenDocument(object parameter)
    {
      var dlg = new OpenFileDialog();
      if (dlg.ShowDialog().GetValueOrDefault())
      {
        var fileViewModel = Open(dlg.FileName);
        ActiveDocument = fileViewModel;
      }
    }

    public DocumentViewModel Open(string filepath)
    {
      var fileViewModel = _documents.FirstOrDefault(fm => fm.FilePath == filepath);
      if (fileViewModel != null)
        return fileViewModel;

      fileViewModel = new DocumentViewModel(filepath);
      _documents.Add(fileViewModel);
      return fileViewModel;
    }

    #endregion

    #region NewDocumentCommand
    RelayCommand _newDocumentCommand = null;
    public ICommand NewDocumentCommand
    {
      get { return _newDocumentCommand ?? (_newDocumentCommand = new RelayCommand(OnNewDocument, CanNewDocument)); }
    }

    private bool CanNewDocument(object parameter)
    {
      return true;
    }

    private void OnNewDocument(object parameter)
    {
      var filePath = parameter as string;
      if (filePath != null)
      {
        _documents.Add(new DocumentViewModel(filePath));
        ActiveDocument = _documents.Last();
      }
      //else
      //{
      //  _documents.Add(new DocumentViewModel());
      //}
    }

    #endregion

    #region ActiveDocument

    private DocumentViewModel _activeDocument = null;
    public DocumentViewModel ActiveDocument
    {
      get { return _activeDocument; }
      set
      {
        if (_activeDocument != value)
        {
          _activeDocument = value;
          RaisePropertyChanged("ActiveDocument");
          if (ActiveDocumentChanged != null)
            ActiveDocumentChanged(this, EventArgs.Empty);
        }
      }
    }

    public event EventHandler ActiveDocumentChanged;

    #endregion

    #region NewToolCommand
    RelayCommand _newToolCommand = null;
    public ICommand NewToolCommand
    {
      get { return _newToolCommand ?? (_newToolCommand = new RelayCommand(OnNewTool, CanNewTool)); }
    }

    private bool CanNewTool(object parameter)
    {
      return true;
    }

    private void OnNewTool(object parameter)
    {
      _tools.Add(new AnchorableViewModel());
      ActiveToolWindow = _tools.Last();
    }

    #endregion
    #region ActiveToolWindow

    private AnchorableViewModel _activeToolWindow;
    public AnchorableViewModel ActiveToolWindow
    {
      get { return _activeToolWindow; }
      set
      {
        if (_activeToolWindow != value)
        {
          _activeToolWindow = value;
          RaisePropertyChanged("ActiveToolWindow");
          if (ActiveToolWindowChanged != null)
            ActiveToolWindowChanged(this, EventArgs.Empty);

        }
      }
    }
    public event EventHandler ActiveToolWindowChanged;
    #endregion

    internal void Close(DocumentViewModel fileToClose)
    {
      if (fileToClose.IsDirty)
      {
        var res = MessageBox.Show(string.Format("Save changes for file '{0}'?", fileToClose.FileName), "AvalonDock Test App", MessageBoxButton.YesNoCancel);
        if (res == MessageBoxResult.Cancel)
          return;
        if (res == MessageBoxResult.Yes)
        {
          Save(fileToClose);
        }
      }

      _documents.Remove(fileToClose);
    }

    internal void Save(DocumentViewModel fileToSave, bool saveAsFlag = false)
    {
      if (fileToSave.FilePath == null || saveAsFlag)
      {
        var dlg = new SaveFileDialog();
        if (dlg.ShowDialog().GetValueOrDefault())
          fileToSave.FilePath = dlg.SafeFileName;
      }
      
      if (fileToSave.FilePath != null) 
        File.WriteAllText(fileToSave.FilePath, fileToSave.TextContent);
      ActiveDocument.IsDirty = false;
    }



  }
}
