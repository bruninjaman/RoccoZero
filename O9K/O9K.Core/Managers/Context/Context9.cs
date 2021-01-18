namespace O9K.Core.Managers.Context
{
    using System;

    using Assembly;

    //using Ensage.SDK.Renderer;

    //using Input;

    using Jungle;

    using Menu;

    using Particle;

    public static class Context9
    {
        private static readonly Lazy<IAssemblyEventManager9> assemblyEventManager;

        //private readonly Lazy<IInputManager9> inputManager;

        private static readonly Lazy<IJungleManager> jungleManager;

        private static readonly Lazy<IMenuManager9> menuManager;

        private static readonly Lazy<IParticleManager9> particleManger;

        //private readonly Lazy<IRenderManager> renderer;

        //public Context9(
        //    Lazy<IAssemblyEventManager9> assemblyEventManager,
        //    Lazy<IRenderManager> renderer,
        //    Lazy<IMenuManager9> menuManager,
        //    Lazy<IInputManager9> inputManager,
        //    Lazy<IJungleManager> jungleManager
        //    Lazy<IParticleManager9> particleManger)
        //{
        //    this.assemblyEventManager = assemblyEventManager;
        //    this.renderer = renderer;
        //    this.menuManager = menuManager;
        //    this.inputManager = inputManager;
        //    this.jungleManager = jungleManager;
        //    this.particleManger = particleManger;
        //}

        static Context9()
        {
            assemblyEventManager = new Lazy<IAssemblyEventManager9>(() => new AssemblyEventManager9());
            jungleManager = new Lazy<IJungleManager>(() => new JungleManager());
            menuManager = new Lazy<IMenuManager9>(() => new MenuManager9());
            particleManger = new Lazy<IParticleManager9>(() => new ParticleManager9());
        }

        public static IAssemblyEventManager9 AssemblyEventManager
        {
            get
            {
                return assemblyEventManager.Value;
            }
        }

        /*public IInputManager9 InputManager
        {
            get
            {
                return this.inputManager.Value;
            }
        }*/

        public static IJungleManager JungleManager
        {
            get
            {
                return jungleManager.Value;
            }
        }

        public static IMenuManager9 MenuManager
        {
            get
            {
                return menuManager.Value;
            }
        }

        public static IParticleManager9 ParticleManger
        {
            get
            {
                return particleManger.Value;
            }
        }

        /*public IRenderManager Renderer
        {
            get
            {
                return this.renderer.Value;
            }
        }*/
    }
}