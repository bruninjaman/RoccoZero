using Ensage.Common.Menu;
using Ensage.SDK.Menu;
using Ensage.SDK.Renderer;
using SharpDX;

namespace TinkerCrappahilationPaid
{
    public class InfoPanel
    {
        private TinkerCrappahilationPaid _main;

        public InfoPanel(TinkerCrappahilationPaid main)
        {
            _main = main;
            Factory = main.Config.Factory.Menu("Info Panel");
            Enable = Factory.Item("Draw panel", true);
            PosX = Factory.Item("Position X", new Slider(28, 0, 2000));
            PosY = Factory.Item("Position Y", new Slider(127, 0, 2000));
            TextSize = Factory.Item("Text Size", new Slider(20, 1, 50));

            if (Enable)
                Activate();

            Enable.PropertyChanged += (sender, args) =>
            {
                if (Enable)
                    Activate();
                else
                    Deactivate();
            };
        }

        public MenuItem<Slider> TextSize { get; set; }

        public MenuItem<Slider> PosX { get; set; }
        public MenuItem<Slider> PosY { get; set; }

        public IRenderManager Renderer => _main.Context.RenderManager;

        public MenuItem<bool> Enable { get; set; }

        public MenuFactory Factory { get; set; }

        private void Activate()
        {
            Renderer.Draw += Renderer_Draw;
        }


        private void Deactivate()
        {
            Renderer.Draw -= Renderer_Draw;
        }

        private void Renderer_Draw(IRenderer renderer)
        {
            var text = _main.AutoPushing.Enable ? "In AutoPushing" : _main.Config.ComboKey ? "In Combo" : "Idle";
            renderer.DrawText(new Vector2(PosX, PosY), text, System.Drawing.Color.Red, TextSize);
        }
    }
}