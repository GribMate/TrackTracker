using System.Collections.Generic;



namespace Onlab.DAL
{
    /*
    Interface: IEnvironmentProvider
    Description:
        Handles environmental input and output operations, like Windows Registry values, local drive names, config paths and so on.
    */
    public interface IEnvironmentProvider
    {
        string TryFindFoobar(); //tries to locate foobar2000 installation through various methods, returns null for no success
        List<string> GetSystemDriveNames(); //returns all attached system drive names
    }
}
