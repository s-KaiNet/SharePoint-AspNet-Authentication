echo off
nuget pack ../../AspNet.Owin.SharePoint.Addin.Authentication.csproj -IncludeReferencedProjects -Prop Configuration=Release -OutputDirectory "../../Release"