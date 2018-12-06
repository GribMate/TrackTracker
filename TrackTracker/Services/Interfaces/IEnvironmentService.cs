using System;
using System.Collections.Generic;



namespace TrackTracker.Services.Interfaces
{
    /*
    Interface: IEnvironmentService
    Description:
        Handles environmental input and output operations, like Windows Registry values, local drive names, config paths and so on.
    */
    public interface IEnvironmentService
    {
        bool InternetConnectionIsAlive(); //returns true if the application can connect to the internet
        List<string> GetExternalDriveNames(); //returns all attached external drive names
    }
}
