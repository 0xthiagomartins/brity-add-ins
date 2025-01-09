using BrityWorks.AddIn.DB.Properties;
using RPAGO.AddIn;
using RPAGO.Common.Data;
using RPAGO.Common.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace BrityWorks.AddIn.DB.Activities
{
    internal class ExecuteView : IActivityItem
    {
        public static readonly PropKey ConnectionReferencePropKey = ConnectToDatabaseActivity.ConnectionReferencePropKey;
        public static readonly PropKey SqlQueryPropKey = new PropKey("Database", "SqlQuery");
        public static readonly PropKey ResultArrayPropKey = new PropKey("Database", "ResultArray");

        public string DisplayName => "Execute SQL View";
        public Bitmap Icon => Resources.DBExecute;
        public LibraryHeadlessType Mode => LibraryHeadlessType.Both;
        public PropKey DisplayTextProperty => ResultArrayPropKey;
        public PropKey OutputProperty => ResultArrayPropKey;

        public List<Property> OnCreateProperties()
        {
            return new List<Property>()
            {
                new Property(this, ResultArrayPropKey, "RESULT").SetRequired(),
                new Property(this, SqlQueryPropKey, "SELECT * FROM YourView").SetRequired()
            };
        }

        public void OnLoad(PropertySet properties)
        {
            return;
        }

        public object OnRun(IDictionary<string, object> properties)
        {
            if (!properties.ContainsKey(ConnectionReferencePropKey))
            {
                throw new Exception("No database connection found. Please run 'Connect to Database' first.");
            }

            var sqlConnection = properties[ConnectionReferencePropKey] as SqlConnection;
            if (sqlConnection == null || sqlConnection.State != ConnectionState.Open)
            {
                throw new Exception("The connection is not open.");
            }

            string sqlQuery = properties[SqlQueryPropKey].ToString();

            try
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, sqlConnection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int columnCount = reader.FieldCount;
                    
                    var rows = new List<object[]>();

                    var headers = new object[columnCount];
                    for (int i = 0; i < columnCount; i++)
                    {
                        headers[i] = reader.GetName(i);
                    }
                    rows.Add(headers);

                    while (reader.Read())
                    {
                        var row = new object[columnCount];
                        reader.GetValues(row);
                        rows.Add(row);
                    }

                    var resultArray = rows.ToArray();

                    properties[ResultArrayPropKey] = resultArray;
                    return resultArray;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while executing SQL view: {ex.Message}");
            }
        }
    }
} 