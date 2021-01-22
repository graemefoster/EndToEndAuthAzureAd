# End to End OAuth options

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


## Option 2.5 (on behalf of with nicer user experience)
With option 2 the middle tier has an implicit ability to use scopes on-behalf-of the user. But the consent experience doesn't convey this. We can make some tweaks in Azure AD to make this nicer.
- Add the Application Id of the client into the KnownApplications list on the middle tier API
- Add an explicit entry for the scope defined on the middle tier to the client
- Change the client to request a scope called '.default' from the middle tier.

The .default scope in conjunction with the KnownApplication entry causes Azure AD to show all the scopes the middle tier requires in the initial consent flow.

## Option 3 (Client Credentials)
In Option 3 the middle tier uses its own credentials to get a token allowing it to call the backend API.
The cons to this are:
 - The backend API receives a token which doesn't contain end user information
 - The middle tier API has the ability to call the backend for _any_ user, even if no user is using the application.

For these reasons I've not added a sample project for this flow.


## Setup

There are 4 AAD applications required. You'll need to update the settings files in each project to reflect the Applications in your Active Directory. You'll also need to get the ClientSecrets and set them as user-secrets to run locally.

### Back End API
| Property | Setting | Notes |
| --- | --- | --- |
| Exposed APIs | Orders |
| App Roles | BackOffice | Can use this to ensure that the user calling is in the correct role in AAD

### Middle-tier API
| Property | Setting | Notes |
| --- | --- | --- |
| API Permissions | Orders 'exposed API' from the back end API |
| Exposed APIs | on-behalf-of | Requested for the on behalf of flow
| Direct entry in manifest file | knownClientApplications | Add the Application ID of the ClientWebApp-OnBehalfOf |

### Client Web App App
No apis exposed / roles / permissions for this one

### Client Web App (On Behalf Of) App
| Property | Setting | Notes |
| --- | --- | --- |
| API Permissions | on-behalf-of 'exposed API' from the Middle-tier API |


For a user to successfully authenticate and call the API you will need to add them into the BackOffice role in Azure Active Directory. 
A scope doesn't mean that a user has the permission to do something. It means that someone consented for an API to do something 'as them'. They may not have permission to do that thing.

Think of a hospital. Maybe a client app has some functionality to call an api that can administer treatments. The end user that consents for that API to be consumed needs to be someone that can do this. Scopes carve an API surface area up into chunks, and are mainly used for a end user consent experience. Roles, permissions and policy are still the vehicle for what a user can do.

You can also stop users being able to get a token for an application regardless of permissions / roles. To do this you can find the application in Azure AD, click through to the Managed Application, and check the 'User Assignment Required' on the Properties tab.

More good info here: https://auth0.com/blog/on-the-nature-of-oauth2-scopes/
