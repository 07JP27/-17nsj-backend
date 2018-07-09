using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace _17nsj.Jedi.Helpers
{
    [HtmlTargetElement(Attributes = "is-active-page")]
    public class ActivePageTagHelper : TagHelper
    {
        /// <summary>
        /// ページ名を取得または設定します。
        /// </summary>
        /// <value>ページ名</value>
        [HtmlAttributeName("asp-page")]
        public string Page { get; set; }

        /// <summary>
        /// リクエストを取得または設定します。
        /// </summary>
        /// <value>リクエスト</value>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="context">TagHelperContext</param>
        /// <param name="output">TagHelperOutput</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (this.ShouldBeActive())
            {
                this.MakeActive(output);
            }

            output.Attributes.RemoveAll("is-active-page");
        }

        /// <summary>
        /// アクティブページにすべきかを判定します。
        /// </summary>
        /// <returns>アクティブにすべきならTrue</returns>
        private bool ShouldBeActive()
        {
            string currentPage = this.ViewContext.RouteData.Values["Page"].ToString();

            if (!string.IsNullOrWhiteSpace(this.Page) && this.Page.ToLower() != currentPage.ToLower())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// アクティブタグを作成します。
        /// </summary>
        /// <param name="output">出力タグ</param>
        private void MakeActive(TagHelperOutput output)
        {
            var classAttr = output.Attributes.FirstOrDefault(a => a.Name == "class");
            if (classAttr == null)
            {
                // 出力タグにclassがなかったら追加
                classAttr = new TagHelperAttribute("class", "active");
                output.Attributes.Add(classAttr);
            }
            else if (classAttr.Value == null || classAttr.Value.ToString().IndexOf("active") < 0)
            {
                // classタグがあって、それの中身が空、もしくはactiveが含まれない場合にclassの中にactiveを追加
                output.Attributes.SetAttribute("class", classAttr.Value == null ? "active" : classAttr.Value.ToString() + " active");
            }
        }
    }
}
