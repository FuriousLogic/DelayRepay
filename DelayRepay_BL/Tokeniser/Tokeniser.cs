using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.HtmlControls;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;

namespace DelayRepay_BL.Tokenise
{
    public class Tokeniser
    {
        private readonly Stack<WebItem> _stack = new Stack<WebItem>();

        public List<WebItem> Tokenise(string html)
        {
            List<WebItem> page = new List<WebItem>();
            WebItem currentItem = new WebItem(false);
            bool inScript = false;

            //Loop through each character
            foreach (char c in html)
            {
                switch (c)
                {
                    case '<':
                        //Is there any content?
                        if (!String.IsNullOrEmpty(currentItem.Text) && !currentItem.IsTag && !inScript)
                            _stack.Peek().Children.Add(currentItem);

                        //Start New Item
                        currentItem = new WebItem(true);
                        break;
                    case '>':
                        //Is it a comment?
                        if (currentItem.IsComment) continue;

                        //Ignore Script
                        if (currentItem.Text.ToLower() == "script") inScript = true;
                        if (currentItem.Text.ToLower() == "/script") inScript = false;
                        if (inScript || currentItem.Text.ToLower() == "/script") continue;

                        //Is this a terminator tag?
                        if (currentItem.IsTerminatorTag)
                        {
                            WebItem topOfStack = _stack.Pop();

                            if ("/" + topOfStack.Text.ToLower() != currentItem.Text.ToLower())
                            {
                                //Possible bug, but more likely a malformed page
                                //TDHelper.Trc(TDHelper.TrcLvl.Warning, string.Format("Terminator tag {0} does not match top of stack {1}.  Ignoring terminator.", currentItem.Text, topOfStack.Text));
                                _stack.Push(topOfStack);
                                continue;
                            }

                            if (_stack.Count > 0)
                                _stack.Peek().Children.Add(topOfStack);
                            else
                                page.Add(topOfStack);

                            currentItem = new WebItem(false);
                            continue;
                        }

                        //Did this item self-terminate?
                        currentItem.CheckText();
                        if (currentItem.LastChar == '/' || currentItem.IsAutoTerminatorTag)
                            _stack.Peek().Children.Add(currentItem);
                        else
                            _stack.Push(currentItem);

                        //Close Item
                        currentItem = new WebItem(false);
                        break;
                    case '\r':
                    case '\n':
                        //Ignore These characters
                        break;
                    default:
                        currentItem.AddChar(c);
                        break;
                }
            }

            //Dump out from the stack
            //The stack should be empty, but this could be due to bugs or malformed code
            while (_stack.Count > 0)
            {
                WebItem topOfStack = _stack.Pop();
                if (_stack.Count > 0)
                    _stack.Peek().Children.Add(topOfStack);
                else
                    page.Add(topOfStack);
            }

            return page;
        }

        public static WebItem ExtractSection(WebItem parent, string tag, string attributeName, string attributeContent)
        {
            tag = tag.ToLower().Trim();
            attributeName = attributeName.ToLower().Trim();
            attributeContent = attributeContent.ToLower().Trim();

            //Is this a match
            if (parent.Text.ToLower() == tag)
            {
                if (!String.IsNullOrEmpty(attributeName))
                {
                    foreach (WebItemAttribute att in parent.Attributes)
                    {
                        if (att.Name.ToLower() == attributeName)
                        {
                            if (!String.IsNullOrEmpty(attributeContent))
                            {
                                if (att.Content.ToLower() == attributeContent)
                                    return parent;

                                //Try it with quotes
                                if (att.Content.Substring(0, 1) == "\"" && attributeContent.Substring(0, 1) != "\"")
                                {
                                    if (att.Content.ToLower() == "\"" + attributeContent + "\"")
                                        return parent;
                                }
                            }
                            else
                                return parent;
                        }
                    }

                }
                else
                    return parent;
            }

            //Search the children
            return parent.Children.Select(child => ExtractSection(child, tag, attributeName, attributeContent)).FirstOrDefault(result => result != null);
        }

    }
}
