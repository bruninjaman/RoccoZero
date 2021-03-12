namespace O9K.Core.Managers.Menu
{
    //using EnsageSharp.Sandbox;

    public enum Lang
    {
        En,

        Ru,

        Cn
    }

    internal static class Language
    {
        public static Lang GetLanguage()
        {
            //switch (language.Selected)
            {
                /*case "RU":
                    return Lang.Ru;
                case "ZH":
                    return Lang.Cn;
                default:*/
                    return Lang.En;
            }
        }
    }
}