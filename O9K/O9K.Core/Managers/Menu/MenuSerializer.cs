namespace O9K.Core.Managers.Menu;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

using Divine.Zero.Helpers;

using Items;

using Logger;

internal sealed class MenuSerializer
{
    public MenuSerializer()
    {
        this.ConfigDirectory = Path.Combine(Directories.Config, "Plugins", "O9K");

        try
        {
            Directory.CreateDirectory(this.ConfigDirectory);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    public string ConfigDirectory { get; }

    public JsonNode Deserialize(MenuItem menuItem)
    {
        var file = Path.Combine(this.ConfigDirectory, menuItem.Name + ".json");

        if (!File.Exists(file))
        {
            return null;
        }

        try
        {
            var json = File.ReadAllText(file);
            return JsonNode.Parse(json);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return null;
        }
    }

    public void Serialize(MainMenu mainMenu)
    {
        var menus = mainMenu.MenuItems.ToList();
        menus.Add(mainMenu);

        foreach (var menu in menus)
        {
            using var fs = new FileStream(Path.Combine(this.ConfigDirectory, menu.Name + ".json"), FileMode.Create, FileAccess.Write);
            using var writer = new Utf8JsonWriter(fs, new() { Indented = true });
            writer.WriteStartObject();
            Serialize(writer, menu);
            writer.WriteEndObject();
        }
    }

    private static void Serialize(Utf8JsonWriter writer, MenuItem menuItem)
    {
        if (menuItem is Menu menu && !menu.IsMainMenu)
        {
            var saved = new List<string>();

            writer.WritePropertyName(menuItem.Name);
            writer.WriteStartObject();

            foreach (var item in menu.MenuItems.ToList())
            {
                Serialize(writer, item);
                saved.Add(item.Name);
            }

            if (menu.JsonNode != null)
            {
                foreach (var item in menu.JsonNode.AsObject())
                {
                    if (saved.Contains(item.Key))
                    {
                        continue;
                    }

                    writer.WritePropertyName(item.Key);
                    JsonSerializer.Serialize(writer, item.Value, new JsonSerializerOptions() { WriteIndented = true });
                }
            }

            writer.WriteEndObject();
        }
        else
        {
            var value = menuItem.GetSaveValue();
            if (value == null)
            {
                return;
            }

            writer.WritePropertyName(menuItem.Name);
            WritePropertyValue(writer, value);
        }
    }

    private static void WritePropertyValue(Utf8JsonWriter writer, object propertyValue)
    {
        var propertyType = propertyValue.GetType();
        if (propertyType.IsArray && propertyValue is object[] values)
        {
            writer.WriteStartArray();

            foreach (var value in values)
            {
                JsonSerializer.Serialize(writer, value);
            }

            writer.WriteEndArray();
        }
        else
        {
            JsonSerializer.Serialize(writer, propertyValue);
        }
    }
}