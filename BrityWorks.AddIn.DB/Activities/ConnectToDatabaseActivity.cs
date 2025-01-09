using BrityWorks.AddIn.DB.Properties;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using RPAGO.AddIn;
using RPAGO.Common.Library;
using RPAGO.Common.Data;
using System.IO;
using System.Windows;

namespace BrityWorks.AddIn.DB.Activities
{
    internal class ConnectToDatabaseActivity : IActivityItem
    {
        public static readonly PropKey ConnectionStringPropKey = new PropKey("Database", "ConnectionString");
        public static readonly PropKey ConnectionReferencePropKey = new PropKey("Database", "ConnectionReference");

        public string DisplayName => "Connect to Database";
        public System.Drawing.Bitmap Icon => Resources.DBConnect;

        public LibraryHeadlessType Mode => LibraryHeadlessType.Both;

        public PropKey DisplayTextProperty => ConnectionReferencePropKey;
        public PropKey OutputProperty => ConnectionReferencePropKey;


        public List<Property> OnCreateProperties()
        {
            var properties = new List<Property>()
            {
                new Property(this, ConnectionReferencePropKey, "RESULT").SetRequired(),
                new Property(this, ConnectionStringPropKey, "'Server=localhost;Database=SampleDB;User Id=user;Password=pass;'").SetRequired(),
            };
            return properties;
        }

        public void OnLoad(PropertySet properties)
        {
            return;
        }

        public object OnRun(IDictionary<string, object> properties)
        {
            string connectionString = properties[ConnectionStringPropKey].ToString();

            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                // Retornamos a conexão para que fique disponível para as próximas Activities.
                // Observação: em um fluxo real, pode ser interessante usar connection pooling,
                // ou até armazenar de outra forma, para evitar a desconexão automática.
                properties[ConnectionReferencePropKey] = sqlConnection;

                return sqlConnection;
            }
            catch (Exception ex)
            {
                return $"Error while connecting: {ex.Message}";
            }
        }
    }
}
