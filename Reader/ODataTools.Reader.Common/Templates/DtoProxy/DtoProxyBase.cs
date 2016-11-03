using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DtoProxy
{
    public abstract class DtoProxyBase<T> : BindableBase, IRevertibleChangeTracking
    {
        // Dictionary for the original values
        private Dictionary<string, object> originalValues = null;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="model">The model.</param>
        public DtoProxyBase(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            this.originalValues = new Dictionary<string, object>();

            this.Model = model;
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="TValue">Type of the property.</typeparam>
        /// <param name="newValue">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the desired value.</returns>
        protected virtual bool SetProperty<TValue>(TValue newValue, [CallerMemberName] string propertyName = null)
        {
            // Get current value
            Type modelType = typeof(T);
            PropertyInfo pi = modelType.GetProperty(propertyName);

            object currentValue = pi.GetValue(Model);

            if (Equals(currentValue, newValue))
                return false;

            // Update orignal value in change tracking dictionary
            UpdateOriginalValue(currentValue, newValue, propertyName);

            // Set new value
            pi.SetValue(Model, newValue);

            // Raise PropertyChangedEvent
            this.OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Returns the property value
        /// </summary>
        /// <typeparam name="TValue">Type of the property.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected TValue GetProperty<TValue>([CallerMemberName] string propertyName = null)
        {
            Type modelType = typeof(T);
            PropertyInfo pi = modelType.GetProperty(propertyName);

            return (TValue)pi.GetValue(Model);
        }

        /// <summary>
        /// The wrapped model
        /// </summary>
        public T Model { get; private set; }

        #region Interface IRevertibleChangeTracking

        /// <summary>
        /// Resets the object’s state to unchanged by rejecting the modifications.
        /// </summary>
        public void RejectChanges()
        {
            Type modelType = typeof(T);

            foreach (var orignialValue in originalValues)
            {
                PropertyInfo pi = modelType.GetProperty(orignialValue.Key);
                pi.SetValue(Model, orignialValue.Value);
                OnPropertyChanged(orignialValue.Key);
            }

            this.originalValues.Clear();

            OnPropertyChanged(nameof(IsChanged));
        }

        /// <summary>
        /// Resets the object’s state to unchanged by accepting the modifications.
        /// </summary>
        public void AcceptChanges()
        {
            //foreach(var orgValue in originalValues)
            //{
            //    OnPropertyChanged(orgValue.Key);
            //}

            this.originalValues.Clear();

            OnPropertyChanged(nameof(IsChanged));
        }

        /// <summary>
        /// Gets the object's changed status.
        /// </summary>
        public bool IsChanged => originalValues.Count > 0;

        /// <summary>
        /// Update the original value of the property in the change tracking dictionary
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The property name.</param>
        private void UpdateOriginalValue(object currentValue, object newValue, string propertyName)
        {
            if (!originalValues.ContainsKey(propertyName))
            {
                originalValues.Add(propertyName, currentValue);
                OnPropertyChanged(nameof(IsChanged));
            }
            else
            {
                if (Equals(originalValues[propertyName], newValue))
                {
                    originalValues.Remove(propertyName);
                    OnPropertyChanged(nameof(IsChanged));
                }
            }
        }

        /// <summary>
        /// Get original value of the property
        /// </summary>
        /// <typeparam name="TValue">Type of the property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <returns></returns>
        public TValue GetOriginalValue<TValue>(string propertyName)
        {
            return originalValues.ContainsKey(propertyName)
              ? (TValue)originalValues[propertyName]
              : GetProperty<TValue>(propertyName);
        }

        /// <summary>
        /// Checks if the given property is changed
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns></returns>
        public bool GetIsChanged(string propertyName)
        {
            return originalValues.ContainsKey(propertyName);
        }

        #endregion Interface IRevertibleChangeTracking
    }
}