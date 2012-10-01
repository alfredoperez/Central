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
using System.ComponentModel.Composition;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Regions.Behaviors;
using Microsoft.Practices.ServiceLocation;

namespace Central.Infrastructure.RegionAdapters
{
  [Export(typeof(AvalonDockRegion))]
  public class AvalonDockRegion : DependencyObject
  {
    #region Name

    /// <summary>
    /// Name Attached Dependency Property
    /// </summary>
    public static readonly DependencyProperty NameProperty =
        DependencyProperty.RegisterAttached("Name", typeof(string), typeof(AvalonDockRegion),
            new FrameworkPropertyMetadata((string)null,
                new PropertyChangedCallback(OnNameChanged)));

    /// <summary>
    /// Gets the Name property.  This dependency property
    /// indicates the region name of the layout item.
    /// </summary>
    public static string GetName(DependencyObject d)
    {
      return (string)d.GetValue(NameProperty);
    }

    /// <summary>
    /// Sets the Name property.  This dependency property
    /// indicates the region name of the layout item.
    /// </summary>
    public static void SetName(DependencyObject d, string value)
    {
      d.SetValue(NameProperty, value);
    }

    /// <summary>
    /// Handles changes to the Name property.
    /// </summary>
    private static void OnNameChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
    {
      CreateRegion(s, (string)e.NewValue);
    }

    #endregion Name

    private static void CreateRegion(object element, string regionName)
    {
      if (element == null)
        throw new ArgumentNullException("element");

      //If I'm in design mode the main window is not set
      if (Application.Current == null ||
          Application.Current.MainWindow == null)
        return;

      try
      {
        if (ServiceLocator.Current == null)
          return;

        // Build the region
        var mappings = ServiceLocator.Current.GetInstance<RegionAdapterMappings>();
        if (mappings == null)
          return;
        IRegionAdapter regionAdapter = mappings.GetMapping(element.GetType());
        if (regionAdapter == null)
          return;

        regionAdapter.Initialize(element, regionName);
      }
      catch (Exception ex)
      {
        throw new RegionCreationException(string.Format("Unable to create region {0}", regionName), ex);
      }
    }
  }
}