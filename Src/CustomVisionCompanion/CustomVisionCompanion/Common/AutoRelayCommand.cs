using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomVisionCompanion.Common
{
    public class AutoRelayCommand : RelayCommand, IDisposable
    {
        private ISet<string> properties;

        public AutoRelayCommand(Action execute) : base(execute)
        {
            Initialize();
        }

        public AutoRelayCommand(Action execute, Func<bool> canExecute) : base(execute, canExecute)
        {
            Initialize();
        }

        private void Initialize()
        {
            Messenger.Default.Register<PropertyChangedMessageBase>(this, true, (property) =>
            {
                if (properties?.Contains(property.PropertyName) ?? false)
                {
                    RaiseCanExecuteChanged();
                }
            });
        }

        public AutoRelayCommand DependsOn<T>(Expression<Func<T>> propertyExpression)
            => DependsOn(GetPropertyName(propertyExpression));

        public AutoRelayCommand DependsOn(string propertyName)
        {
            if (properties == null)
            {
                properties = new HashSet<string>();
            }

            properties.Add(propertyName);
            return this;
        }

        private string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Invalid argument", nameof(propertyExpression));
            }

            var property = body.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", nameof(propertyExpression));
            }

            return property.Name;
        }

        public void Dispose()
        {
            Messenger.Default.Unregister(this);
        }
    }

    public class AutoRelayCommand<T> : RelayCommand<T>, IDisposable
    {
        private ISet<string> properties;

        public AutoRelayCommand(Action<T> execute) : base(execute)
        {
            Initialize();
        }

        public AutoRelayCommand(Action<T> execute, Func<T, bool> canExecute) : base(execute, canExecute)
        {
            Initialize();
        }

        private void Initialize()
        {
            Messenger.Default.Register<PropertyChangedMessageBase>(this, true, (property) =>
            {
                if (properties?.Contains(property.PropertyName) ?? false)
                {
                    RaiseCanExecuteChanged();
                }
            });
        }

        public AutoRelayCommand<T> DependsOn<U>(Expression<Func<U>> propertyExpression)
            => DependsOn(GetPropertyName(propertyExpression));

        public AutoRelayCommand<T> DependsOn(string propertyName)
        {
            if (properties == null)
            {
                properties = new HashSet<string>();
            }

            properties.Add(propertyName);
            return this;
        }

        private string GetPropertyName<U>(Expression<Func<U>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Invalid argument", nameof(propertyExpression));
            }

            var property = body.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", nameof(propertyExpression));
            }

            return property.Name;
        }

        public void Dispose()
        {
            Messenger.Default.Unregister(this);
        }
    }
}
