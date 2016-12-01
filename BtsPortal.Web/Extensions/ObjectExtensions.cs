using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using BtsPortal.Entities.Bam;
using BtsPortal.Entities.Bts;
using BtsPortal.Web.Infrastructure.Settings;

namespace BtsPortal.Web.Extensions
{
    public static class ObjectExtensions
    {
        public static List<SelectListItem> ToSelectListItems(this List<BtsModule> modules, int? selected)
        {
            return modules.OrderBy(m => m.ModuleName).Select(module => new SelectListItem()
            {
                Text = module.ModuleName,
                Value = module.ModuleId.ToString(),
                Selected = module.ModuleId == selected
            }).ToList();
        }

        public static List<SelectListItem> ToSelectListItems(this List<BtsApplication> apps, bool insertEmptyOnTop = true, string selected = null)
        {
            var list = apps.OrderBy(m => m.Name).Select(module => new SelectListItem()
            {
                Text = module.Name,
                Value = module.Name.ToString(),
                Selected = module.Name == selected
            }).ToList();

            if (insertEmptyOnTop)
            {
                list.Insert(0, new SelectListItem() { Text = "--Select an application--" });
            }
            return list;

        }

        public static List<SelectListItem> ToSelectListItems(this List<SsoApplication> apps, bool insertEmptyOnTop = true, string selected = null)
        {
            var btdfGrp = new SelectListGroup() { Name = "BTDF" };
            var msSsoGrp = new SelectListGroup() { Name = "MS SSO" };
            var list = apps.OrderBy(m => m.Name).Select(module => new SelectListItem()
            {
                Text = module.Name,
                Value = module.Name.ToString(),
                Selected = module.Name == selected,
                Group = string.Equals(module.ContactInfo, AppSettings.SsoApplicationBtdfContactEmail, StringComparison.OrdinalIgnoreCase) ? btdfGrp : msSsoGrp
            }).ToList();

            if (insertEmptyOnTop)
            {
                list.Insert(0, new SelectListItem() { Text = "--Select an application--" });
            }
            return list;

        }

        public static string ToHtmlTable(this List<BtsOrchestrationPort> ports)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<table class='table-font11'>");

            foreach (var port in ports)
            {
                builder.AppendLine("<tr>").Append("<td>Binding</td>").Append("<td>&nbsp;</td>").Append("<td>").Append(port.Binding).Append("</td>").AppendLine("</tr>");
                builder.AppendLine("<tr>").Append("<td>&nbsp;</td><td>&nbsp;</td>").Append("<td>&nbsp;</td>").AppendLine("</tr>");
                builder.AppendLine("<tr>").Append("<td>Type</td>").Append("<td>&nbsp;</td>").Append("<td>").Append(port.PortType).Append("</td>").AppendLine("</tr>");
                builder.AppendLine("<tr>").Append("<td>&nbsp;</td><td>&nbsp;</td>").Append("<td>&nbsp;</td>").AppendLine("</tr>");

                builder.AppendLine("<tr>").Append("<td>Logical </td>").Append("<td>&nbsp;</td>").Append("<td>").Append(string.Format(" <span class='span-width-150'>{0}</span>", port.OrchestrationPortName)).Append("</td>").AppendLine("</tr>");
                builder.AppendLine("<tr>").Append("<td>&nbsp;</td><td>&nbsp;</td>").Append("<td>&nbsp;</td>").AppendLine("</tr>");

                builder.AppendLine("<tr>").Append("<td>Physical</td>").Append("<td>&nbsp;</td>").Append("<td>").Append(string.Format(" <span class='span-width-250'>{0}</span>", port.PhysicalPortName)).Append("</td>").AppendLine("</tr>");

            }

            builder.AppendLine("</table>");
            return builder.ToString();

        }

        public static string ToHtmlTable(this List<ActivityViewFilterParameter> parameters)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<table class='table table-font11 table-striped'>");
            builder.AppendLine("<tr>");
            int counter = 0;
            bool isFirst = true;
            foreach (var item in parameters)
            {
                counter++;
                if (counter % 2 == 1 && !isFirst)
                {
                    builder.AppendLine("</tr>");
                    builder.AppendLine("<tr>");
                }


                builder.AppendLine($"<td> <span id='sp_{item.Name}'>{item.DisplayName}</span>: </td>");
                if (item.ParameterType == ParameterType.Number)
                {
                    builder.AppendLine($"<td><input type='text' class='input-xs form-control form-input numeric' id='{item.Name}' value='{item.Value}'/> </td>");
                }
                else if (item.ParameterType == ParameterType.Date)
                {
                    builder.AppendLine($"<td><input type='text' class='input-xs form-control form-input datetimepicker' id='{item.Name}' value='{item.Value}'/> </td>");
                }
                else
                {
                    builder.AppendLine($"<td><input type='text' class='input-xs form-control form-input' id='{item.Name}'/> </td>");
                }


                isFirst = false;
            }

            if (counter % 2 == 1)
            {
                builder.AppendLine("<td></td><td></td>");
            }

            builder.AppendLine("</tr>");
            builder.AppendLine("</table>");
            return builder.ToString();
        }


    }
}