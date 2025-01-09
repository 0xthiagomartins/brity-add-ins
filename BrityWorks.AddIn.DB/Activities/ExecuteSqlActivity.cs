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
    internal class ExecuteSqlActivity : IActivityItem
    {
        // Usamos a mesma chave de ConnectionReference da atividade de conexão
        public static readonly PropKey ConnectionReferencePropKey = ConnectToDatabaseActivity.ConnectionReferencePropKey;

        // Comando SQL
        public static readonly PropKey SqlCommandPropKey = new PropKey("Database", "SqlCommand");

        // PropKey de saída, para mostrar o resultado (quantidade de linhas afetadas)
        public static readonly PropKey RowsAffectedPropKey = new PropKey("Database", "RowsAffected");

        public string DisplayName => "Execute SQL Command";

        public Bitmap Icon => Resources.DBExecute;

        public LibraryHeadlessType Mode => LibraryHeadlessType.Both;

        public PropKey DisplayTextProperty => RowsAffectedPropKey;

        // Indicamos essa PropKey como nossa saída
        public PropKey OutputProperty => RowsAffectedPropKey;

        public List<Property> OnCreateProperties()
        {
            return new List<Property>()
            {
                new Property(this, SqlCommandPropKey, "UPDATE MyTable SET IsActive = 0 WHERE Id = 1").SetRequired()
            };
        }

        public void OnLoad(PropertySet properties)
        {
            return;
        }

        public object OnRun(IDictionary<string, object> properties)
        {
            // Recuperamos a conexão
            if (!properties.ContainsKey(ConnectionReferencePropKey))
            {
                return "No database connection found. Please run 'Connect to Database' first.";
            }

            var sqlConnection = properties[ConnectionReferencePropKey] as SqlConnection;
            if (sqlConnection == null || sqlConnection.State != ConnectionState.Open)
            {
                return "The connection is not open.";
            }

            string sqlCommandText = properties[SqlCommandPropKey].ToString();

            try
            {
                using (SqlCommand cmd = new SqlCommand(sqlCommandText, sqlConnection))
                {
                    // ExecuteNonQuery retorna o número de linhas afetadas
                    int rowsAffected = cmd.ExecuteNonQuery();

                    string resultMessage = $"SQL command executed successfully. {rowsAffected} row(s) were affected.";

                    // Salvamos o valor em nossa PropKey de saída
                    properties[RowsAffectedPropKey] = resultMessage;

                    // Também retornamos a string
                    return resultMessage;
                }
            }
            catch (Exception ex)
            {
                // Em caso de erro, também podemos devolver o erro diretamente
                return $"Error while executing SQL: {ex.Message}";
            }
        }
    }
}
