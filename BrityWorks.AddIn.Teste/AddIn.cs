using System.Collections.Generic;
using System.Drawing;
using BrityWorks.AddIn.Teste.Properties;
using RPAGO.AddIn;
using BrityWorks.AddIn.Teste.Activities;

namespace BrityWorks.AddIn.Teste
{
    public class AddIn : ActivityAddInBase
    {
        protected override string AddInDisplayName => "Teste AddIn";

        protected override Bitmap AddInIcon => Resources.teste;

        protected override Bitmap AddInOverIcon => Resources.teste_hover;

        protected override List<IActivity> CreateActivites()
        {
            List<IActivity> activities = new List<IActivity>()
            {
                new NormalActivitySample(),
            };
            return activities;
        }
    }
}
