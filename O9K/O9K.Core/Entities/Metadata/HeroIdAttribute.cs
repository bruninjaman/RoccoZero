namespace O9K.Core.Entities.Metadata
{
    using System;

    using Divine;

    using Attribute = System.Attribute;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class HeroIdAttribute : Attribute
    {
        public HeroIdAttribute(HeroId heroId)
        {
            this.HeroId = heroId;
        }

        public HeroId HeroId { get; }
    }
}