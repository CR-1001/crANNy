/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    [Obsolete("Use generic variant: [Serializable] public class TYP : NotifyBase<TYP>")]
    [Serializable] public abstract class NotifyBase : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged<TObject>(
            TObject obj, Expression<Func<TObject, Object>> selector)
        {
            if (!(this is TObject))
            {
                throw new ArgumentException(
                    "Please always use a class instance as the first parameter, "
                    + "whose property is being changed. So normally "
                    + "NotifyPropertyChanged(this, x => x.PROPERTY)");
            }

            String propertyName = Notify.GetPropertyName<TObject>(selector);

            NotifyPropertyChanged(propertyName);
        }

        public void NotifyPropertyChanged([CallerMemberName] String info = null)
        {

#if DEBUG

            if (GetType().GetProperty(info) == null)
            {
                throw new ArgumentException(
                    GetType().Name + "." + info + " does not exists.");
            }
#endif

			var propertyChanged = PropertyChanged;

            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("{{{0}}}", GetType().FullName);

            stringBuilder.AppendLine();

            var properties
                = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && !p.GetIndexParameters().Any())
                .OrderBy(p => p.Name)
                .ToList();

            foreach(var property in properties)
            {
                var value = property.GetValue(this);
                if(value != null)
                {
                    
                    if(!(value is String) && (value is IEnumerable))
                    {
                        var index = 0;
                        foreach(var item in (IEnumerable)value)
                        {
                            stringBuilder.AppendFormat("{0}[{1}]=", property.Name, index++);
                            stringBuilder.Append(String.Format(
                                CultureInfo.InvariantCulture, 
                                "{0}", item));
                            stringBuilder.AppendLine(";");
                        }
                    }
                    else
                    {
                        stringBuilder.AppendFormat("{0}=", property.Name);
                        stringBuilder.Append(String.Format(
                            CultureInfo.InvariantCulture, 
                            "{0}", value));
                        stringBuilder.AppendLine(";");
                    }
                }
            }

            return stringBuilder.ToString();
        }

    }

    [Serializable] public abstract class NotifyBase<TObject> : NotifyBase
    {
        public void NotifyPropertyChanged(
           Expression<Func<TObject, Object>> selector)
        {
            String propertyName = Notify.GetPropertyName<TObject>(selector);

            NotifyPropertyChanged(propertyName);
        }

    }
}
