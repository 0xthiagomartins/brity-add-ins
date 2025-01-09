using System.Collections.Generic;
using System.Drawing;
using BrityWorks.AddIn.DB.Properties;
using RPAGO.AddIn;
using BrityWorks.AddIn.DB.Activities;


namespace BrityWorks.AddIn.DB
{   
    public class AddIn : ActivityAddInBase
    {
        protected override string AddInDisplayName => "DB AddIn";
        protected override Bitmap AddInIcon => Resources.DBIcon;
        protected override Bitmap AddInOverIcon => Resources.DBIconOver;

        protected override List<IActivity> CreateActivites()
        {
            List<IActivity> activities = new List<IActivity>()
            {
                new ConnectToDatabaseActivity(),
                new ExecuteView(),
                /*new DisconnectDatabaseActivity(),*/
            };
            return activities;
        }
    }
}
