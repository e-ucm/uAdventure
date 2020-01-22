# AssetManager
RAGE Asset Manager.

Note: 
      The IWebServicerequest2 branch has been merged onto the main branch.
      
      Differences are that the old IWebServiceRequest interface is renamed to IWebServiceRequestAsync
      and the new IWebServiceRequest2 interface to IWebServiceRequest.
      
      This new interface, although coded synchronous, but off-course allows game programmers 
      to call asset asset menthods using it asynchronously. This also allows usage with 
      both co-routines/yield() as well as async/await.
      
      The new interface also allows for non-standard verbs and can now be used easily for code like
      if (HealthCheck() && Login()) { <Communicate with the server> }

Note: 
      The repository was recently rebuild, so please clone it again.
