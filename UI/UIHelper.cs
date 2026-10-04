using Stride.Core.IO;
using Stride.Core.Mathematics;
using Stride.Core.Storage;
using Stride.Engine;
using Stride.Games;
using Stride.Graphics;
using Stride.Graphics.Font;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;

namespace StrideTemplate.UI;

public static class UIHelper
{
    private static SpriteFont? _defaultFont;

    public static void Initialize(IGame game)
    {
        // Register the bundled font into Stride's virtual database so FontManager can find it
        RegisterFontInDatabase(game, "m5x7.ttf");

        var fontFactory = game.Services.GetService<IFontFactory>();
        _defaultFont = fontFactory?.NewDynamic(20f, "m5x7", FontStyle.Regular,
            FontAntiAliasMode.Default, false, 0f, 0f, ' ');
    }

    private static void RegisterFontInDatabase(IGame game, string fontFileName)
    {
        var fontFilePath = Path.Combine(AppContext.BaseDirectory, "assets", fontFileName);
        if (!File.Exists(fontFilePath)) return;

        var fontName = Path.GetFileNameWithoutExtension(fontFileName);
        var dbUrl = $"fonts/{fontName}.ttf";

        // Get the database provider and its underlying object database
        var dbProviderService = game.Services.GetService<IDatabaseFileProviderService>();
        var dbProvider = dbProviderService?.FileProvider;
        if (dbProvider == null) return;

        // Write font bytes into the object database and map the URL
        var fontBytes = File.ReadAllBytes(fontFilePath);
        ObjectId objectId;
        using (var stream = new MemoryStream(fontBytes))
        {
            objectId = dbProvider.ObjectDatabase.Write(stream);
        }
        dbProvider.ContentIndexMap[dbUrl] = objectId;
    }

    public static Entity CreateUIEntity(string name, UIElement rootElement)
    {
        var entity = new Entity(name);
        entity.Add(new UIComponent
        {
            Page = new UIPage { RootElement = rootElement },
            IsFullScreen = true
        });
        return entity;
    }

    public static TextBlock CreateText(string text, float fontSize = 24f, Color? color = null)
    {
        return new TextBlock
        {
            Text = text,
            Font = _defaultFont,
            TextSize = fontSize,
            TextColor = color ?? Color.White,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    public static Button CreateButton(string label, Action onClick, float fontSize = 20f)
    {
        var button = new Button
        {
            Content = new TextBlock
            {
                Text = label,
                Font = _defaultFont,
                TextSize = fontSize,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            },
            Padding = new Thickness(20, 10, 20, 10),
            HorizontalAlignment = HorizontalAlignment.Center,
            BackgroundColor = new Color(60, 60, 60, 200)
        };
        button.Click += (_, _) => onClick();
        return button;
    }

    public static StackPanel CreateVerticalStack(float gap = 10f, params UIElement[] children)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        foreach (var child in children)
            stack.Children.Add(child);
        return stack;
    }

    public static Grid CreateCenteredContainer(UIElement content)
    {
        var grid = new Grid();
        content.HorizontalAlignment = HorizontalAlignment.Center;
        content.VerticalAlignment = VerticalAlignment.Center;
        grid.Children.Add(content);
        return grid;
    }
}
