using System;
using System.Collections.Generic;
using System.Linq;

using Divine.BeAware.Data;
using Divine.BeAware.MenuManager.PartialMapHack;
using Divine.BeAware.MenuManager.ShowMeMore;
using Divine.Helpers;
using Divine.SDK.Extensions;
using Divine.SDK.Helpers;
using Divine.SDK.Managers.Log;
using Divine.SDK.Managers.Update;

using SharpDX;

namespace Divine.BeAware.Helpers
{
    internal class Verification
    {
        private MoreInformationMenu MoreInformationMenu { get; }

        private PartialMapHackMenu PartialMapHackMenu { get; }

        private MessageCreator MessageCreator { get; }

        private SoundHelper SoundHelper { get; }

        private Vector2 HeroIconExtraPos { get; }

        private Vector2 IconExtraPos { get; }

        private int IconSize { get; }

        private static readonly Random Random = new();

        private static readonly Log Log = LogManager.GetCurrentClassLogger();

        public Verification(Common common)
        {
            MoreInformationMenu = common.MenuConfig.ShowMeMoreMenu.MoreInformationMenu;
            PartialMapHackMenu = common.MenuConfig.PartialMapHackMenu;

            MessageCreator = common.MessageCreator;
            SoundHelper = common.SoundHelper;

            for (var i = 0; i <= 10; i++)
            {
                var heroColor = $"Divine.BeAware.Resources.Textures.HeroColors.{i}.png";
                RendererManager.LoadTextureFromAssembly(heroColor);
            }

            RendererManager.LoadTextureFromAssembly("Divine.BeAware.Resources.Textures.HeroColors.red.png");

            /*if (Drawing.RenderMode == RenderMode.Dx9)
            {
                HeroIconExtraPos = new Vector2(10, 12);
                IconExtraPos = new Vector2(13, 37);
                IconSize = 58;
            }
            else*/
            {
                HeroIconExtraPos = new Vector2(10, 12);
                IconExtraPos = new Vector2(12, 19);
                IconSize = 21;
            }

            RendererManager.Draw += OnRendererDraw;
        }

        public void Dispose()
        {
            //RendererManager.Draw -= OnRendererDraw;
        }

        public void EntityVerification(Vector3 position, string textureName, AbilityId abilityId, int playerId, bool isDangerous, string particleName = "")
        {
            try
            {
                var abilityTextureName = abilityId.ToString();
                var isTeleport = abilityId == AbilityId.item_tpscroll;
                var isItem = abilityTextureName.Contains("item_");
                var isEnd = ParticleCorrection.EndParticles.Any(x => particleName.Contains(x));
                if (!isTeleport)
                {
                    if (PartialMapHackMenu.ReduceTheNumberOfIconsItem && !isEnd)
                    {
                        DrawingData.RemoveAll(x => x.GetHeroTexturName.Contains(textureName));
                    }
                    else if (isItem && textureName != "npc_dota_hero_default")
                    {
                        DrawingData.RemoveAll(x => $"{x.GetHeroTexturName}_{x.GetAbilityTexturName}" == $"{textureName}_{abilityTextureName}");
                    }
                    else
                    {
                        DrawingData.RemoveAll(x => x.GetParticleName.Contains(particleName));
                    }
                }

                var random = Random.Next(int.MaxValue);
                DrawingData.Add(new Data(position, position.WorldToMinimap(), textureName, abilityTextureName, playerId, random, particleName, isItem, isEnd));

                UpdateManager.BeginInvoke((uint)(PartialMapHackMenu.DrawTimerItem.Value * 1000), () =>
                {
                    DrawingData.RemoveAll(x => x.GetRandom == random);
                });

                if (!isEnd && isDangerous)
                {
                    if (PartialMapHackMenu.SideMessageItem)
                    {
                        if (!isItem)
                        {
                            MessageCreator.MessageEnemyCreator(textureName, abilityTextureName);
                        }
                        else
                        {
                            MessageCreator.MessageItemCreator(textureName, abilityTextureName);
                        }
                    }

                    if (PartialMapHackMenu.SoundItem)
                    {
                        SoundHelper.Play(abilityTextureName);
                    }
                }
            }
            catch (Exception e)
            {
                DrawingData.Clear();
                Log.Error(e);
            }
        }

        private readonly Sleeper roshanSleeper = new();

        public void JungleVerification(Vector3 position, bool isRoshan)
        {
            try
            {
                var textureName = isRoshan ? "npc_dota_hero_roshan" : "npc_dota_hero_default";
                DrawingData.RemoveAll(x => x.GetWorldPos.Distance(position) < 500 && x.GetHeroTexturName == textureName);

                var random = Random.Next(int.MaxValue);
                DrawingData.Add(new Data(position, position.WorldToMinimap(), textureName, "default", 0, random, ""));

                UpdateManager.BeginInvoke((uint)(PartialMapHackMenu.DrawTimerItem.Value * 1000), () =>
                {
                    DrawingData.RemoveAll(x => x.GetRandom == random);
                });

                if (isRoshan && !roshanSleeper.Sleeping)
                {
                    if (PartialMapHackMenu.SideMessageItem)
                    {
                        MessageCreator.MessageEnemyCreator("npc_dota_hero_roshan", "roshan_spell_block");
                    }

                    if (PartialMapHackMenu.SoundItem)
                    {
                        SoundHelper.Play("default");
                    }

                    roshanSleeper.Sleep(8000);
                }
            }
            catch (Exception e)
            {
                DrawingData.Clear();
                Log.Error(e);
            }
        }

        public void ModifierVerification(Vector3 position, string heroTextureName, string abilityTextureName, int playerId = 0, HeroId heroId = 0)
        {
            try
            {
                string correctName;
                if (heroId != 0)
                {
                    correctName = heroId.ToString();
                    playerId = 0;
                }
                else
                {
                    correctName = heroTextureName;
                }

                if (heroId != HeroId.npc_dota_hero_nevermore)
                {
                    if (PartialMapHackMenu.ReduceTheNumberOfIconsItem)
                    {
                        DrawingData.RemoveAll(x => x.GetHeroTexturName.Contains(correctName));
                    }
                    else
                    {
                        DrawingData.RemoveAll(x => $"{x.GetHeroTexturName}_{x.GetAbilityTexturName}" == $"{correctName}_{abilityTextureName}");
                    }
                }

                var isItem = abilityTextureName.Contains("item_");

                var random = Random.Next(int.MaxValue);
                DrawingData.Add(new Data(position, position.WorldToMinimap(), correctName, abilityTextureName, playerId, random, "", isItem));

                UpdateManager.BeginInvoke((uint)(PartialMapHackMenu.DrawTimerItem.Value * 1000), () =>
                {
                    DrawingData.RemoveAll(x => x.GetRandom == random);
                });

                if (!ModifierCorrection.IgnoreModifiers.Any(x => x == abilityTextureName))
                {
                    if (PartialMapHackMenu.SideMessageItem)
                    {
                        if (isItem)
                        {
                            MessageCreator.MessageItemCreator(heroTextureName, abilityTextureName);
                        }
                        else
                        {
                            MessageCreator.MessageAllyCreator(heroTextureName, abilityTextureName);
                        }
                    }

                    if (PartialMapHackMenu.SoundItem)
                    {
                        SoundHelper.Play(abilityTextureName);
                    }
                }
            }
            catch (Exception e)
            {
                DrawingData.Clear();
                Log.Error(e);
            }
        }

        public void InfoVerification(Vector3 position, Vector2 minimapPos, string textureName, AbilityId abilityId, int playerId, bool isMessage = false, bool isSound = false)
        {
            try
            {
                var abilityTextureName = abilityId.ToString();
                DrawingData.RemoveAll(x => $"{x.GetHeroTexturName}_{x.GetAbilityTexturName}" == $"{textureName}_{abilityTextureName}");

                var random = Random.Next(int.MaxValue);
                DrawingData.Add(new Data(position, minimapPos, textureName, abilityTextureName, playerId, random, string.Empty));

                UpdateManager.BeginInvoke((uint)(MoreInformationMenu.DrawTimerItem.Value * 1000), () =>
                {
                    DrawingData.RemoveAll(x => x.GetRandom == random);
                });

                if (isMessage)
                {
                    if (abilityId == AbilityId.spirit_breaker_charge_of_darkness)
                    {
                        MessageCreator.MessageAllyCreator(textureName, abilityTextureName);
                    }
                    else
                    {
                        MessageCreator.MessageEnemyCreator(textureName, abilityTextureName);
                    }
                }

                if (isSound)
                {
                    SoundHelper.Play(abilityTextureName);
                }
            }
            catch (Exception e)
            {
                DrawingData.Clear();
                Log.Error(e);
            }
        }

        private void OnRendererDraw()
        {
            /*if (!Menu.PartialMapHackMenu.OnMinimapItem)
            {
                return;
            }*/

            foreach (var data in DrawingData.ToArray().OrderBy(x => x.GetHeroTexturName != "npc_dota_hero_default"))
            {
                if (data.GetWorldPos.IsZero)
                {
                    continue;
                }

                var pos = data.GetMinimapPos;
                if (pos.IsZero)
                {
                    continue;
                }

                var playerId = data.GetPlayerId;
                var heroTexturName = data.GetHeroTexturName;

                if (data.GetIsEnd)
                {
                    RendererManager.DrawTexture("Divine.BeAware.Resources.Textures.HeroColors.red.png", new RectangleF((pos.X - 4) - HeroIconExtraPos.X, (pos.Y - 4) - HeroIconExtraPos.Y, 26f, 26f), 1);
                }

                if (heroTexturName != "npc_dota_hero_default")
                {
                    RendererManager.DrawTexture($"Divine.BeAware.Resources.Textures.HeroColors.{playerId}.png", new RectangleF((pos.X - 2.5f) - HeroIconExtraPos.X, (pos.Y - 2.5f) - HeroIconExtraPos.Y, 23, 23), 1);
                    RendererManager.DrawTexture(heroTexturName, new RectangleF(pos.X - HeroIconExtraPos.X, pos.Y - HeroIconExtraPos.Y, 18, 18), TextureType.MiniUnit, true);
                }
                else
                {
                    RendererManager.DrawTexture($"Divine.BeAware.Resources.Textures.HeroColors.{playerId}.png", new RectangleF((pos.X - 2.5f) - HeroIconExtraPos.X, (pos.Y - 2.5f) - HeroIconExtraPos.Y, 21.5f, 21.5f), 1);
                }
            }

            foreach (var data in DrawingData.ToArray())
            {
                var worldPosition = data.GetWorldPos;
                if (worldPosition.IsZero)
                {
                    continue;
                }

                var pos = RendererManager.WorldToScreen(worldPosition);
                if (pos.IsZero)
                {
                    continue;
                }

                RendererManager.DrawTexture("Divine.BeAware.Resources.Textures.beawareplus_screen.png", new RectangleF(pos.X + 18, pos.Y - 35, 64, 128));
                RendererManager.DrawTexture(data.GetHeroTexturName, new RectangleF(pos.X + 25, pos.Y - 20, 50, 50), TextureType.RoundUnit, true);
                RendererManager.DrawTexture(data.GetAbilityTexturName, new RectangleF(pos.X + 34, pos.Y + 40, 35, 35), TextureType.RoundAbility, true);
            }
        }

        private List<Data> DrawingData { get; } = new List<Data>();

        private class Data
        {
            public string GetParticleName { get; }

            public Vector3 GetWorldPos { get; }

            public Vector2 GetMinimapPos { get; }

            public string GetHeroTexturName { get; }

            public string GetAbilityTexturName { get; }

            public int GetPlayerId { get; }

            public bool IsItem { get; }

            public bool GetIsEnd { get; }

            public int GetRandom { get; }

            public Data(Vector3 position, Vector2 minimapPos, string TexturName, string abilityTexturName, int playerId, int random, string particleName, bool isItem = false, bool isEnd = false)
            {
                GetParticleName = particleName;
                GetWorldPos = position;
                GetMinimapPos = minimapPos;
                GetHeroTexturName = TexturName;
                GetAbilityTexturName = abilityTexturName;
                GetPlayerId = playerId;
                IsItem = isItem;
                GetIsEnd = isEnd;
                GetRandom = random;
            }
        }
    }
}