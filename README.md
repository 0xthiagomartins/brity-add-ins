# Brity RPA Custom Extensions

## Development Enviroment Setup:

1. Creating a Visual Studio Project.
    - Project type: Class library (.NET Framework, Visual c#) 
      - **File > New > Project > Installed > Visual C#**
      - Framework: .NET Framework
    - Project name: BrityWorks.AddIn.[ProjectName]

2. Add references.
    - **Project > Add Reference > Assemblies, Search**.
    - WindowsBase, PresentationCore, PresentationFramework.

3. Add the IPA AddIn reference.
    - IPA.AddIn.dll, IPA.Common.dll (Browse Designer installation directory).
    - Copy Local: Set as “False.”

4. Configure the VisualStudio project properties.
    - **Build > Output path**: Designer_directory/AddIns/
    - **Debug > Start external program**: “Designer.exe” file path
    - **Debug > Start options > Working directory**: Designer installation folder
    - **Resources > Add Resource** (AddIn default icon, icon to display on mouse-over, and icon for each activity).

for more detailed informations follow the [official brity documentation](https://www.samsungsdsbiz.com/help/BrityAutomation_User_Eng_4_0/4cdc06ba2268339d#0c90421ffe6ed15b) 