
## Introduction ##
O2P is a puplic  wrapper class that expose all internal services and simplify API calls of OData2Poco library

## OData2Poco V 2.0.0##
**Example 1:**

     var url = "http://services.odata.org/V4/OData/OData.svc";
     var o2p = new O2P();
     var code = await o2p.GenerateAsync(new Uri(url));
     Debug.WriteLine(code);       
     Debug.WriteLine(o2p.MetaDataVersion);
   

**Example 2: generating attributes, nullable data type**

       PocoSetting setting = new PocoSetting
   		 {
		    AddNullableDataType = true,
		    AddKeyAttribute = true,
		    AddTableAttribute = true,
		    AddRequiredAttribute = true,
		    AddNavigation = true,
		    AddPartial = true
  		  }; 
    var url = "http://services.odata.org/V4/OData/OData.svc";
     var o2p = new O2P(setting);
     var code = await o2p.GenerateAsync(new Uri(url));
     Debug.WriteLine(code);   
     Debug.WriteLine(o2p.MetaDataVersion);
   
## OData2Poco V1.3.0##

using wrapper class O2P

Example 1:
Simply, one line of code and you get CS code

     var code = new O2P(url).Generate();
 
*Example 2:*

 for basic authentication

     var code = new O2P(url,user,password).Generate();



