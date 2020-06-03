CRUD (Create, Read, Update, Delete) Operations
==============================================

FDO Toolbox supports CRUD operations. The level of support varies from provider to provider. Generally speaking:

 * Creating data is supported if the provider supports the ``IInsert`` FDO command.
 * Reading data is supported if the provider supports the ``ISelect`` FDO command.
 * Updating data is supported if the provider supports the ``IUpdate`` FDO command.
 * Deleting data is supported if the provider supports the ``IDelete`` FDO command.
 
.. index::
   single: Inserting Features
 
Creating (Inserting) Features
-----------------------------

1. Open the **Data Query** view for your target feature class. Or alternatively right click your target feature class and select :guilabel:`Insert New Feature`. If you chose this approach, goto step 3.
2. Click the :guilabel:`New Feature` button on the toolbar. If the underlying connection does not support the ``ISelect`` command, this button will be disabled.

.. image:: content/data-query-new-feature.png

3. A new dialog will appear similar to the one below. Green fields indicate optional fields. You can choose to perform this insert inside a transaction by checking the :guilabel:`Use Transaction` box. This is only enabled if the underlying connection supports transactions. For geometry fields, you must enter **valid** FGF geometry text.

.. image:: content/crud-insert.png

4. Click :guilabel:`Insert` to insert the new feature.

.. index::
   single: Updating Features

Updating Features
-----------------

1. Open the **Data Query** view for your target feature class.
2. Run a query where the feature you want to update will be part of the result set.
3. Right click the feature from the result set that you want to update. Select :guilabel:`Update this Feature`

.. image:: content/data-query-update.png

4. You will get the following dialog. Just like the **Insert New Feature** dialog, the green fields indicate optional fields, and geometry fields must have valid FGF text. You can also choose to perform this update inside a transaction by checking the :guilabel:`Use Transaction` box. Once again, it is only enabled if the underlying connection supports transactions.

.. image:: content/crud-update.png

5. Make your changes and click :guilabel:`Update` to update the feature.

.. index::
   single: Deleting Features

Deleting Features
-----------------

1. Open the **Data Query** view for your target feature class.
2. Run a query where the feature you want to delete will be part of the result set.
3. Right click the feature from the result set that you want to update. Select :guilabel:`Update this Feature`

.. image:: content/data-query-delete-confirm.png

4. If you click :guilabel:`Yes` the feature will be deleted. You can re-run the same query as before to confirm that the feature is now gone.

.. index::
   single: Bulk Updating Features

Bulk Updating Features
----------------------

**WARNING**: This is a very dangerous operation. If you are not careful with your update filters, it could cause irreversible data changes!

1. Right click the feature class you want to bulk update and choose :guilabel:`Bulk Update`. You will get the following warning dialog, click :guilabel:`Yes` to proceed.

.. image:: content/bulk-update-confirm.png

2. You will get the following dialog. Set the update filter which will determine how many features will be updated.

.. image:: content/bulk-update-dialog.png

3. Enter the values for the properties you want to update. Only ticked property values will be considered in the update.

3. Before you perform the bulk update, it is seriously recommended to test the update filter by clicking :guilabel:`Test`. This will tell you how many features would be updated by this given filter.

.. image:: content/bulk-update-test.png

4. Once you are happy with the test. Click :guilabel:`Delete` to perform the bulk update.

.. index::
   single: Bulk Deleting Features
   
Bulk Deleting Features
----------------------

**WARNING**: This is a very dangerous operation. If you are not careful with your delete filters, it could cause irreversible data loss!

1. Right click the feature class you want to bulk delete and choose :guilabel:`Bulk Delete`. You will get the following warning dialog, click :guilabel:`Yes` to proceed.

.. image:: content/bulk-delete-confirm.png

2. You will get the following dialog. Set the delete filter which will determine how many features will be deleted.

.. image:: content/bulk-delete-dialog.png

3. Before you perform the bulk delete, it is seriously recommended to test the delete filter by clicking :guilabel:`Test`. This will tell you how many features would be deleted by this given filter.

.. image:: content/bulk-delete-test.png

4. Once you are happy with the test. Click :guilabel:`Delete` to perform the bulk delete.


