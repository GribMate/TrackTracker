using System;
using System.Collections.Generic;



namespace TrackTracker.Services.Interfaces
{
    /*
    Interface: IDatabaseService
    Description:
        Handles main data persistence.
    */
    public interface IDatabaseService
    {
        bool DatabaseExists { get; } //returns true if the SQLite database file already exists

        void CreateDatabase(); //creates a new database file if none created yet
        void FormatNewDatabase(); //formats a new database from a 0-byte file: creates tables, keys, indexes

        string[] GetRowByID(string table, string IDColumnName, string ID); //selects exactly 1 row by ID
        List<string[]> GetRowsByWhere(string table, string whereExpression); //selects multiple rows where an expression is true
        List<string[]> GetAllRows(string table); //selects all rows in a given table

        int InsertInto(string table, string[] values); //inserts a new row into a given table

        int UpdateRowByID(string table, string IDColumnName, string ID, string[] columnNames, string[] values); //updates excatly 1 row by ID
        int UpdateRowsByWhere(string table, string whereExpression, string[] columnNames, string[] values); //updates multiple rows where an expression is true

        int DeleteRowByID(string table, string IDColumnName, string ID); //deletes exaclty 1 row by ID
        int DeleteRowsByWhere(string table, string whereExpression); //deletes multiple rows where an expression is true

        bool ExecuteSQLCommand(string SQL); //executes any direct SQL command, returning true if no exceptions were thrown
    }
}
