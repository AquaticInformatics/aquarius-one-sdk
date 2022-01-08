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



Resources (Feature-based access control)
----




.. autosummary::
   :toctree: generated
  
