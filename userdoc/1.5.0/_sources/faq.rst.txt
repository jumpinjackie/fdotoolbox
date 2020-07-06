.. index::
   single: FAQ

Frequently Asked Questions (FAQ)
================================

**Oracle provider do not work**

You must install the Oracle 11g Instant Client and make sure its dlls are either in the same path as the King Oracle provider dll or the instant client libraries are in a path that is part of the PATH environment variable

**How do I supply the ConnectionString parameter for the ODBC Provider?**

You must surround the value in double quotes

**Where are my preferences stored at?**

The user preferences are stored at "%APPDATA%\\FDO Toolbox" (eg. C:\\Documents and Settings\\YOURUSERNAME\\Application Data\\FDO Toolbox)

**Why are some of my commands disabled?**

FDO Toolbox uses the FDO capability API to disable certain commands if the provider in context 
does not support such action. For example, if a provider does not support the **IApplySchema** FDO command, then
if you are editing a Feature Schema on a connection to that provider, the **Apply Schema** button is disabled.

**Does FDO Toolbox support class inheritance?**

Not at this point in time

**Does FDO Toolbox support creation of Schema Mappings?**

Not at this point in time

**Does FDO Toolbox support Long Transactions?**

Not at this point in time

**I get random crashes when using the Generic connection dialog**

Some providers have buggy reference counting which the managed API does not know about. The best way to 
avoid this is to use the specialized method in the Express Add-In. The Generic Dialog is designed as a fallback mechanism and 
should only be used if there is no Express method of connecting to your desired data source.

**My Bulk Copy (with transformation) fails. The logs say: Failed to retrieve message for "MgCoordinateSystemInitializationFailedException"**

Chances are very high that you are probably transforming data from one coordinate system to another which requires a country-specifc grid file that is not shipped by default in the FDO Toolbox installer (for installer size reasons).

Download the Dictionaries_CountryGrids.7z file from the releases page and extract the contents into the `Dictionaries` directory of your FDO Toolbox installation and retry again.