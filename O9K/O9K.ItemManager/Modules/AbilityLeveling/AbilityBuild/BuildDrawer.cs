namespace O9K.ItemManager.Modules.AbilityLeveling.AbilityBuild
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Menu.Items;

    using Divine;

    using SharpDX;

    internal class BuildDrawer : IDisposable
    {
        private readonly MenuSelector<BuildType> abilitiesType;

        private readonly AbilityBuilder abilityBuilder;

        private readonly MenuSelector<BuildType> talentsType;

        public BuildDrawer(AbilityBuilder abilityBuilder, MenuSelector<BuildType> abilitiesType, MenuSelector<BuildType> talentsType)
        {
            this.abilityBuilder = abilityBuilder;

            this.abilitiesType = abilitiesType;
            this.talentsType = talentsType;

            RendererManager.Draw += this.OnDraw;
        }

        public void Dispose()
        {
            RendererManager.Draw -= this.OnDraw;
        }

        private void DrawAbilities()
        {
            var abilities = this.abilityBuilder.Abilities;
            var maxLevel = abilities.Max(x => x.LearnLevel);

            var build = new Dictionary<uint, LearnableAbility>();

            for (var i = 1u; i < maxLevel; i++)
            {
                var currentLevelAbilities = abilities.Where(
                        x => x.LearnLevel == i && this.abilityBuilder.IsLearnable(
                                 x,
                                 i,
                                 i,
                                 (uint)build.Count(z => z.Value.Ability == x.Ability)))
                    .ToList();

                if (currentLevelAbilities.Count == 0)
                {
                    continue;
                }

                var ability = this.abilitiesType.Selected == BuildType.WinRate
                                  ? currentLevelAbilities.OrderByDescending(x => x.WinRate).ThenByDescending(x => x.PickRate).First()
                                  : currentLevelAbilities.OrderByDescending(x => x.PickRate).First();

                build.Add(i, ability);
            }

            var ratio = Hud.Info.ScreenRatio;
            var xStart = Hud.Info.ScreenSize.X * 0.35f;
            var yStart = Hud.Info.ScreenSize.Y * 0.55f;
            var uniqueAbilities = build.Values.GroupBy(x => x.Ability).Select(x => x.First()).OrderBy(x => x.Ability.AbilitySlot).ToList();
            var positions = new Dictionary<Ability, float>();

            RendererManager.DrawFilledRectangle(
                new RectangleF(xStart - 2, yStart, ((build.Count + 1) * 48 * ratio) + 2, uniqueAbilities.Count * 40 * ratio),
                new Color(75, 75, 75, 175));

            for (var i = 0; i < uniqueAbilities.Count; i++)
            {
                //Drawing.DrawRect(
                //    new Vector2(xStart, yStart + (i * 40 * ratio)),
                //    new Vector2(45 * ratio, 40 * ratio),
                //    uniqueAbilities[i].Texture);
                positions.Add(uniqueAbilities[i].Ability, yStart + (i * 40 * ratio));
                RendererManager.DrawFilledRectangle(
                    new RectangleF(xStart - 2, (yStart - 2) + (i * 40 * ratio), (build.Count + 1) * 48 * ratio, 2),
                    Color.Silver);
            }

            RendererManager.DrawFilledRectangle(
                    new RectangleF(xStart - 2, (yStart - 2) + (uniqueAbilities.Count * 40 * ratio), (build.Count + 1) * 48 * ratio, 2),
                    Color.Silver);

            for (var i = 1u; i <= build.Count; i++)
            {
                var size = RendererManager.MeasureText(i.ToString(), "Arial", 35 * ratio);

                RendererManager.DrawFilledRectangle(
                    new RectangleF((xStart - 2) + (i * 48 * ratio), yStart - 2, 2, uniqueAbilities.Count * 40 * ratio),
                    Color.Silver);

                if (!build.TryGetValue(i, out var buildAbility))
                {
                    continue;
                }

                RendererManager.DrawText(
                    i.ToString(),
                    new Vector2(
                        xStart + (45 * ratio) + ((i - 1) * 48 * ratio) + (((48 * ratio) - size.X) / 2),
                        positions[buildAbility.Ability]),
                    Color.White,
                    "Arial",
                    35 * ratio);
            }

            RendererManager.DrawFilledRectangle(
                    new RectangleF((xStart - 2) + ((build.Count + 1) * 48 * ratio), yStart - 2, 2, (uniqueAbilities.Count * 40 * ratio) + 2),
                    Color.Silver);

            RendererManager.DrawFilledRectangle(
                    new RectangleF(xStart - 2, yStart - 2, 2, (uniqueAbilities.Count * 40 * ratio) + 2),
                    Color.Silver);
        }

        private void DrawTalents()
        {
            var ratio = Hud.Info.ScreenRatio;
            var xStart = Hud.Info.ScreenSize.X * 0.6f;
            var yStart = Hud.Info.ScreenSize.Y * 0.3f;

            var groups = this.abilityBuilder.Talents.GroupBy(x => x.LearnLevel).ToList();
            var talents = new Dictionary<uint, string>();

            var nameSize = new Vector2();
            foreach (var group in groups)
            {
                var ability = this.talentsType.Selected == BuildType.WinRate
                                  ? group.OrderByDescending(x => x.WinRate).First()
                                  : group.OrderByDescending(x => x.PickRate).First();

                var measure = RendererManager.MeasureText(ability.DisplayName, "Arial", 35 * ratio);
                if (measure.X > nameSize.X)
                {
                    nameSize = measure;
                }

                talents.Add(group.Key, ability.DisplayName);
            }

            var levelSize = RendererManager.MeasureText(talents.ElementAt(0).Key.ToString(), "Arial", 35 * ratio);

            RendererManager.DrawFilledRectangle(
                    new RectangleF(xStart, yStart, nameSize.X + levelSize.X + 24, talents.Count * 40 * ratio),
                    new Color(75, 75, 75, 175));

            for (var i = 0; i < talents.Count; i++)
            {
                RendererManager.DrawText(
                    talents.ElementAt(i).Key.ToString(),
                    new Vector2(xStart + 2, yStart + (i * 40 * ratio)),
                    Color.White,
                    "Arial",
                    35 * ratio);

                var size = RendererManager.MeasureText(talents.ElementAt(i).Key.ToString(), "Arial", 35 * ratio);

                RendererManager.DrawText(
                    talents.ElementAt(i).Value,
                    new Vector2(xStart + size.X + 10 + 2, yStart + (i * 40 * ratio)),
                    Color.White,
                    "Arial",
                    35 * ratio);

                RendererManager.DrawFilledRectangle(
                    new RectangleF(xStart - 2, (yStart - 2) + (i * 40 * ratio), nameSize.X + levelSize.X + 24, 2),
                    Color.Silver);
            }

            RendererManager.DrawFilledRectangle(
                    new RectangleF(xStart - 2, yStart - 2, 2, (talents.Count * 40 * ratio) + 2),
                    Color.Silver);

            RendererManager.DrawFilledRectangle(
                    new RectangleF((xStart - 2) + levelSize.X + 8, yStart - 2, 2, (talents.Count * 40 * ratio) + 2),
                    Color.Silver);

            RendererManager.DrawFilledRectangle(
                    new RectangleF((xStart - 2) + nameSize.X + levelSize.X + 24, yStart - 2, 2, (talents.Count * 40 * ratio) + 2),
                    Color.Silver);

            RendererManager.DrawFilledRectangle(
                    new RectangleF(xStart - 2, (yStart - 2) + (talents.Count * 40 * ratio), nameSize.X + levelSize.X + 24, 2),
                    Color.Silver);
        }

        private void OnDraw()
        {
            try
            {
                this.DrawAbilities();
                this.DrawTalents();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                RendererManager.Draw -= this.OnDraw;
            }
        }
    }
}