namespace Divine.Core.Managers.Menu;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Input;
using Divine.Menu;
using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

internal static class MenuFactory
{
    public static void RegisterMenu(object obj)
    {
        var type = obj.GetType();

        var rootMenuAttribute = type.GetCustomAttribute<MenuAttribute>();
        if (rootMenuAttribute == null)
        {
            return;
        }

        var rootMenu = MenuManager.CreateRootMenu(rootMenuAttribute.Name, rootMenuAttribute.DisplayName);

        var textureAttribute = type.GetCustomAttribute<TextureAttribute>();
        if (textureAttribute != null)
        {
            //rootMenu.SetImage();
        }

        var tooltipAttribute = type.GetCustomAttributes<TooltipAttribute>().LastOrDefault();
        if (tooltipAttribute != null)
        {
            rootMenu.SetTooltip(tooltipAttribute.Text);
        }

        foreach (var property in type.GetProperties())
        {
            var menuAttribute = property.GetCustomAttribute<MenuAttribute>();
            if (menuAttribute != null)
            {
                CreateMenu(menuAttribute.Name, menuAttribute.DisplayName, rootMenu, property.GetValue(obj), property);
            }
            else
            {
                var itemAttribute = property.GetCustomAttribute<ItemAttribute>();
                if (itemAttribute == null)
                {
                    continue;
                }

                CreateMenuItem(itemAttribute.Name, itemAttribute.DisplayName, rootMenu, obj, property);
            }
        }
    }

    private static void CreateMenu(string name, string displayName, Menu parentMenu, object obj, PropertyInfo parentProperty)
    {
        var menu = parentMenu.CreateMenu(name, displayName);

        var tooltipAttribute = parentProperty.GetCustomAttributes<TooltipAttribute>().LastOrDefault();
        if (tooltipAttribute != null)
        {
            menu.SetTooltip(tooltipAttribute.Text);
        }

        foreach (var property in obj.GetType().GetProperties())
        {
            var menuAttribute = property.GetCustomAttribute<MenuAttribute>();
            if (menuAttribute != null)
            {
                CreateMenu(menuAttribute.Name, menuAttribute.DisplayName, menu, property.GetValue(obj), property);
            }
            else
            {
                var itemAttribute = property.GetCustomAttribute<ItemAttribute>();
                if (itemAttribute == null)
                {
                    continue;
                }

                CreateMenuItem(itemAttribute.Name, itemAttribute.DisplayName, menu, obj, property);
            }
        }
    }

    private static void CreateMenuItem(string name, string displayName, Menu parentMenu, object obj, PropertyInfo property)
    {
        MenuItem menuItem = null;

        var propertyType = property.PropertyType;
        if (propertyType == typeof(MenuAbilityToggler))
        {
            var parameterAttribute = property.GetCustomAttributes<ParameterAttribute>(false).LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
                 ?? property.GetCustomAttributes<ParameterAttribute>().LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase));

            var values = new Dictionary<AbilityId, bool>();

            var valueAttributes = GetValueAttributes(property);

            foreach (var valueAttribute in valueAttributes)
            {
                var objects = valueAttribute.Objects;
                if (objects.Length < 2)
                {
                    continue;
                }

                if (objects[0] is not AbilityId abilityId || objects[1] is not bool value)
                {
                    continue;
                }

                values[abilityId] = value;
            }

            menuItem = parentMenu.CreateAbilityToggler(name, displayName, values, (bool?)parameterAttribute?.Value ?? false);
        }
        else if (propertyType == typeof(MenuHeroToggler))
        {
            var parameterAttribute = property.GetCustomAttributes<ParameterAttribute>(false).LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
                 ?? property.GetCustomAttributes<ParameterAttribute>().LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase));

            var values = new Dictionary<HeroId, bool>();

            var valueAttributes = GetValueAttributes(property);

            foreach (var valueAttribute in valueAttributes)
            {
                var objects = valueAttribute.Objects;
                if (objects.Length < 2)
                {
                    continue;
                }

                if (objects[0] is not HeroId heroId || objects[1] is not bool value)
                {
                    continue;
                }

                values[heroId] = value;
            }

            menuItem = parentMenu.CreateHeroToggler(name, displayName, values, (bool?)parameterAttribute?.Value ?? false);
        }
        else if (propertyType == typeof(MenuHoldKey))
        {
            var objects = GetValueAttributes(property).Select(x => x.Objects).LastOrDefault();
            if (objects == null || objects.Length < 1)
            {
                menuItem = parentMenu.CreateHoldKey(name, displayName, Key.None);
            }
            else
            {
                menuItem = parentMenu.CreateHoldKey(name, displayName, (Key)objects[0]);
            }
        }
        else if (propertyType == typeof(MenuItemToggler))
        {
            var parameterAttribute = property.GetCustomAttributes<ParameterAttribute>(false).LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
                 ?? property.GetCustomAttributes<ParameterAttribute>().LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase));

            var values = new Dictionary<AbilityId, bool>();

            var valueAttributes = GetValueAttributes(property);

            foreach (var valueAttribute in valueAttributes)
            {
                var objects = valueAttribute.Objects;
                if (objects.Length < 2)
                {
                    continue;
                }

                if (objects[0] is not AbilityId abilityId || objects[1] is not bool value)
                {
                    continue;
                }

                values[abilityId] = value;
            }

            menuItem = parentMenu.CreateItemToggler(name, displayName, values, (bool?)parameterAttribute?.Value ?? false);
        }
        else if (propertyType == typeof(MenuSelector))
        {
            var objects = GetValueAttributes(property).Select(x => x.Objects).LastOrDefault();
            if (objects != null)
            {
                menuItem = parentMenu.CreateSelector(name, displayName, objects.Select(x => x.ToString()).ToArray());
            }
        }
        else if (propertyType == typeof(MenuSlider))
        {
            var objects = GetValueAttributes(property).Select(x => x.Objects).LastOrDefault(x => x.Length >= 3);
            if (objects != null)
            {
                menuItem = parentMenu.CreateSlider(name, displayName, (int)objects[0], (int)objects[1], (int)objects[2]);
            }
        }
        else if (propertyType == typeof(MenuSpellToggler))
        {
            var parameterAttribute = property.GetCustomAttributes<ParameterAttribute>(false).LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
                 ?? property.GetCustomAttributes<ParameterAttribute>().LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase));

            var values = new Dictionary<AbilityId, bool>();

            var valueAttributes = GetValueAttributes(property);

            foreach (var valueAttribute in valueAttributes)
            {
                var objects = valueAttribute.Objects;
                if (objects.Length < 2)
                {
                    continue;
                }

                if (objects[0] is not AbilityId abilityId || objects[1] is not bool value)
                {
                    continue;
                }

                values[abilityId] = value;
            }

            menuItem = parentMenu.CreateSpellToggler(name, displayName, values, (bool?)parameterAttribute?.Value ?? false);
        }
        else if (propertyType == typeof(MenuSwitcher))
        {
            var objects = GetValueAttributes(property).Select(x => x.Objects).LastOrDefault(x => x.Length >= 1);
            menuItem = parentMenu.CreateSwitcher(name, displayName, (bool?)objects?[0] ?? true);
        }
        else if (propertyType == typeof(MenuText))
        {
            menuItem = parentMenu.CreateText(name, displayName);
        }
        else if (propertyType == typeof(MenuToggleKey))
        {
            var objects = GetValueAttributes(property).Select(x => x.Objects).LastOrDefault();
            if (objects == null || objects.Length < 1)
            {
                menuItem = parentMenu.CreateToggleKey(name, displayName, Key.None);
            }
            else if (objects.Length == 1)
            {
                menuItem = parentMenu.CreateToggleKey(name, displayName, (Key)objects[0]);
            }
            else
            {
                menuItem = parentMenu.CreateToggleKey(name, displayName, (Key)objects[0], (bool)objects[1]);
            }
        }
        else if (propertyType == typeof(MenuToggler))
        {
            var parameterAttribute = property.GetCustomAttributes<ParameterAttribute>(false).LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
                 ?? property.GetCustomAttributes<ParameterAttribute>().LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase));

            var values = new Dictionary<string, bool>();

            var valueAttributes = GetValueAttributes(property);

            foreach (var valueAttribute in valueAttributes)
            {
                var objects = valueAttribute.Objects;
                if (objects.Length < 2)
                {
                    continue;
                }

                if (objects[0] is not string str || objects[1] is not bool value)
                {
                    continue;
                }

                values[str] = value;
            }

            menuItem = parentMenu.CreateToggler(name, displayName, values, (bool?)parameterAttribute?.Value ?? false);
        }
        else if (propertyType == typeof(MenuUnitToggler))
        {
            var parameterAttribute = property.GetCustomAttributes<ParameterAttribute>(false).LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
                 ?? property.GetCustomAttributes<ParameterAttribute>().LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase));

            var values = new Dictionary<string, bool>();

            var valueAttributes = GetValueAttributes(property);

            foreach (var valueAttribute in valueAttributes)
            {
                var objects = valueAttribute.Objects;
                if (objects.Length < 2)
                {
                    continue;
                }

                if (objects[0] is not string str || objects[1] is not bool value)
                {
                    continue;
                }

                values[str] = value;
            }

            menuItem = parentMenu.CreateUnitToggler(name, displayName, values, (bool?)parameterAttribute?.Value ?? false);
        }

        if (menuItem != null)
        {
            var tooltipAttribute = property.GetCustomAttributes<TooltipAttribute>(false).LastOrDefault()
                ?? property.GetCustomAttributes<TooltipAttribute>().LastOrDefault();

            if (tooltipAttribute != null)
            {
                menuItem.SetTooltip(tooltipAttribute.Text);
            }
        }
    }

    public static void DeregisterMenu(object obj)
    {
    }

    private static IEnumerable<ValueAttribute> GetValueAttributes(PropertyInfo property)
    {
        var valueAttributes = property.GetCustomAttributes<ValueAttribute>(false);
        if (!valueAttributes.Any())
        {
            valueAttributes = property.GetCustomAttributes<ValueAttribute>();
        }

        return valueAttributes;
    }
}