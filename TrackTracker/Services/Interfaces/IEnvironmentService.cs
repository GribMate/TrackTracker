using System.Collections.Generic;



namespace Onlab.Services.Interfaces
{
    /*
    Interface: IEnvironmentService
    Description:
        Handles environmental input and output operations, like Windows Registry values, local drive names, config paths and so on.
    */
    public interface IEnvironmentService
    {
        bool InternetConnectionIsAlive(); //returns true if the application can connect to the internet
        string TryFindFoobar(); //tries to locate foobar2000 installation through various methods, returns null for no success
        List<string> GetExternalDriveNames(); //returns all attached external drive names
    }
}
