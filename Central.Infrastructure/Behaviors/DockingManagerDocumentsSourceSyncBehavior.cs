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
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using AvalonDock;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Regions.Behaviors;

namespace Central.Infrastructure.Behaviors
{
  [Export(typeof(DockingManagerDocumentsSourceSyncBehavior))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  internal class DockingManagerDocumentsSourceSyncBehavior : RegionBehavior, IHostAwareRegionBehavior
  {
    public static readonly string BehaviorKey = "DockingManagerDocumentsSourceSyncBehavior";
    private bool _updatingActiveViewsInManagerActiveContentChanged;
    private DockingManager _dockingManager;

    public DependencyObject HostControl
    {
      get
      {
        return _dockingManager;
      }

      set
      {
        _dockingManager = value as DockingManager;
      }
    }

    private ObservableCollection<object> _documents = new ObservableCollection<object>();
    private ReadOnlyObservableCollection<object> _readonlyDocumentsList;

    public ReadOnlyObservableCollection<object> Documents
    {
      get { return _readonlyDocumentsList ?? (_readonlyDocumentsList = new ReadOnlyObservableCollection<object>(_documents)); }
    }

    /// <summary>
    /// Starts to monitor the <see cref="IRegion"/> to keep it in synch with the items of the <see cref="HostControl"/>.
    /// </summary>
    protected override void OnAttach()
    {
      var itemsSourceIsSet = _dockingManager.DocumentsSource != null;

      if (itemsSourceIsSet)
      {
        throw new InvalidOperationException();
      }

      SynchronizeItems();

      _dockingManager.ActiveContentChanged += ManagerActiveContentChanged;
      Region.ActiveViews.CollectionChanged += ActiveViewsCollectionChanged;
      Region.Views.CollectionChanged += ViewsCollectionChanged;
    }

    private void ViewsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          {
            var startIndex = e.NewStartingIndex;
            foreach (var newItem in e.NewItems)
            {
              _documents.Insert(startIndex++, newItem);
            }
          }
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (var oldItem in e.OldItems)
          {
            _documents.Remove(oldItem);
          }
          break;
      }
    }

    private void SynchronizeItems()
    {
      BindingOperations.SetBinding(
          _dockingManager,
          DockingManager.DocumentsSourceProperty,
          new Binding("Documents") { Source = this });

      foreach (object view in Region.Views)
      {
        _documents.Add(view);
      }
    }

    private void ActiveViewsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (_updatingActiveViewsInManagerActiveContentChanged)
      {
        return;
      }

      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        if (_dockingManager.ActiveContent != null
            && _dockingManager.ActiveContent != e.NewItems[0]
            && Region.ActiveViews.Contains(_dockingManager.ActiveContent))
        {
          Region.Deactivate(_dockingManager.ActiveContent);
        }

        _dockingManager.ActiveContent = e.NewItems[0];
      }
      else if (e.Action == NotifyCollectionChangedAction.Remove &&
               e.OldItems.Contains(_dockingManager.ActiveContent))
      {
        _dockingManager.ActiveContent = null;
      }
    }

    private void ManagerActiveContentChanged(object sender, EventArgs e)
    {
      try
      {
        _updatingActiveViewsInManagerActiveContentChanged = true;

        if (_dockingManager == sender)
        {
          object activeContent = _dockingManager.ActiveContent;
          foreach (var item in Region.ActiveViews.Where(it => it != activeContent))
          {
            Region.Deactivate(item);
          }

          if (Region.Views.Contains(activeContent) && !Region.ActiveViews.Contains(activeContent))
          {
            Region.Activate(activeContent);
          }
        }
      }
      finally
      {
        _updatingActiveViewsInManagerActiveContentChanged = false;
      }
    }
  }
}