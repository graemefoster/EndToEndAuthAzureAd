#End to End OAuth options

This sample solution shows a few different ways you can call a backend API from a client. The scenarios use 
a web-app as a client, but this could easily be a native application as-well.

## Option 1 (Client / middle tier seen as 1 application)
In this scenario we view the client and the middle tier as the same application. This is a suggestion [here](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow) under the heading "Use of a single application".

The client requests a token for the scopes on the backend API. It passes them through the integration tier which can do some basic validation, but really proxies them through the the backend API.


## Option 2 (on behalf of)
For this scenario we see the client and middle tier as separate applications in Azure Active Directory. 
 - The middle tier declares that it wants to delegate permissions for the back-end API
 - The middle tier declare a scope with a name like 'user_impersonation', or 'on_behalf_of'

The client requests a token for the 'user_impersonation' scope declared by the middle tier. When the middle tier receives this is requests another token which uses a grant involving the initial token. This new token is passed to the backend API.

