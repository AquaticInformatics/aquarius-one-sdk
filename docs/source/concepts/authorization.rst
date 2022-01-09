Authorization: Roles & Resources
====

Authorization of an API is based on four criteria:
   #. **Authenticated**: Does the API require the user to be authenticated? 
   #. **Role(s)**: Does the user accessing the API have to belong to any specific roles?
   #. **Resources**:  Does the user need to have "rights" to the resources being accessed?
   #. **Features**:  Does the user need to have the features (derived from the Product Offerings they own) to be able to use the API?


Authenticated
----

Authenticated simply means that the user has successfully logged in.  Most APIs require the user to be logged in, while a minority may be accessible to the public.

Roles (Roles-based access control)
----


Users may have one or more of the following roles:
  -  **Basic**: The user can perform typical functions such as data entry, but cannot configure the application or change settings.
  -  **Admin**: The user can configure the application and its resources such as users.
  -  **Support Basic**: (Deprecated) The user can perform elevated tasks.
  -  **Support Admin**: The user can perform elevated tasks required to administrate the system across tenants / customers.
  -  **Super Admin**: The user can perform all functions and by-pass resource authorization restrications.  This role is only granted to personnel responsible for administring the platform.

.. Note::
   - A user must belong to at least one role
   - A user can belong to multiple roles

Resources (Resource-based access control)
----
The authority to use an API is also based on your "rights" to the entity that you are referencing.  
For examply you can ask for a list of tenants (customers) or locations (digital twins), but will only return those in which you have rights to see.


Features (Feature-based access control)
----
In addition to knowing whether the user is authenticated, belongs to a specific role and has access to the resource, 
the user may also need to have a feature to be authorized for the API.

An example of this is that the user may need to have an SPREADSHEET feature in order to access some of the spreadsheet capabilities.  

.. Note::
   Typically, features are reserved for client-side authorization, not server side.  
   A list of features that the user has can be retrieved and the capabilites can be retricted in the application.




.. autosummary::
   :toctree: generated
  
