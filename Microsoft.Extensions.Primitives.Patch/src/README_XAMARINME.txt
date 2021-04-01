Microsoft.Net.Http.Headers v2.2.0 from AspNetCore assembly references 'InplaceStringBuilder' from 
Microsoft.Extensions.Primitives assembly to format date and time. 
Unfortunately 'InplaceStringBuilder' has been removed from Microsoft.Extensions.Primitives in v5.0. Hence
the call ends up with run time error.
This patch uses source code of v5.0 of the assemble and adds 'InplaceStringBuilder' file.
Version number of the patch assembly is set to v5.9 so that it overrides the original DLL which has v5.0. 