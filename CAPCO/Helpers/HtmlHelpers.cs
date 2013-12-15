using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.ComponentModel;
using System;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;
using System.Web.Security;

namespace System.Web.Mvc
{
    public static class HtmlHelpers
    {
        const string pubDir = "Public";
        const string cssDir = "Stylesheets";
        const string imageDir = "Images";
        const string scriptDir = "Javascript";

        public static string ToFriendlyPricingPreference(this string value)
        {
            return ((PricePreferences)Enum.Parse(typeof(PricePreferences), value)).ToFriendlyString();
        }
        
        public static MvcHtmlString GetSectionHtml(this HtmlHelper helper, ContentSection section)
        { 
            if (section != null && !String.IsNullOrWhiteSpace(section.Text))
            {
                return MvcHtmlString.Create(section.Text);
            }
            else
            { 
                if (Roles.IsUserInRole(UserRoles.Administrators.ToString()))
                {
                    return MvcHtmlString.Create(String.Format("<p>This content section has not been configured. Visit the {0} to update the section.</p>", helper.ActionLink("administration section", "index", "content", new { area = "admin" }, null)));
                }
                else
                    return MvcHtmlString.Empty;
            }
        }

        public static MvcHtmlString DatePickerEnable(this HtmlHelper helper)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>$(document).ready(function() {$('.date-selector').datepicker();});</script>\n");
            return MvcHtmlString.Create(sb.ToString());
        }

        public static string ActiveLinkHelper(this HtmlHelper htmlHelper, RouteData routeData, string actionName, string controllerName)
        {
            if (routeData.Values["controller"].ToString().ToLower() == controllerName.ToLower())
            {
                return "activeNav";
            }
            else
            {
                return "inactiveNav";
            }
        }
        
        private static string GetAppUrl(this HtmlHelper helper)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            string appUrl = urlHelper.Content("~/");
            if (appUrl == "/")
            {
                return "";
            }
            else
            {
                return appUrl;
            }
        }

        private static string GetPublicUrl(this HtmlHelper helper)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            string appUrl = urlHelper.Content("~/");
            return appUrl + pubDir;
        }

        public static MvcHtmlString Script(this HtmlHelper helper, string fileName)
        {
            if (!fileName.EndsWith(".js"))
                fileName += ".js";
            var jsPath = string.Format("<script src='{0}/{1}/{2}' ></script>\n", GetPublicUrl(helper), scriptDir, helper.AttributeEncode(fileName));
            return MvcHtmlString.Create(jsPath);
        }

        public static MvcHtmlString CSS(this HtmlHelper helper, string fileName)
        {
            return CSS(helper, fileName, "screen");
        }

        public static MvcHtmlString CSS(this HtmlHelper helper, string fileName, string media)
        {
            string cssPath = String.Empty;

            if (!fileName.EndsWith(".css"))
                fileName += ".css";

            if (fileName.StartsWith("~"))
                cssPath = string.Format("<link rel='stylesheet' type='text/css' href='{0}'  media='" + media + "'/>\n", helper.AttributeEncode(fileName));
            else
                cssPath = string.Format("<link rel='stylesheet' type='text/css' href='{0}/{1}/{2}'  media='" + media + "'/>\n", GetPublicUrl(helper), cssDir, helper.AttributeEncode(fileName));
            return MvcHtmlString.Create(cssPath);
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string fileName)
        {
            return Image(helper, fileName, "");
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string fileName, string attributes)
        {
            fileName = string.Format("{0}/{1}/{2}", GetPublicUrl(helper), imageDir, fileName);
            return new MvcHtmlString(string.Format("<img src='{0}' {1} />", helper.AttributeEncode(fileName), helper.AttributeEncode(attributes)));
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string url, string altText, object htmlAttributes)
        {
            TagBuilder builder = new TagBuilder("image");
            builder.Attributes.Add("src", url);
            builder.Attributes.Add("alt", altText);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString MenuNotificationBlock(this HtmlHelper html, int stepNumber)
        {
            string template = "<span class=\"menu-notification\">{0}</span>";
            return MvcHtmlString.Create(String.Format(template, stepNumber));
        }

        #region HTML Helper Extensions

        /// <summary>
        /// Provides a HTML helper to render a label, textbox field pair.
        /// </summary>
        /// <param name="html">The helper.</param>
        /// <param name="labelExpression">the label expression</param>
        /// <param name="textBoxName">the text box name</param>
        /// <param name="textBoxValue">the textbox value</param>
        /// <param name="labelHtmlAttributes"> the label html attributes</param>
        /// <param name="textBoxHtmlAttributes">the textbox hatml attributes</param>
        /// <param name="wrapperClass">the wrapper class</param>
        /// <returns></returns>
        public static MvcHtmlString FieldPairTextBox(this HtmlHelper html, string labelExpression, object labelHtmlAttributes,
                                                     string textBoxName, object textBoxValue, object textBoxHtmlAttributes, string wrapperClass)
        {
            string template = "<div class=\"field-pair{0}\">" +
                                html.Label(labelExpression, labelHtmlAttributes).ToHtmlString() +
                                html.TextBox(textBoxName, textBoxValue, textBoxHtmlAttributes).ToHtmlString() +
                              "</div>";
            return MvcHtmlString.Create(String.Format(template, " " + wrapperClass));
        }

        public static MvcHtmlString FieldPairTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> labelExpression, object labelHtmlAttributes,
                                                                        Expression<Func<TModel, TValue>> textBoxExpression, object textBoxHtmlAttributes, string wrapperClass)
        {
            string template = "<div class=\"field-pair{0}\">" +
                                html.LabelFor(labelExpression, labelHtmlAttributes).ToHtmlString() +
                                html.TextBoxFor(textBoxExpression, textBoxHtmlAttributes).ToHtmlString() +
                              "</div>";
            return MvcHtmlString.Create(String.Format(template, " " + wrapperClass));
        }

        public static MvcHtmlString FieldPairDropDownList(this HtmlHelper html, string labelExpression, object labelHtmlAttributes,
                                                   string dropDownName, IEnumerable<SelectListItem> selectList, string optionLabel, object dropDownHtmlAttributes, string wrapperClass)
        {
            string template = "<div class=\"field-pair{0}\">" +
                                html.Label(labelExpression, labelHtmlAttributes).ToHtmlString() +
                                html.DropDownList(dropDownName, selectList, optionLabel, dropDownHtmlAttributes).ToHtmlString() +
                              "</div>";
            return MvcHtmlString.Create(String.Format(template, " " + wrapperClass));
        }

        public static MvcHtmlString FieldPairRadio(this HtmlHelper html, string radioName, object radioValue, bool isChecked, object radioHtmlAttributes,
                                                   string labelExpression, object labelHtmlAttributes, string wrapperClass)
        {
            string template = "<div class=\"field-pair{0}\">" +
                                html.RadioButton(radioName, radioValue, isChecked, radioHtmlAttributes).ToHtmlString() +
                                html.Label(labelExpression, labelHtmlAttributes).ToHtmlString() +
                              "</div>";
            return MvcHtmlString.Create(String.Format(template, " " + wrapperClass));
        }

        public static MvcHtmlString FieldPairRadioFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> radioExpression, object radioValue,
                                                                      bool isChecked, object radioHtmlAttributes, Expression<Func<TModel, TValue>> labelExpression,
                                                                      object labelHtmlAttributes, string wrapperClass)
        {
            string template = "<div class=\"field-pair{0}\">" +
                                Html.InputExtensions.RadioButtonFor(html, radioExpression, radioValue, radioHtmlAttributes).ToHtmlString() +
                                html.LabelFor(labelExpression, labelHtmlAttributes).ToHtmlString() +
                              "</div>";
            return MvcHtmlString.Create(String.Format(template, " " + wrapperClass));
        }

        //public static MvcHtmlString Label(this HtmlHelper html, string expression, object htmlAttributes)
        //{
        //    return Label(html, expression, new RouteValueDictionary(htmlAttributes));
        //}

        //public static MvcHtmlString Label(this HtmlHelper html, string expression, IDictionary<string, object> htmlAttributes)
        //{
        //    ModelMetadata metadata = ModelMetadata.FromStringExpression(expression, html.ViewData);
        //    string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
        //    string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
        //    if (String.IsNullOrEmpty(labelText))
        //    {
        //        return MvcHtmlString.Empty;
        //    }

        //    TagBuilder tag = new TagBuilder("label");
        //    tag.MergeAttributes(htmlAttributes);
        //    tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
        //    tag.SetInnerText(labelText);
        //    return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        //}

        //public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        //{
        //    return LabelFor(html, expression, new RouteValueDictionary(htmlAttributes));
        //}

        //public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        //{
        //    ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
        //    string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
        //    string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
        //    if (String.IsNullOrEmpty(labelText))
        //    {
        //        return MvcHtmlString.Empty;
        //    }

        //    TagBuilder tag = new TagBuilder("label");
        //    tag.MergeAttributes(htmlAttributes);
        //    tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
        //    tag.SetInnerText(labelText);
        //    return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        //}

        public static MvcHtmlString ToDo(this HtmlHelper helper, string text)
        {
            return MvcHtmlString.Create(String.Format("<div class=\"todo\"><strong>TO DO:</strong><br/>{0}&hellip;</div>", text));
        }

        public static MvcHtmlString GoogleStaticMap(this HtmlHelper helper, string address)
        {
            return MvcHtmlString.Create(
                String.Format("<div class=\"center\"><a href=\"http://maps.google.com/maps?q={0}\" target=\"_blank\"><img class=\"google-map\" src=\"http://maps.google.com/maps/api/staticmap?markers=color:blue|label:A|{0}&zoom=14&size=240x240&sensor=false\" alt=\"Google Map\" /></a></div>", address)
            );
        }
        #endregion

        

        public static MultiSelectList ToMultiSelectList(this Enum enumeration)
        {
            return enumeration.ToMultiSelectList(false);
        }

        public static MultiSelectList ToMultiSelectList(this Enum enumeration, bool useSelectedValues)
        {
            var values = new Dictionary<string, string>();
            foreach (Enum e in Enum.GetValues(enumeration.GetType()))
            {
                values.Add(e.ToString(), e.GetDisplayName());
            }
            MultiSelectList list;
            if (useSelectedValues)
            {
                list = new MultiSelectList(values, "key", "value", values[enumeration.ToString()]);
            }
            else
            {
                list = new MultiSelectList(values, "key", "value");
            }
            return list;
        }

        public static IDictionary<string, string> ToEnumerable(this Enum enumeration)
        {
            var values = new Dictionary<string, string>();
            foreach (Enum e in Enum.GetValues(enumeration.GetType()))
            {
                values.Add(e.GetDisplayName(), e.ToString());
            }
            return values;
        }

        public static string GetDisplayName(this Enum enumerationValue)
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of type Enum", "enumerationValue");
            }
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Any())
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Any())
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }

        public static MvcHtmlString DateTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string formatString, object htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string format = String.IsNullOrEmpty(formatString) ? "M/d/yyyy" : formatString;
            DateTime date = metadata.Model == null ? new DateTime() : DateTime.Parse(metadata.Model.ToString());
            string value = date == new DateTime() ? String.Empty : date.ToString(format);
            RouteValueDictionary attributes = new RouteValueDictionary(htmlAttributes);
            string datePickerClass = "date-selector";
            if (attributes.ContainsKey("class"))
            {
                string cssClass = attributes["class"].ToString();
                attributes["class"] = cssClass.Insert(cssClass.Length, " " + datePickerClass);
            }
            else
            {
                attributes["class"] = datePickerClass;
            }
            return helper.TextBox(ExpressionHelper.GetExpressionText(expression), value, attributes);
        }

        public static MvcHtmlString DateTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            return DateTextBoxFor<TModel, TValue>(helper, expression, String.Empty, null);
        }

        public static MvcHtmlString DateTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string formatString)
        {
            return DateTextBoxFor<TModel, TValue>(helper, expression, formatString, null);
        }

        public static MvcHtmlString DateTextBox(this HtmlHelper helper, string name, string formatString, object htmlAttributes)
        {
            string format = String.IsNullOrEmpty(formatString) ? "M/d/yyyy" : formatString;
            string value = string.Empty;
            RouteValueDictionary attributes = new RouteValueDictionary(htmlAttributes);
            string datePickerClass = "date-selector";
            if (attributes.ContainsKey("class"))
            {
                string cssClass = attributes["class"].ToString();
                attributes["class"] = cssClass.Insert(cssClass.Length, " " + datePickerClass);
            }
            else
                attributes["class"] = datePickerClass;
            return helper.TextBox(name, value, attributes);
        }

        public static MvcHtmlString DateTextBox(this HtmlHelper helper, string name)
        {
            return DateTextBox(helper, name, string.Empty, null);
        }

        public static string TruncateCharacters(this HtmlHelper helper, string text, int length)
        {
            return text.Length > length ? text.Substring(0, length - 1) + "..." : text;
        }

        public static string TruncateWords(this HtmlHelper helper, string text, int wordCount)
        {
            string output = "";

            if (text.Length > 0)
            {
                try
                {
                    string[] words = text.Split(' ');

                    if (words.Length < wordCount)
                        wordCount = words.Length;

                    for (int x = 0; x <= wordCount; x++)
                        output += words[x] + " ";

                    if (words.Length > wordCount)
                        output = output.Trim() + "...";
                }
                catch (Exception ex)
                {

                }
            }
            return output;
        }

        //public static MvcHtmlString ProfileLink(this HtmlHelper helper, Member user)
        //{
        //    var result = String.Format("{0} ({1})", helper.ActionLink(user.UserName, "Show", "Profile", new { id = user.UserName }, null).ToHtmlString(), user.FullName);
        //    return MvcHtmlString.Create(result);
        //}
    }

}
