﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Android.Content;
using Xamarin.Forms.Platform.Android;

namespace AiForms.Renderers.Droid
{
    /// <summary>
    /// Cell base renderer.
    /// </summary>
    public class CellBaseRenderer<TnativeCell> : CellRenderer where TnativeCell : CellBaseView
    {
        internal static class InstanceCreator<T1, T2, TInstance>
        {
            public static Func<T1, T2, TInstance> Create { get; } = CreateInstance();

            private static Func<T1, T2, TInstance> CreateInstance()
            {
                var argsTypes = new[] { typeof(T1), typeof(T2) };
                var constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder,
                       argsTypes, null);
                var args = argsTypes.Select(Expression.Parameter).ToArray();
                return Expression.Lambda<Func<T1, T2, TInstance>>(Expression.New(constructor, args), args).Compile();
            }
        }

        /// <summary>
        /// Gets the cell core.
        /// </summary>
        /// <returns>The cell core.</returns>
        /// <param name="item">Item.</param>
        /// <param name="convertView">Convert view.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="context">Context.</param>
        protected override Android.Views.View GetCellCore(Xamarin.Forms.Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context)
        {
            TnativeCell nativeCell = convertView as TnativeCell;
            if (nativeCell == null) {
                nativeCell = InstanceCreator<Context, Xamarin.Forms.Cell, TnativeCell>.Create(context, item);
            }

            nativeCell.Cell = item;

            SetUpPropertyChanged(nativeCell);

            nativeCell.UpdateCell();

            return nativeCell;
        }

        /// <summary>
        /// Sets up property changed.
        /// </summary>
        /// <param name="nativeCell">Native cell.</param>
        protected void SetUpPropertyChanged(CellBaseView nativeCell)
        {
            var formsCell = nativeCell.Cell as CellBase;
            var parentElement = formsCell.Parent as SettingsView;

            formsCell.PropertyChanged -= nativeCell.CellPropertyChanged;
            formsCell.PropertyChanged += nativeCell.CellPropertyChanged;

            if (parentElement != null) {
                parentElement.PropertyChanged -= nativeCell.ParentPropertyChanged;
                parentElement.PropertyChanged += nativeCell.ParentPropertyChanged;
            }
        }

    }
}
