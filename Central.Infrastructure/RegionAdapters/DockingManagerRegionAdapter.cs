﻿//New BSD License (BSD)
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
using System.ComponentModel.Composition;
using AvalonDock;
using Central.Infrastructure.Behaviors;
using Microsoft.Practices.Prism.Regions;

namespace Central.Infrastructure.RegionAdapters
{
  [Export(typeof(DockingManagerRegionAdapter))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class DockingManagerRegionAdapter : RegionAdapterBase<DockingManager>
  {
    [ImportingConstructor]
    public DockingManagerRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
      : base(regionBehaviorFactory)
    {
    }

    protected override void Adapt(IRegion region, DockingManager regionTarget)
    {
    }

    protected override IRegion CreateRegion()
    {
      return new Region();
    }

    protected override void AttachBehaviors(IRegion region, DockingManager regionTarget)
    {
      if (region == null)
        throw new System.ArgumentNullException("region");

      // Add the behavior that syncs the items source items with the rest of the items
      region.Behaviors.Add(DockingManagerDocumentsSourceSyncBehavior.BehaviorKey, new DockingManagerDocumentsSourceSyncBehavior
      {
        HostControl = regionTarget
      });

      base.AttachBehaviors(region, regionTarget);
    }
  }
}