using Prism.Mvvm;

namespace ODataTools.Infrastructure.Model
{
    public class TabContent : BindableBase
    {
        public TabContent(string header, object content)
        {
            Header = header;
            Content = content;
        }

        private string header;

        public string Header
        {
            get { return header; }
            set { this.SetProperty<string>(ref this.header, value); }
        }

        private object content;

        public object Content
        {
            get { return content; }
            set { this.SetProperty<object>(ref this.content, value); }
        }
    }
}
