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
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;
using AvalonDock;
using AvalonDock.Layout;
using Central.Infrastructure.RegionAdapters;
using Central.Utilities;
using Central.Views;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Central
{
  internal class Bootstrapper : MefBootstrapper
  {
    private const string DirectoryModulePath = @".\Modules";

    protected override ILoggerFacade CreateLogger()
    {
      return new Log4NetLogger();
    }
    protected override IModuleCatalog CreateModuleCatalog()
    {
      var moduleCatalog = new DirectoryModuleCatalog { ModulePath = DirectoryModulePath };
      return moduleCatalog;
    }

    protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
    {
      base.ConfigureRegionAdapterMappings();

      var regionAdapterMappings = ServiceLocator.Current.GetInstance<RegionAdapterMappings>();
      if (regionAdapterMappings != null)
      {
        regionAdapterMappings.RegisterMapping(typeof(DockingManager),
          ServiceLocator.Current.GetInstance<DockingManagerRegionAdapter>());
        regionAdapterMappings.RegisterMapping(typeof(LayoutAnchorable),
        ServiceLocator.Current.GetInstance<AnchorableRegionAdapter>());
        regionAdapterMappings.RegisterMapping(typeof(LayoutDocument),
       ServiceLocator.Current.GetInstance<DocumentRegionAdapter>());
      }

      return regionAdapterMappings;
    }
   
    protected override void ConfigureAggregateCatalog()
    {
      AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
      AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(DockingManagerRegionAdapter).Assembly));
      if (!Directory.Exists(DirectoryModulePath))
      {
        DirectoryInfo di = Directory.CreateDirectory(DirectoryModulePath);
      }
      var catalog = new DirectoryCatalog(DirectoryModulePath);
      AggregateCatalog.Catalogs.Add(catalog);

    }
    protected override DependencyObject CreateShell()
    {
      return Container.GetExportedValue<ShellWindow>();
    }
    protected override void InitializeShell()
    {
      base.InitializeShell();
      Application.Current.MainWindow = (ShellWindow)Shell;
      Application.Current.MainWindow.Show();
    }
  }
}