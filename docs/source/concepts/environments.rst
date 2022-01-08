Deployment Environments
====

Aquatic informatic utilizes a progressive environment approach to developing software. Code is progressed through environments as the code is matured. Each environment is completely isolated from each other including separate:

   - **Code Deployment**: All Aquatic Information Code is unique per environment
   - **PaaS Services**: The Platform as a Service, services are all completely separately deployed. For example, there are separate file servers, database servers, storage, etc. for each environment.
   - **Data**: The data including users, user accounts, tenants, operations, and related data are completely separate and unique.

Source Code Control
-------------------

Aquatic Informatics leverages GIT as source code control.

GitFlow
-------------------

The Aquatic Informatic developers leverage: `Gitflow Workflow | Atlassian Git Tutorial <https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow>`_

Continuous Integration / Continuous Deployment (CI / CD)
-------------------

Aquatic Informatic fully utilizes automated CI / CD Processes to:

- Compile Data upon submission
- Perform automated code review
- Deploy code to the appropriate environments
- Perform automated tests on code

Environments
-------------------

Feature
^^^^^^^^^^^^^^^^^^^

Purpose: Testing daily work for developers.

The feature environment is the first environment that receives new code. When developers work on an issue a GIT code branch is made. when code is committed, the CI/CD process automatically compiles and deploys to the Feature Environment.

Changes to this environment may occur nearly every hour.

   - **Web URL**: https://feature-us.aquaticinformatics.net
   - **API URL**: https://api-feature-us.aquaticinformatics.net

Integration
^^^^^^^^^^^^^^^^^^^

Purpose: Testing the integration of multiple developers work together.

The integration environment is the first stable environment that receives code. When developers finish work on an issue a pull request is performed and Team Code review is initiated. When Code review has been approved by all participants the code is merged with the Develop Branch and the code is deployed to the Integration Environment.

Changes to this environment may occur several times a day.

   - **Web URL**: https://integration-us.aquaticinformatics.net
   - **API URL**: https://api-integration-us.aquaticinformatics.net

Stage
^^^^^^^^^^^^^^^^^^^

Purpose: Full regression testing prior to delivering to production environments.

The stage environment is the last place code can be tested before going to production. After an Agile Sprint is complete (Every Other Thursday) the code will be merged into the Stage Branch and this will trigger the new code to be deployed to the Stage Environment.

Changes to this environment should only occur once every two weeks.

   - **Web URL**: https://stage-us.aquaticinformatics.net
   - **API URL**: https://api-stage-us.aquaticinformatics.net

US Production
^^^^^^^^^^^^^^^^^^^

Purpose: Production usage of the applications.

The Production environments are where customers access their applications and data. After full regression testing has been performed on Stage, the code will be merged with master and the CI / CD process will deploy the new code to the production environments.

Changes to this environment may occur once every two weeks.

   - **Web URL**: https://us.aquaticinformatics.net
   - **API URL**: https://api-us.aquaticinformatics.net



.. autosummary::
   :toctree: generated
  
