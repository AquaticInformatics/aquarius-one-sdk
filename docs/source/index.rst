Welcome to Aquarius ONE SDK documentation!
===================================

**Aquarius ONE SDK** is a C# SDK for .NET enabling developers to interact with the Aquarius ONE Platform.
Documentation for the RESTful APIs can be found at: `Aquarius ONE Developer Portal <https://aqi-feature-api-mgmt.developer.azure-api.net/>`_.

Check out the :doc:`usage` section for further information, including
how to :ref:`installation` the nuget package.

.. note::

   This project is under active development.

Contents
--------

.. toctree::
   :caption: Introduction

   usage
   getting-started
   clientsdk

.. toctree::
   :caption: Core Concepts

   concepts/authorization
   concepts/environments
   concepts/digitaltwin
   concepts/library
   concepts/spreadsheet


.. toctree::
   :caption: Authentication & Authorization APIs

   enterprise/authentication
   enterprise/core
   enterprise/data
   enterprise/twin

.. toctree::
   :caption: Common Service APIs
   
   common/activities
   common/configuration
   common/library
   common/notifications
   enterprise/report
   enterprise/timezone

.. toctree::
   :caption: Data APIs

   operation/spreadsheet
   historical/data

.. toctree::
   :caption: Caching APIs

   cache/cache
   cache/librarycache
   cache/operationscache
   cache/operationcache
   cache/reportcache

.. toctree::
   :caption: Helper APIs

   helpers/environment
   helpers/logger
   helpers/poeditor
   helpers/qualifiers
   helpers/rownumber