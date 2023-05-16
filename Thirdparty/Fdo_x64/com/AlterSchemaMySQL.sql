/*----------------------------------------------------------------------*/
/* Name                                                                 */
/*  AlterSchemaMySQL.sql - alter table F_ATTRIBUTEDEFINITION            */
/*                                                                      */
/* Description                                                          */
/*      The file adds a new column to the table F_ATTRIBUTEDEFINITION.  */
/*                                                                      */
/* Remarks                                                              */
/*      None.                                                           */
/*----------------------------------------------------------------------*/

alter table F_ATTRIBUTEDEFINITION
  add geometrytype varchar(64) null;
commit;


