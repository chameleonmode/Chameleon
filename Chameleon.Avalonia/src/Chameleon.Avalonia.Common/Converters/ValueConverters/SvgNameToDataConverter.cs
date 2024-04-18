using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Svg.Skia;
using Svg;
using Svg.Skia;
using System;
using System.Globalization;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Xml;
using System.Xml.XPath;

namespace Chameleon.Avalonia.Common.Converters.ValueConverters;

public class SvgNameToDataConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || value.ToString() == string.Empty)
        {
            return null;
        }
        var uri = $"avares://Chameleon.Avalonia.Common/Assets/Svgs/{value}.svg";
        using var stream = AssetLoader.Open(new Uri(uri));
        using var reader = new StreamReader(stream);
        var data = reader.ReadToEnd();


        var r = Application.Current.RequestedThemeVariant;
        var a = Application.Current.ActualThemeVariant;
        // var tv = Application.Current.ActualThemeVariant;
        if (Application.Current.TryGetResource("AccentFillColorDefaultBrush", Application.Current.ActualThemeVariant,out object accentbrush) &&
            Application.Current.TryGetResource("ControlStrokeColorOnAccentDefaultBrush", Application.Current.ActualThemeVariant, out object strokebrush))
        {
            var acc = accentbrush.ToString().Replace("#ff", "#");
            //8094AE   B5B5B5
            data = data.Replace("#5D25A6", acc);
            data = data.Replace("#8094AE", acc);
            data = data.Replace("#B5B5B5", acc);
            if (a == ThemeVariant.Dark)
                data = data.Replace("fill=\"black\"", "fill=\"white\"");
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(data);
            //XPathNavigator navigator = xmlDoc.CreateNavigator();

            //XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
            //SetThemeColors(xmlDoc.ChildNodes, accentbrush.ToString().Replace("#ff", "#"), strokebrush.ToString().Replace("#ff", "#"));

            //return xmlDoc.InnerXml;

        }
        return data;
    //  var d = SvgDocument.Open<SvgDocument>(AssetLoader.Open(new Uri(uri)));
        /// using var svg = new SKSvg();
        //   var pic = svg.Load(AssetLoader.Open(new Uri(uri)));
        //  return d.Content;
    }

    private void SetThemeColors(XmlNodeList childNodes, string accent, string stroke)
    {
        if (childNodes?.Count > 0)
        {
            for (int i = 0; i < childNodes.Count; i++)
            {
                XmlNode node = childNodes.Item(i);
                var fill = node.Attributes["fill"];
                if(fill is not null && fill.Value == "#5D25A6")
                   fill.Value = accent;
                //var str = node.Attributes["stroke"];
                //if (str is null)
                //{
                //    str = node.OwnerDocument.CreateAttribute("stroke");
                //    node.Attributes.Append(str);
                //}
                       
                //    str.Value = stroke;
                //foreach (XmlAttribute item in node.Attributes)
                //{
                //    if (item.Name == "fill" && item.Value != "none")
                //        item.Value = accent;
                //    else if (item.Name == "stroke")
                //        item.Value = stroke;
                //}

                SetThemeColors(node.ChildNodes, accent, stroke);
            }
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
