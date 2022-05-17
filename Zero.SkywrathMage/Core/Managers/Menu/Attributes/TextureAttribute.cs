namespace Ensage.SDK.Menu.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class TextureAttribute : Attribute
    {
        public TextureAttribute(string textureKey)
        {
            TextureKey = textureKey;
        }

        public string TextureKey { get; }
    }
}