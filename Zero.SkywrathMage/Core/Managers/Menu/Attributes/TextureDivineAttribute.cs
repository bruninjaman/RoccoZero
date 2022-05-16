using System;
using System.IO;

using Divine.Renderer;

namespace Ensage.SDK.Menu.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class TextureDivineAttribute : TextureAttribute
    {
        private readonly string FileName;

        public TextureDivineAttribute(string divineTexture)
            : base(divineTexture)
        {
            FileName = divineTexture;
        }

        public override void Load()
        {
            if (!File.Exists(FileName))
            {
                RendererManager.LoadImageFromResources(TextureKey, "Resources.Empty.png");
                return;
            }

            RendererManager.LoadImageFromResources(TextureKey, FileName);
        }
    }
}