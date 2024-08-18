using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace TestRadComboBox
{
internal sealed class PostgreSQLDataBase
{
static NpgsqlConnection _conn = null;
static PostgreSQLDataBase Instance = null;
string ConnectionString = null;
private PostgreSQLDataBase()
{
    try
    {
        ConnectionString = ConfigurationManager.
            ConnectionStrings["myinvoices"].ConnectionString;
    }
    catch (Exception ex)
    {
        Logger.LogWriteError(ex.Message);
        throw ex;
    }
}
private static void CreateInstance()
{
    if (Instance == null)
    { Instance = new PostgreSQLDataBase(); }
}
public static PostgreSQLDataBase GetInstance()
{
    if (Instance == null)
        CreateInstance();
    return Instance;
}
public NpgsqlConnection GetConnection()
{
    try
    {
        _conn = new NpgsqlConnection(ConnectionString);
        _conn.Open();
        return _conn;
    }
    catch (Exception ex)
    {
        Logger.LogWriteError(ex.Message);
        throw ex;
    }
}
}
}
