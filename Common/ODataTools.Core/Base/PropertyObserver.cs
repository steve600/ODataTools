using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ODataTools.Core.Base
{
    public class PropertyObserver<T> where T : class, INotifyPropertyChanged
    {
        private readonly WeakReference<T> propertySourceRef;
        private Dictionary<string, Action<T>> propertyNameToHandlerMap;

        private readonly string eventName = "NotifyPropertyChanged";

        public PropertyObserver(T propertySource)
        {
            if (propertySource == null)
                throw new ArgumentNullException("propertySource");

            this.propertySourceRef = new WeakReference<T>(propertySource);
            this.propertyNameToHandlerMap = new Dictionary<string, Action<T>>();
        }

        public void RegisterHandler(string propertyName, Action<T> handler)
        {
            T obj = null;

            if (this.propertySourceRef.TryGetTarget(out obj))
            {
                WeakEventManager<T, PropertyChangedEventArgs>.AddHandler(obj, eventName, this.PropertyChangedEventHandler);
                this.propertyNameToHandlerMap.Add(propertyName, handler);
            }
        }

        public void UnregisterHandler(string propertyName)
        {
            T obj = null;

            if (this.propertySourceRef.TryGetTarget(out obj))
            {
                WeakEventManager<T, PropertyChangedEventArgs>.RemoveHandler(obj, eventName, this.PropertyChangedEventHandler);
                this.propertyNameToHandlerMap.Remove(propertyName);

                PropertyChangedEventManager.AddHandler(obj, this.PropertyChangedEventHandler, propertyName);
            }
        }

        private void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs args)
        {
            Action<T> handler;
            T propertySource = (T)sender;

            if (this.propertyNameToHandlerMap.TryGetValue(args.PropertyName, out handler))
            {
                handler(propertySource);
            }
        }
    }
}
