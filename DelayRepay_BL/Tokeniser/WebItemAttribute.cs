namespace DelayRepay_BL.Tokenise
{
    public class WebItemAttribute
    {
        private string _name;
        private string _content;
        private bool _gotName;
        private bool _allDone;

        public string Name { get { return _name; } }
        public string Content { get { return _content; } }
        public bool AllDone { get { return _allDone; } }

        public void AddChar(char c)
        {
            if (!_gotName && c == '=') _gotName = true;
            else
            {
                if (!_gotName)
                    _name += c.ToString();
                else
                {
                    _content += c.ToString();
                    if (c == '"' && _content!="\"")
                        _allDone = true;
                }
            }
        }
    }
}
