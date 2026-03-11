using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Xan.TextBoard.Models;

public static class TextFragmentStore
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".textboard.xml");

    public static List<TextFragment> Load()
    {
        var fragments = new List<TextFragment>();

        if (!File.Exists(FilePath))
            return fragments;

        try
        {
            var doc = XDocument.Load(FilePath);
            foreach (var element in doc.Root?.Elements("Fragment") ?? [])
            {
                var name = (string?)element.Attribute("name") ?? string.Empty;
                var text = element.Value;
                fragments.Add(new TextFragment { Name = name, Text = text });
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to load fragments: {ex.Message}");
        }

        return fragments;
    }

    public static void Save(IEnumerable<TextFragment> fragments)
    {
        try
        {
            var root = new XElement("TextBoard");
            foreach (var fragment in fragments)
                root.Add(new XElement("Fragment", new XAttribute("name", fragment.Name), fragment.Text));

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null), root);
            doc.Save(FilePath);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to save fragments: {ex.Message}");
        }
    }
}
