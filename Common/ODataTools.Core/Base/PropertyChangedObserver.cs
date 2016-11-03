using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.Core.Base
{
    public class PropertyChangedObserver<TPropertySource> where TPropertySource : class, INotifyPropertyChanged
    {
        private readonly WeakReference<TPropertySource> propertySourceRef;
        private Dictionary<string, Action<TPropertySource>> propertyNameToHandlerMap;

        public PropertyChangedObserver(TPropertySource propertySource)
        {
            if (propertySource == null)
                throw new ArgumentNullException(nameof(propertySource));

            this.propertySourceRef = new WeakReference<TPropertySource>(propertySource);
            this.propertyNameToHandlerMap = new Dictionary<string, Action<TPropertySource>>();
        }

        public PropertyChangedObserver<TPropertySource> RegisterHandler(string propertyName, Action<TPropertySource> handler)
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            TPropertySource obj = null;

            if (this.propertySourceRef.TryGetTarget(out obj))
            {
                PropertyChangedEventManager.AddHandler(obj, PropertyChangedEventHandler, propertyName);
                this.propertyNameToHandlerMap.Add(propertyName, handler);
            }

            return this;
        }

        public PropertyChangedObserver<TPropertySource> UnregisterHandler(string propertyName)
        {
            TPropertySource obj = null;

            if (this.propertySourceRef.TryGetTarget(out obj))
            {
                this.propertyNameToHandlerMap.Remove(propertyName);

                PropertyChangedEventManager.RemoveHandler(obj, this.PropertyChangedEventHandler, propertyName);
            }

            return this;
        }

        private void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs args)
        {
            Action<TPropertySource> handler;
            TPropertySource propertySource = (TPropertySource)sender;

            if (this.propertyNameToHandlerMap.TryGetValue(args.PropertyName, out handler))
            {
                handler(propertySource);
            }
        }
    }
}
