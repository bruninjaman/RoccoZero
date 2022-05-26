namespace BAIO.Interfaces
{
    using System;
    using System.ComponentModel.Composition;

    using Ensage;

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public class ExportHeroAttribute : ExportAttribute, IHeroMetadata
    {
        public ExportHeroAttribute(HeroId id)
            : base(typeof(IHero))
        {
            this.Id = id;
        }

        public HeroId Id { get; }
    }
}