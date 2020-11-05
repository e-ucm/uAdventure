using System;
using WixSharp;

static class Script
{
    static public void Main()
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Docs\Manual.txt"),
                    new File(@"Files\Bin\MyApp.exe")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }
}