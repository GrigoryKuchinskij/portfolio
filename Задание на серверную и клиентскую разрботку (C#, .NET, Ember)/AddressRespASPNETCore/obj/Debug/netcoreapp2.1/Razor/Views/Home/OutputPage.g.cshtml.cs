#pragma checksum "E:\address-recognition-master\AddressRespASPNETCore\Views\Home\OutputPage.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f22ad53ded588a05479571bb04f3738700705c94"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_OutputPage), @"mvc.1.0.view", @"/Views/Home/OutputPage.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/OutputPage.cshtml", typeof(AspNetCore.Views_Home_OutputPage))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "E:\address-recognition-master\AddressRespASPNETCore\Views\_ViewImports.cshtml"
using AddressRespASPNETCore;

#line default
#line hidden
#line 2 "E:\address-recognition-master\AddressRespASPNETCore\Views\_ViewImports.cshtml"
using AddressRespASPNETCore.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f22ad53ded588a05479571bb04f3738700705c94", @"/Views/Home/OutputPage.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d9a7b3637976ef650f363285b97de5c9012ad116", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_OutputPage : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<AddressViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("action", new global::Microsoft.AspNetCore.Html.HtmlString("~/Home/InputPage"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("form-horizontal"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("role", new global::Microsoft.AspNetCore.Html.HtmlString("form"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("action", new global::Microsoft.AspNetCore.Html.HtmlString("~/Home/OutputPage"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(25, 11, true);
            WriteLiteral("<p></p>\r\n\r\n");
            EndContext();
            BeginContext(36, 214, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "a6f43c221af546a0ad9cacbf7d129cbb", async() => {
                BeginContext(118, 45, true);
                WriteLiteral("\r\n    <input type=\"hidden\" name=\"FullAddress\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 163, "\"", 194, 1);
#line 5 "E:\address-recognition-master\AddressRespASPNETCore\Views\Home\OutputPage.cshtml"
WriteAttributeValue("", 171, ViewBag.StandartOutput, 171, 23, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(195, 48, true);
                WriteLiteral(" />\r\n    <input type=\"submit\" value=\"Назад\" />\r\n");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(250, 11, true);
            WriteLiteral("\r\n<p></p>\r\n");
            EndContext();
            BeginContext(261, 890, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fe7a9a219fb0410e9d3bfc814703e985", async() => {
                BeginContext(344, 41, true);
                WriteLiteral("\r\n    <input type=\"text\" name=\"PostIndex\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 385, "\"", 411, 1);
#line 10 "E:\address-recognition-master\AddressRespASPNETCore\Views\Home\OutputPage.cshtml"
WriteAttributeValue("", 393, ViewBag.PostIndex, 393, 18, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(412, 40, true);
                WriteLiteral(" />\r\n    <input type=\"text\" name=\"State\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 452, "\"", 474, 1);
#line 11 "E:\address-recognition-master\AddressRespASPNETCore\Views\Home\OutputPage.cshtml"
WriteAttributeValue("", 460, ViewBag.State, 460, 14, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(475, 39, true);
                WriteLiteral(" />\r\n    <input type=\"text\" name=\"City\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 514, "\"", 535, 1);
#line 12 "E:\address-recognition-master\AddressRespASPNETCore\Views\Home\OutputPage.cshtml"
WriteAttributeValue("", 522, ViewBag.City, 522, 13, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(536, 41, true);
                WriteLiteral(" />\r\n    <input type=\"text\" name=\"Street\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 577, "\"", 600, 1);
#line 13 "E:\address-recognition-master\AddressRespASPNETCore\Views\Home\OutputPage.cshtml"
WriteAttributeValue("", 585, ViewBag.Street, 585, 15, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(601, 40, true);
                WriteLiteral(" />\r\n    <input type=\"text\" name=\"House\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 641, "\"", 663, 1);
#line 14 "E:\address-recognition-master\AddressRespASPNETCore\Views\Home\OutputPage.cshtml"
WriteAttributeValue("", 649, ViewBag.House, 649, 14, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(664, 42, true);
                WriteLiteral(" />\r\n    <input type=\"text\" name=\"Housing\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 706, "\"", 730, 1);
#line 15 "E:\address-recognition-master\AddressRespASPNETCore\Views\Home\OutputPage.cshtml"
WriteAttributeValue("", 714, ViewBag.Housing, 714, 16, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(731, 44, true);
                WriteLiteral(" />\r\n    <input type=\"text\" name=\"Apartment\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 775, "\"", 801, 1);
#line 16 "E:\address-recognition-master\AddressRespASPNETCore\Views\Home\OutputPage.cshtml"
WriteAttributeValue("", 783, ViewBag.Apartment, 783, 18, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(802, 43, true);
                WriteLiteral(" />\r\n    <input type=\"text\" name=\"District\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 845, "\"", 870, 1);
#line 17 "E:\address-recognition-master\AddressRespASPNETCore\Views\Home\OutputPage.cshtml"
WriteAttributeValue("", 853, ViewBag.District, 853, 17, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(871, 42, true);
                WriteLiteral(" />\r\n    <input type=\"text\" name=\"Village\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 913, "\"", 937, 1);
#line 18 "E:\address-recognition-master\AddressRespASPNETCore\Views\Home\OutputPage.cshtml"
WriteAttributeValue("", 921, ViewBag.Village, 921, 16, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(938, 82, true);
                WriteLiteral(" />\r\n    <p></p>\r\n    <p>\r\n        <input style=\"width: 70%;\" type=\"text\" readonly");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 1020, "\"", 1051, 1);
#line 21 "E:\address-recognition-master\AddressRespASPNETCore\Views\Home\OutputPage.cshtml"
WriteAttributeValue("", 1028, ViewBag.StandartOutput, 1028, 23, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(1052, 92, true);
                WriteLiteral(" />\r\n        <input type=\"submit\" value=\"Составить стандартизированный адрес\" />\r\n    </p>\r\n");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(1151, 2, true);
            WriteLiteral("\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<AddressViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
