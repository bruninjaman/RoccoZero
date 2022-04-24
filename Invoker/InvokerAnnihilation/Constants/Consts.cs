using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.GameConsole;
using Divine.Modifier.Modifiers;
using Divine.Numerics;

namespace InvokerAnnihilation.Constants;

public static class Consts
{
    public static string TornadoModifier = "modifier_invoker_tornado";
    public static string CycloneModifier = "modifier_eul_cyclone";
    public static string WindWakerModifier = "modifier_wind_waker";
    public static string MeteorBurn = "modifier_invoker_chaos_meteor_burn";

    public static string[] InvulModifiers = {
        "modifier_invoker_tornado",
        "modifier_eul_cyclone",
        "modifier_wind_waker",
        "modifier_shadow_demon_disruption",
        "modifier_obsidian_destroyer_astral_imprisonment_prison",
        "modifier_brewmaster_storm_cyclone",
        "modifier_puck_phase_shift",
    };
}

public static class Extensions
{
    public static bool TargetIsInvul(this Unit target, out float time)
    {
        var result = target.HasAnyModifierWithRemaining(out var remaining, Consts.InvulModifiers);
        time = remaining;
        return result;
    }
    
    public static bool HasAnyModifierWithRemaining(this Unit target, out float time, params string[] modifiers)
    {
        var modifier = target.Modifiers.FirstOrDefault(x =>
            x.Name.In(modifiers));
        time = 0;
        if (modifier != null && modifier.RemainingTime >= 0.01f)
        {
            time = modifier.RemainingTime;
            return true;
        }

        return false;
    }

    public static bool IsStunned(this Unit target, out float remaining)
    {
        remaining = 0;
        if (target.IsStunned())
        {
            remaining = target.DisableDuration();
            return true;
        }

        return false;
    }
    
    public static float DisableDuration(this Unit unit, string? except = null)
    {
        Modifier? disableModifier = null;
        var maxTime = 0f;
        foreach (var modifier in unit.Modifiers)
        {
            if (
                !(modifier.IsStunDebuff /*|| modifier.Name.In(Consts.InvulModifiers)*/
                  && (except == null || modifier.Name != except)))
            {
                continue;
            }

            var remainingTime = modifier.RemainingTime;
            if (!(remainingTime > maxTime))
            {
                continue;
            }

            disableModifier = modifier;
            maxTime = remainingTime;
        }

        if (disableModifier == null)
        {
            return 0;
        }

        if (disableModifier.Name is "modifier_eul_cyclone" or "modifier_invoker_tornado")
        {
            maxTime += 0.07f;
        }

        return maxTime;
    }
    
    public static void MoveCamera(Vector3 position)
    {
        GameConsoleManager.ExecuteCommand("dota_camera_set_lookatpos " + position.X + " " + position.Y);
    }
    
    public static float GetAngle(this Hero hero, Vector3 position, bool rotationDifference = false)
    {
        var angle = Math.Abs(Math.Atan2(position.Y - hero.Position.Y, position.X - hero.Position.X) - hero.NetworkRotationRad);

        if (angle > Math.PI)
        {
            angle = Math.Abs(Math.PI * 2 - angle);
        }

        return (float)angle;
    }
    
    public static float GetTurnTime(this Hero hero, Vector3 position)
    {
        return hero.GetTurnTime(hero.GetAngle(position));
    }

    public static float GetTurnTime(this Hero hero, float angleRad)
    {
        angleRad -= 0.2f;

        if (angleRad <= 0)
        {
            return 0;
        }

        return 0.03f / hero.TurnRate() * angleRad * 1.15f;
    }
}