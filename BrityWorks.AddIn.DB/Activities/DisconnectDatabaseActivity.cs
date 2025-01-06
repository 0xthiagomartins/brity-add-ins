using RPAGO.AddIn;
using RPAGO.Common.Data;
using RPAGO.Common.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using BrityWorks.AddIn.DB.Properties;

namespace BrityWorks.AddIn.DB.Activities
{
    internal class DisconnectDatabaseActivity : IActivityItem
    {
        public static readonly PropKey ConnectionReferencePropKey = ConnectToDatabaseActivity.ConnectionReferencePropKey;

        // Defina uma PropKey para armazenar o status da desconexão
        public static readonly PropKey DisconnectStatusPropKey = new PropKey("Database", "Status");

        public string DisplayName => "Disconnect from Database";

        public Bitmap Icon => Resources.DBDisconnect;

        public LibraryHeadlessType Mode => LibraryHeadlessType.Both;

        public PropKey DisplayTextProperty => DisconnectStatusPropKey;

        // Passamos a indicar esta PropKey como saída
        public PropKey OutputProperty => DisconnectStatusPropKey;

        public List<Property> OnCreateProperties()
        {
            // Nenhuma propriedade necessária aqui
            return new List<Property>();
        }

        public void OnLoad(PropertySet properties)
        {
            return;
        }

        public object OnRun(IDictionary<string, object> properties)
        {
            string result = "";

            if (!properties.ContainsKey(ConnectionReferencePropKey))
            {
                result = "No database connection found to close.";
                properties[DisconnectStatusPropKey] = result;
                return result;  // Retorna a string
            }

            var sqlConnection = properties[ConnectionReferencePropKey] as SqlConnection;
            if (sqlConnection == null || sqlConnection.State == ConnectionState.Closed)
            {
                result = "Connection is already closed.";
                properties[DisconnectStatusPropKey] = result;
                return result;
            }

            try
            {
                sqlConnection.Close();
                properties[ConnectionReferencePropKey] = null;

                result = "Connection closed successfully.";
                properties[DisconnectStatusPropKey] = result;
                return result;
            }
            catch (Exception ex)
            {
                result = $"Error while closing connection: {ex.Message}";
                properties[DisconnectStatusPropKey] = result;
                return result;
            }
        }
    }
}
