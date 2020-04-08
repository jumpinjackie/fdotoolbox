#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// https://github.com/jumpinjackie/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;
using System.Collections.ObjectModel;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Default copy spatial contexts command
    /// </summary>
    public class CopySpatialContext : ICopySpatialContext
    {
        protected Dictionary<string, string> _overrideWkts;

        /// <summary>
        /// Determines if the given spatial context in the list of spatial context names
        /// </summary>
        /// <param name="ctx">The spatial context</param>
        /// <param name="names">The spatial context name list</param>
        /// <returns></returns>
        protected bool SpatialContextInSpecifiedList(SpatialContextInfo ctx, string[] names)
        {
            return Array.Exists<string>(names, delegate(string s) { return s == ctx.Name; });
        }

        protected void CreateSpatialContext(FdoFeatureService service, SpatialContextInfo sc, bool overwrite)
        {
            if (_overrideWkts != null && _overrideWkts.TryGetValue(sc.Name, out var ovWkt))
            {
                sc.CoordinateSystemWkt = ovWkt;
            }
            service.CreateSpatialContext(sc, overwrite);
        }

        /// <summary>
        /// Copies all spatial contexts
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts</param>
        public virtual void Execute(FdoConnection source, FdoConnection target, bool overwrite)
        {
            List<string> names = new List<string>();
            using (FdoFeatureService service = source.CreateFeatureService())
            {   
                ReadOnlyCollection<SpatialContextInfo> contexts = service.GetSpatialContexts();
                if (contexts.Count == 0)
                    return;

                foreach (SpatialContextInfo ctx in contexts)
                {
                    names.Add(ctx.Name);
                }
            }
            Execute(source, target, overwrite, names.ToArray());
        }

        /// <summary>
        /// Copies all spatial contexts
        /// </summary>
        /// <param name="spatialContexts">The spatial contexts.</param>
        /// <param name="target">The target.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        public virtual void Execute(ICollection<SpatialContextInfo> spatialContexts, FdoConnection target, bool overwrite)
        {
            bool supportsMultipleScs = target.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts);
            bool supportsDestroySc = target.Capability.HasArrayCapability(CapabilityType.FdoCapabilityType_CommandList, (int)OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext);

            using (FdoFeatureService service = target.CreateFeatureService())
            {
                if (supportsMultipleScs)
                {
                    if (overwrite && supportsDestroySc)
                    {
                        ReadOnlyCollection<SpatialContextInfo> targetContexts = service.GetSpatialContexts();

                        foreach (SpatialContextInfo sc in spatialContexts)
                        {
                            //Only destroy spatial context if it exists in target connection
                            if(SpatialContextExists(targetContexts, sc))
                                service.DestroySpatialContext(sc);
                        }
                    }
                    foreach (SpatialContextInfo sc in spatialContexts)
                    {
                        CreateSpatialContext(service, sc, overwrite);
                    }
                }
                else
                {
                    List<SpatialContextInfo> contexts = new List<SpatialContextInfo>(spatialContexts);
                    //Copy either the active spatial context in the list or the first
                    //spatial context (if no active one is found)
                    SpatialContextInfo active = null;
                    if (contexts.Count > 0)
                    {
                        foreach (SpatialContextInfo sc in contexts)
                        {
                            if (sc.IsActive)
                            {
                                active = sc;
                                break;
                            }
                        }
                        if (active == null)
                            active = contexts[0];
                    }
                    if (active != null)
                        CreateSpatialContext(service, active, overwrite);
                }
            }
        }

        /// <summary>
        /// Determines if a given spatial context exists in the given collection (comparison is by name). 
        /// </summary>
        /// <param name="targetContexts">The target spatial context list</param>
        /// <param name="sc">The spatial context to look for</param>
        /// <returns></returns>
        protected static bool SpatialContextExists(ReadOnlyCollection<SpatialContextInfo> targetContexts, SpatialContextInfo sc)
        {
            bool found = false;
            foreach (SpatialContextInfo tsc in targetContexts)
            {
                if (tsc.Name == sc.Name)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        /// <summary>
        /// Copies the spatial contexts given in the list
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will overwrite any existing spatial contexts</param>
        /// <param name="spatialContextNames">The list of spatial contexts to copy</param>
        public virtual void Execute(FdoConnection source, FdoConnection target, bool overwrite, string[] spatialContextNames)
        {
            if (spatialContextNames.Length == 0)
                return;

            using (FdoFeatureService sService = source.CreateFeatureService())
            using (FdoFeatureService tService = target.CreateFeatureService())
            {
                bool supportsMultipleScs = target.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts);
                bool supportsDestroySc = target.Capability.HasArrayCapability(CapabilityType.FdoCapabilityType_CommandList, (int)OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext);
                if (supportsMultipleScs)
                {
                    //Get all contexts in target
                    ReadOnlyCollection<SpatialContextInfo> contexts = tService.GetSpatialContexts();

                    List<string> updated = new List<string>();
                    //Destroy any clashing ones
                    if (overwrite && supportsDestroySc)
                    {
                        foreach (SpatialContextInfo c in contexts)
                        {
                            if (SpatialContextInSpecifiedList(c, spatialContextNames))
                                tService.DestroySpatialContext(c);
                        }
                    }
                    
                    //Then overwrite them
                    foreach (SpatialContextInfo c in contexts)
                    {
                        if (SpatialContextInSpecifiedList(c, spatialContextNames))
                        {
                            CreateSpatialContext(tService, c, overwrite);
                            updated.Add(c.Name);
                        }
                    }

                    //Now create the rest
                    var sourceScs = sService.GetSpatialContexts();
                    foreach (var sc in sourceScs)
                    {
                        if (!updated.Contains(sc.Name))
                        {
                            CreateSpatialContext(tService, sc, overwrite);
                        }
                    }
                }
                else
                {
                    CreateSpatialContext(tService, sService.GetActiveSpatialContext(), true);
                }
            }
        }

        /// <summary>
        /// Copies the named spatial context
        /// </summary>
        /// <param name="source">The source connection</param>
        /// <param name="target">The target connection</param>
        /// <param name="overwrite">If true will ovewrite the spatial context of the same name on the target connection</param>
        /// <param name="spatialContextName"></param>
        public virtual void Execute(FdoConnection source, FdoConnection target, bool overwrite, string spatialContextName)
        {
            Execute(source, target, overwrite, new string[] { spatialContextName });
        }

        public void SetOverrideWkts(Dictionary<string, string> overrideWkts)
        {
            _overrideWkts = overrideWkts;
        }
    }
}
