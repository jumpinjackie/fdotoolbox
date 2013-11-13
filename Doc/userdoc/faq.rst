.. index::
   single: FAQ

Frequently Asked Questions (FAQ)
================================

**The RDBMS providers (MySQL, PostGIS, Oracle) do not work**

These providers have additional dependencies which do not come with the standard FDO distribution and must be downloaded separately.

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