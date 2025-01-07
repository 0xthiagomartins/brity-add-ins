using System.Collections.Generic;
using System.Drawing;
using RPAGO.AddIn;
using BrityWorks.AddIn.TwoCaptcha.Properties;
using BrityWorks.AddIn.TwoCaptcha.Activities;

namespace BrityWorks.AddIn.TwoCaptcha
{
    public class AddIn : ActivityAddInBase
    {
        protected override string AddInDisplayName => "TwoCaptcha";

        // Caso possua algum ícone no Resources: 
        // protected override Bitmap AddInIcon => Resources.someIcon;
        // protected override Bitmap AddInOverIcon => Resources.someIconHover;
        // Se não tiver ícones, pode retornar null
        protected override Bitmap AddInIcon => Resources.Icon;
        protected override Bitmap AddInOverIcon => Resources.Icon;

        protected override List<IActivity> CreateActivites()
        {
            // Registra as Activities
            return new List<IActivity>()
            {
                new CreateRecaptchaV2Task(),
                new GetRecaptchaV2TaskResult()
            };
        }
    }
}
