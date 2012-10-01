using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Input;
using AvalonDock.Layout.Serialization;
using Central.Infrastructure.AvalonDock.ViewModels;
using Central.Infrastructure.Commands;
using Elysium.Theme;
using Microsoft.Practices.Prism.Events;

namespace Central.Views
{
  [Export]
  public partial class ShellWindow
  {
    
   
    public ShellWindow()
    {
      InitializeComponent();
      ThemeManager.Instance.Light(ThemeManager.Instance.Accent);
      DataContext = Workspace.This;

    }

  
    #region LoadLayoutCommand
    RelayCommand _loadLayoutCommand = null;
    public ICommand LoadLayoutCommand
    {
      get { return _loadLayoutCommand ?? (_loadLayoutCommand = new RelayCommand(OnLoadLayout, CanLoadLayout)); }
    }

    private bool CanLoadLayout(object parameter)
    {
      return File.Exists(@".\AvalonDock.Layout.config");
    }

    private void OnLoadLayout(object parameter)
    {
      var layoutSerializer = new XmlLayoutSerializer(dockManager);
      layoutSerializer.LayoutSerializationCallback += (s, e) =>
      {
        if (e.Model.ContentId == AnchorableViewModel.ToolContentId)
          e.Content = Workspace.This.Tools;
        else if (!string.IsNullOrWhiteSpace(e.Model.ContentId) &&
            File.Exists(e.Model.ContentId))
          e.Content = Workspace.This.Open(e.Model.ContentId);
      };
      layoutSerializer.Deserialize(@".\AvalonDock.Layout.config");
    }

    #endregion

    #region SaveLayoutCommand
    RelayCommand _saveLayoutCommand;
    public ICommand SaveLayoutCommand
    {
      get {
        return _saveLayoutCommand ??
               (_saveLayoutCommand = new RelayCommand(OnSaveLayout, CanSaveLayout));
      }
    }

    private bool CanSaveLayout(object parameter)
    {
      return true;
    }

    private void OnSaveLayout(object parameter)
    {
      var layoutSerializer = new XmlLayoutSerializer(dockManager);
      layoutSerializer.Serialize(@".\AvalonDock.Layout.config");
    }

    #endregion 

  }
}