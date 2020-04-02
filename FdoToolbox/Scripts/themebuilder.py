"""
Copyright (C) 2009, Jackie Ng
https://github.com/jumpinjackie/fdotoolbox, jumpinjackie@gmail.com

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

See license.txt for more/additional licensing information
"""

# themebuilder.py
#
# FDO Toolbox script to create a themed bulk copy operations. 
#
# For example, suppose we want to create a themed data set that based on the 
# attribute RTYPE (values: RES, COM, IND) this script will create a bulk copy
# operation with the following tasks:
#
# - Copy matching features to [FeatureClass]_RES where RTYPE = 'RES'
# - Copy matching features to [FeatureClass]_COM where RTYPE = 'COM'
# - Copy matching features to [FeatureClass]_IND where RTYPE = 'IND'
#
# Author: Jackie Ng (jumpinjackie@gmail.com)

import themedialog
from themedialog import ThemeOptionsDialog

def Run():
	try:
		diag = ThemeOptionsDialog(App.ConnectionManager)
		diag.ShowDialog()
	except Exception, ex:
		App.ShowMessage("Error", ex.ToString())
		
Run()