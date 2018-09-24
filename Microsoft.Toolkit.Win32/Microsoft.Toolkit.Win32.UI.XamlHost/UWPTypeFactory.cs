// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Toolkit.Win32.UI.XamlHost
{
    public static class UWPTypeFactory
    {
        /// <summary>
        /// Creates UWP XAML type instance from WinRT type name
        /// UWP XAML type name should be specified as: namespace.class
        /// ex: MyClassLibrary.MyCustomType
        /// ex: Windows.UI.Xaml.Shapes.Rectangle
        /// ex: Windows.UI.Xaml.Controls.Button
        /// </summary>
        /// <param name="xamlTypeName">UWP XAML type name</param>
        /// <exception cref="InvalidOperationException">Condition.</exception>
        /// <returns>Instance of UWP XAML type described by xamlTypeName string</returns>
        public static Windows.UI.Xaml.FrameworkElement CreateXamlContentByType(string xamlTypeName)
        {
            Windows.UI.Xaml.Markup.IXamlType xamlType = null;
            Type systemType = null;

            // If a root metadata provider has been defined on the application object,
            // use it to probe for custom UWP XAML type metadata.  If the root metadata
            // provider has not been implemented on the current application object, assume
            // the caller wants a built-in UWP XAML type, not a custom UWP XAML type.
            var xamlRootMetadataProvider = Windows.UI.Xaml.Application.Current as Windows.UI.Xaml.Markup.IXamlMetadataProvider;
            if (xamlRootMetadataProvider != null)
            {
                xamlType = xamlRootMetadataProvider.GetXamlType(xamlTypeName);
            }

            systemType = FindBuiltInType(xamlTypeName);

            if (xamlType != null)
            {
                // Create custom UWP XAML type
                return (Windows.UI.Xaml.FrameworkElement)xamlType.ActivateInstance();
            }

            if (systemType != null)
            {
                // Create built-in UWP XAML type
                return (Windows.UI.Xaml.FrameworkElement)Activator.CreateInstance(systemType);
            }

            throw new InvalidOperationException("Microsoft.Windows.Interop.UWPTypeFactory: Could not create type: " + xamlTypeName);
        }

        /// <summary>
        /// Searches for a built-in type by iterating through all types in
        /// all assemblies loaded in the current AppDomain
        /// </summary>
        /// <param name="typeName">Full type name, with namespace, without assembly</param>
        /// <returns>If found, <see cref="Type" />; otherwise, null..</returns>
        private static Type FindBuiltInType(string typeName)
        {
            var currentAppDomain = AppDomain.CurrentDomain;
            var appDomainLoadedAssemblies = currentAppDomain.GetAssemblies();

            foreach (var loadedAssembly in appDomainLoadedAssemblies)
            {
                var currentType = loadedAssembly.GetType(typeName);
                if (currentType != null)
                {
                    return currentType;
                }
            }

            return null;
        }
    }
}
