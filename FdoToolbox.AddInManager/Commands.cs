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
using ICSharpCode.Core;

namespace FdoToolbox.AddInManager
{
    public class ShowCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm.ShowForm();
        }
    }

    public class StartupCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ResourceService.RegisterNeutralStrings(Strings.ResourceManager);
        }
    }

    public class AddInManagerAddInStateConditionEvaluator : IConditionEvaluator
    {
        public bool IsValid(object caller, Condition condition)
        {
            string states = condition.Properties["states"];
            string action = ((AddInControl)caller).AddIn.Action.ToString();
            foreach (string state in states.Split(','))
            {
                if (state == action)
                    return true;
            }
            return false;
        }
    }

    public class DisableCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm.Instance.TryRunAction(((AddInControl)Owner).AddIn, AddInAction.Disable);
        }
    }

    public class EnableCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm.Instance.TryRunAction(((AddInControl)Owner).AddIn, AddInAction.Enable);
        }
    }

    public class AbortInstallCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm.Instance.TryRunAction(((AddInControl)Owner).AddIn, AddInAction.Uninstall);
        }
    }

    public class AbortUpdateCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm.Instance.TryRunAction(((AddInControl)Owner).AddIn, AddInAction.InstalledTwice);
        }
    }

    public class UninstallCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm.Instance.TryUninstall(((AddInControl)Owner).AddIn);
        }
    }

    public class OpenHomepageCommand : AbstractMenuCommand
    {
        public override bool IsEnabled => ((AddInControl)Owner).AddIn.Properties["url"].Length > 0;

        public override void Run()
        {
			try {
				System.Diagnostics.Process.Start(((AddInControl)Owner).AddIn.Properties["url"]);
			} catch {}
            ManagerForm.Instance.Close();
        }
    }
}
