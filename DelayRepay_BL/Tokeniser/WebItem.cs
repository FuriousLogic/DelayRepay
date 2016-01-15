using System;
using System.Collections.Generic;

namespace DelayRepay_BL.Tokenise
{
    public class WebItem
    {
        private string _text = "";
        private List<WebItem> _children=new List<WebItem>() ;
        private bool _gotText;
        private bool _isComment;
        private char _lastChar;
        private bool _isAutoTerminatorTag;
        private bool _isTerminatorTag;
        private WebItemAttribute _currentAttribute=new WebItemAttribute();
        private readonly List<WebItemAttribute> _attributes = new List<WebItemAttribute>();

        public WebItem(bool isTag)
        {
            IsTag = isTag;
        }
        public void CheckText()
        {
            if (IsTag && !GotText)
            {
                _gotText = true;

                //Is this an auto-terminating tag?
                const string atTags = ",meta,link,br,img,";
                if (atTags.IndexOf("," + _text.ToLower() + ",") >= 0) _isAutoTerminatorTag = true;
            }
        }
        public void AddChar(char c)
        {
            //Have we got all the 'text'?
            if (c == ' ') CheckText();

            //Is this a 'comment'?
            if (c == '!' && _text == "") _isComment = true;

            //Is this a Terminator Tag?
            if (c == '/' && _text == "") _isTerminatorTag = true;

            //Keep building the 'text'
            if (!_gotText && !_isComment) _text += c.ToString();

            //Is it an attribute?
            if (GotText && !_isComment && ((c != ' ' && String.IsNullOrEmpty(_currentAttribute.Name)) || !String.IsNullOrEmpty(_currentAttribute.Name)))
            {
                _currentAttribute.AddChar(c);

                if (_currentAttribute.AllDone)
                {
                    _attributes.Add(_currentAttribute);
                    _currentAttribute = new WebItemAttribute();
                }
            }

            //Save the lastchar, no matter
            _lastChar = c;
        }
        public char? LastChar { get { return _lastChar; } }
        public string Text { get { return _text; } }
        public bool IsTag { get; set; }
        public List<WebItem> Children { get { return _children; } set { _children = value; } }
        public bool GotText { get { return _gotText; } }
        public bool IsComment { get { return _isComment; } }
        public bool IsAutoTerminatorTag { get { return _isAutoTerminatorTag; } }
        public bool IsTerminatorTag { get { return _isTerminatorTag; } }
        public List<WebItemAttribute> Attributes { get { return _attributes; } }
    }
}
