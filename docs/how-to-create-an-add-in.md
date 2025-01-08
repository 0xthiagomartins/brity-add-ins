## Resource Management

1. Add Resources:
   - Right-click on Properties > Add > Resource File
   - Add icons and other resources to the project
   - Access resources using `Resources.ResourceName`

2. Add Localization (Optional):
   ```xml
   <windows:ResourceDictionary
       xmlns:windows="clr-namespace:System.Windows;assembly=PresentationFramework"
       xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
       xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
       xmlns:system="clr-namespace:System;assembly=mscorlib">

       <!-- Titles -->
       <system:String x:Key="MSG_TITLE_YOUR_ACTIVITY">Your Activity Title</system:String>

       <!-- Properties -->
       <system:String x:Key="PROP_YOUR_PROPERTY">Property Description</system:String>
   </windows:ResourceDictionary>
   ```

## Building and Testing

1. Build the project:
   - The DLL will be generated in the specified output path
   - Ensure all required resources are included

2. Test the Add-in:
   - Launch Brity RPA Designer
   - Your add-in should appear in the activities panel
   - Test each activity's functionality

## Best Practices

1. Error Handling:
   ```csharp
   public object OnRun(IDictionary<string, object> properties)
   {
       try
       {
           // Your activity logic here
           return "Success";
       }
       catch (Exception ex)
       {
           return $"Error: {ex.Message}";
       }
   }
   ```

2. Resource Management:
   ```csharp
   using (var resource = new DisposableResource())
   {
       // Use resource
   } // Resource automatically disposed
   ```

3. Naming Conventions:
   - Use PascalCase for class names and public members
   - Use camelCase for private fields
   - Use descriptive names for activities and properties

## Example Projects

1. Database Add-in Example:
   ```csharp
   public class ConnectToDatabaseActivity : IActivityItem
   {
       public static readonly PropKey ConnectionStringPropKey = 
           new PropKey("Database", "ConnectionString");

       public string DisplayName => "Connect to Database";
       public Bitmap Icon => Resources.DBConnect;

       public object OnRun(IDictionary<string, object> properties)
       {
           try
           {
               string connectionString = properties[ConnectionStringPropKey].ToString();
               SqlConnection sqlConnection = new SqlConnection(connectionString);
               sqlConnection.Open();
               return "Connection opened successfully.";
           }
           catch (Exception ex)
           {
               return $"Error while connecting: {ex.Message}";
           }
       }
   }
   ```

2. TwoCaptcha Add-in Example:
   ```csharp
   public class CreateRecaptchaV2Task : IActivityItem
   {
       public static readonly PropKey ClientKeyPropKey = 
           new PropKey("TwoCaptcha", "ClientKey");
       public static readonly PropKey WebsiteURLPropKey = 
           new PropKey("TwoCaptcha", "WebsiteURL");

       public string DisplayName => "Create reCAPTCHA V2 Task";
       public Bitmap Icon => Resources.Icon;
   }
   ```

## Troubleshooting

Common issues and solutions:

1. Add-in not loading:
   - Verify output path in project properties
   - Check if all required DLLs are referenced
   - Ensure compatible .NET Framework version

2. Resources not found:
   - Verify resource build action is set to "Embedded Resource"
   - Check namespace matches project structure
   - Rebuild solution after adding new resources

3. Runtime errors:
   - Use Visual Studio debugging
   - Check activity property values
   - Verify input validation logic

For more information, refer to the [official Brity documentation](https://www.samsungsdsbiz.com/help/BrityAutomation_User_Eng_4_0/4cdc06ba2268339d#0c90421ffe6ed15b).
