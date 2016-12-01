using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace BtsPortal.Web.Extensions
{
    public static class StringExtensions
    {
        public static List<SelectListItem> ToSelectListItems(this List<string> stringList, string selected = null)
        {
            return stringList.Select(item => new SelectListItem()
            {
                Text = item,
                Value = item,
                Selected = item == selected
            }).ToList();
        }

        public static string RemoveNonAlphaNumeric(this string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(str, "");
        }

       

        //public static string PrettyXml(this string xml)
        //{
        //    var stringBuilder = new StringBuilder();

        //    var element = XElement.Parse(xml);

        //    var settings = new XmlWriterSettings();
        //    settings.OmitXmlDeclaration = true;
        //    settings.Indent = true;
        //    settings.NewLineOnAttributes = true;

        //    using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
        //    {
        //        element.Save(xmlWriter);
        //    }

        //    return stringBuilder.ToString();
        //}

        public static string PrettyXml(this string xml)
        {
           // xml = xml.Replace("</", Environment.NewLine+"</" );
            if (string.IsNullOrWhiteSpace(xml))
            {
                return xml;
            }
            xml = xml.Replace("><", ">"+Environment.NewLine+ "<");
            return xml;
        }
    }
}