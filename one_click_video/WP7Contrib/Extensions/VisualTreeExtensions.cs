// --------------------------------------------------------------------------------------------------------------------
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
// <Credits>
// Peter Torr
// http://blogs.msdn.com/b/ptorr/archive/2010/10/12/procrastination-ftw-lazylistbox-should-improve-your-scrolling-performance-and-responsiveness.aspx
// </Credits>
// --------------------------------------------------------------------------------------------------------------------

namespace WP7Contrib.View.Controls.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows;
    using System.Collections;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Provides useful extensions for working with the visual tree.
    /// </summary>
    /// <remarks>
    /// Since many of these extension methods are declared on types like
    /// DependencyObject high up in the class hierarchy, we've placed them in
    /// the Primitives namespace which is less likely to be imported for normal
    /// scenarios.
    /// </remarks>
    /// <QualityBand>Experimental</QualityBand>
    public static class VisualTreeExtensions
    {
        /// <summary>
        /// Get the visual tree ancestors of an element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The visual tree ancestors of the element.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static IEnumerable<DependencyObject> GetVisualAncestors(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return GetVisualAncestorsAndSelfIterator(element).Skip(1);
        }

        /// <summary>
        /// Get the visual tree ancestors of an element and the element itself.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The visual tree ancestors of an element and the element itself.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static IEnumerable<DependencyObject> GetVisualAncestorsAndSelf(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return GetVisualAncestorsAndSelfIterator(element);
        }

        /// <summary>
        /// Get the visual tree ancestors of an element and the element itself.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The visual tree ancestors of an element and the element itself.
        /// </returns>
        private static IEnumerable<DependencyObject> GetVisualAncestorsAndSelfIterator(DependencyObject element)
        {
            Debug.Assert(element != null, "element should not be null!");

            for (DependencyObject obj = element;
                    obj != null;
                    obj = VisualTreeHelper.GetParent(obj))
            {
                yield return obj;
            }
        }

        /// <summary>
        /// Gets the visual children of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetVisualChildren<T>(this DependencyObject target)
            where T : DependencyObject
        {
            return GetVisualChildren(target).Where(child => child is T).Cast<T>();
        }

        /// <summary>
        /// Gets the visual children of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <param name="strict">if set to <c>true</c> [strict].</param>
        /// <returns></returns>
        public static IEnumerable<T> GetVisualChildren<T>(this DependencyObject target, bool strict)
            where T : DependencyObject
        {
            return GetVisualChildren(target, strict).Where(child => child is T).Cast<T>();
        }

        /// <summary>
        /// Gets the visual children.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="strict">Prevents the search from navigating the logical tree; eg. ContentControl.Content</param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject target, bool strict)
        {
            int count = VisualTreeHelper.GetChildrenCount(target);
            if (count == 0)
            {
                if (!strict && target is ContentControl)
                {
                    var child = ((ContentControl)target).Content as DependencyObject;
                    if (child != null)
                    {
                        yield return child;
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    yield return VisualTreeHelper.GetChild(target, i);
                }
            }

            yield break;
        }

        /// <summary>
        /// Get the visual tree children of an element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The visual tree children of an element.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return GetVisualChildrenAndSelfIterator(element).Skip(1);
        }

        /// <summary>
        /// Get the visual tree children of an element and the element itself.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The visual tree children of an element and the element itself.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static IEnumerable<DependencyObject> GetVisualChildrenAndSelf(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return GetVisualChildrenAndSelfIterator(element);
        }

        /// <summary>
        /// Get the visual tree children of an element and the element itself.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The visual tree children of an element and the element itself.
        /// </returns>
        private static IEnumerable<DependencyObject> GetVisualChildrenAndSelfIterator(this DependencyObject element)
        {
            Debug.Assert(element != null, "element should not be null!");

            yield return element;

            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                yield return VisualTreeHelper.GetChild(element, i);
            }
        }

        /// <summary>
        /// A helper method used to get visual decnedants using a breadth-first strategy.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="strict">Prevents the search from navigating the logical tree; eg. ContentControl.Content</param>
        /// <param name="queue"></param>
        /// <returns></returns>
        private static IEnumerable<DependencyObject> GetVisualDecendants(DependencyObject target, bool strict, Queue<DependencyObject> queue)
        {
            foreach (var child in GetVisualChildren(target, strict))
            {
                queue.Enqueue(child);
            }

            if (queue.Count == 0)
            {
                yield break;
            }

            DependencyObject node = queue.Dequeue();
            yield return node;

            foreach (var decendant in GetVisualDecendants(node, strict, queue))
            {
                yield return decendant;
            }
        }

        /// <summary>
        /// A helper method used to get visual decnedants using a depth-first strategy.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="strict">Prevents the search from navigating the logical tree; eg. ContentControl.Content</param>
        /// <param name="stack"></param>
        /// <returns></returns>
        private static IEnumerable<DependencyObject> GetVisualDecendants(DependencyObject target, bool strict, Stack<DependencyObject> stack)
        {
            foreach (var child in GetVisualChildren(target, strict))
            {
                stack.Push(child);
            }

            if (stack.Count == 0)
            {
                yield break;
            }

            DependencyObject node = stack.Pop();
            yield return node;

            foreach (var decendant in GetVisualDecendants(node, strict, stack))
            {
                yield return decendant;
            }
        }

        /// <summary>
        /// Get the visual tree descendants of an element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The visual tree descendants of an element.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return GetVisualDescendantsAndSelfIterator(element).Skip(1);
        }

        /// <summary>
        /// Get the visual tree descendants of an element and the element
        /// itself.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The visual tree descendants of an element and the element itself.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static IEnumerable<DependencyObject> GetVisualDescendantsAndSelf(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return GetVisualDescendantsAndSelfIterator(element);
        }

        /// <summary>
        /// Get the visual tree descendants of an element and the element
        /// itself.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The visual tree descendants of an element and the element itself.
        /// </returns>
        private static IEnumerable<DependencyObject> GetVisualDescendantsAndSelfIterator(DependencyObject element)
        {
            Debug.Assert(element != null, "element should not be null!");

            var remaining = new Queue<DependencyObject>();
            remaining.Enqueue(element);

            while (remaining.Count > 0)
            {
                DependencyObject obj = remaining.Dequeue();
                yield return obj;

                foreach (DependencyObject child in obj.GetVisualChildren())
                {
                    remaining.Enqueue(child);
                }
            }
        }

        /// <summary>
        /// Get the visual tree siblings of an element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The visual tree siblings of an element.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static IEnumerable<DependencyObject> GetVisualSiblings(this DependencyObject element)
        {
            return element
                .GetVisualSiblingsAndSelf()
                .Where(p => p != element);
        }

        /// <summary>
        /// Get the visual tree siblings of an element and the element itself.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The visual tree siblings of an element and the element itself.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        public static IEnumerable<DependencyObject> GetVisualSiblingsAndSelf(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            DependencyObject parent = VisualTreeHelper.GetParent(element);
            return parent == null ?
                Enumerable.Empty<DependencyObject>() :
                parent.GetVisualChildren();
        }

        /// <summary>
        /// Get the bounds of an element relative to another element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="otherElement">
        /// The element relative to the other element.
        /// </param>
        /// <returns>
        /// The bounds of the element relative to another element, or null if
        /// the elements are not related.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="otherElement"/> is null.
        /// </exception>
        public static Rect? GetBoundsRelativeTo(this FrameworkElement element, UIElement otherElement)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (otherElement == null)
            {
                throw new ArgumentNullException("otherElement");
            }

            try
            {
                Point origin, bottom;
                GeneralTransform transform = element.TransformToVisual(otherElement);
                if (transform != null &&
                    transform.TryTransform(new Point(), out origin) &&
                    transform.TryTransform(new Point(element.ActualWidth, element.ActualHeight), out bottom))
                {
                    return new Rect(origin, bottom);
                }
            }
            catch (ArgumentException)
            {
                // Ignore any exceptions thrown while trying to transform
            }

            return null;
        }

        /// <summary>
        /// Finds the visual child.
        /// </summary>
        /// <typeparam name="TChildItem">The type of the hild item.</typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static TChildItem FindVisualChild<TChildItem>(DependencyObject obj) where TChildItem : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is TChildItem)
                {
                    return (TChildItem)child;
                }
                
                var childOfChild = FindVisualChild<TChildItem>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }

            return null;
        }

        /// <summary>
        /// Equivalent of FindName, but works on the visual tree to go through templates, etc.
        /// </summary>
        /// <param name="root">The node to search from</param>
        /// <param name="name">The name to look for</param>
        /// <returns>The found node, or null if not found</returns>
        public static FrameworkElement FindVisualChild(this FrameworkElement root, string name)
        {
            var temp = root.FindName(name) as FrameworkElement;
            if (temp != null)
            {
                return temp;
            }

            foreach (FrameworkElement element in root.GetVisualChildren())
            {
                temp = element.FindName(name) as FrameworkElement;
                if (temp != null)
                {
                    return temp;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the visuals.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetVisuals(this DependencyObject root)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                yield return child;
                foreach (var descendants in child.GetVisuals())
                {
                    yield return descendants;
                }
            }
        }

        /// <summary>
        /// Gets a visual child of the element
        /// </summary>
        /// <param name="node">The element to check</param>
        /// <param name="index">The index of the child</param>
        /// <returns>The found child</returns>
        public static FrameworkElement GetVisualChild(this FrameworkElement node, int index)
        {
            return VisualTreeHelper.GetChild(node, index) as FrameworkElement;
        }

        /// <summary>
        /// Gets the visual parent of the element
        /// </summary>
        /// <param name="node">The element to check</param>
        /// <returns>The visual parent</returns>
        public static FrameworkElement GetVisualParent(this FrameworkElement node)
        {
            return VisualTreeHelper.GetParent(node) as FrameworkElement;
        }

        /// <summary>
        /// Gets the VisualStateGroup with the given name, looking up the visual tree
        /// </summary>
        /// <param name="root">Element to start from</param>
        /// <param name="groupName">Name of the group to look for</param>
        /// <param name="searchAncestors">Whether or not to look up the tree</param>
        /// <returns>The group, if found</returns>
        public static VisualStateGroup GetVisualStateGroup(this FrameworkElement root, string groupName, bool searchAncestors)
        {
            IList groups = VisualStateManager.GetVisualStateGroups(root);
            foreach (object o in groups)
            {
                var group = o as VisualStateGroup;
                if (group != null && group.Name == groupName)
                {
                    return group;
                }
            }

            if (searchAncestors)
            {
                var parent = root.GetVisualParent();
                if (parent != null)
                {
                    return parent.GetVisualStateGroup(groupName, true);
                }
            }

            return null;
        }

        /// <summary>
        /// Provides a debug string that represents the visual child tree
        /// </summary>
        /// <param name="root">The root node</param>
        /// <param name="result">StringBuilder into which the text is appended</param>
        /// <remarks>This method only works in DEBUG mode</remarks>
        [Conditional("DEBUG")]
        public static void GetVisualChildTreeDebugText(this FrameworkElement root, StringBuilder result)
        {
            var results = new List<string>();
            root.GetChildTree(string.Empty, "  ", results);
            foreach (var s in results)
            {
                result.AppendLine(s);
            }
        }

        /// <summary>
        /// Gets the child tree.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="addPrefix">The add prefix.</param>
        /// <param name="results">The results.</param>
        private static void GetChildTree(this FrameworkElement root, string prefix, string addPrefix, List<string> results)
        {
            string thisElement;
            if (String.IsNullOrEmpty(root.Name))
            {
                thisElement = "[Anonymous]";
            }
            else
            {
                thisElement = "[" + root.Name + "]";
            }

            thisElement += " : " + root.GetType().Name;

            results.Add(prefix + thisElement);
            foreach (FrameworkElement directChild in root.GetVisualChildren())
            {
                directChild.GetChildTree(prefix + addPrefix, addPrefix, results);
            }
        }

        /// <summary>
        /// Provides a debug string that represents the visual child tree
        /// </summary>
        /// <param name="node">The root node</param>
        /// <param name="result">StringBuilder into which the text is appended</param>
        /// <remarks>This method only works in DEBUG mode</remarks>
        [Conditional("DEBUG")]
        public static void GetAncestorVisualTreeDebugText(this FrameworkElement node, StringBuilder result)
        {
            var tree = new List<string>();
            node.GetAncestorVisualTree(tree);
            var prefix = string.Empty;
            foreach (var s in tree)
            {
                result.AppendLine(prefix + s);
                prefix = prefix + "  ";
            }
        }

        /// <summary>
        /// Gets the ancestor visual tree.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="children">The children.</param>
        private static void GetAncestorVisualTree(this FrameworkElement node, List<string> children)
        {
            var name = String.IsNullOrEmpty(node.Name) ? "[Anon]" : node.Name;
            var thisNode = name + ": " + node.GetType().Name;

            // Ensure list is in reverse order going up the tree
            children.Insert(0, thisNode);
            FrameworkElement parent = node.GetVisualParent();
            if (parent != null)
            {
                GetAncestorVisualTree(parent, children);
            }
        }

        /// <summary>
        /// Returns a render transform of the specified type from the element, creating it if necessary
        /// </summary>
        /// <typeparam name="TRequestedTransform">The type of transform (Rotate, Translate, etc)</typeparam>
        /// <param name="element">The element to check</param>
        /// <param name="mode">The mode to use for creating transforms, if not found</param>
        /// <returns>The specified transform, or null if not found and not created</returns>
        public static TRequestedTransform GetTransform<TRequestedTransform>(this UIElement element, TransformCreationMode mode) where TRequestedTransform : Transform, new()
        {
            //if (element == null)
            //    return null;

            Transform originalTransform = element.RenderTransform;
            TRequestedTransform requestedTransform = null;
            MatrixTransform matrixTransform = null;
            TransformGroup transformGroup = null;

            // Current transform is null -- create if necessary and return
            if (originalTransform == null)
            {
                if ((mode & TransformCreationMode.Create) == TransformCreationMode.Create)
                {
                    requestedTransform = new TRequestedTransform();
                    element.RenderTransform = requestedTransform;
                    return requestedTransform;
                }

                return null;
            }

            // Transform is exactly what we want -- return it
            requestedTransform = originalTransform as TRequestedTransform;
            if (requestedTransform != null)
                return requestedTransform;


            // The existing transform is matrix transform - overwrite if necessary and return
            matrixTransform = originalTransform as MatrixTransform;
            if (matrixTransform != null)
            {
                if (matrixTransform.Matrix.IsIdentity
                  && (mode & TransformCreationMode.Create) == TransformCreationMode.Create
                  && (mode & TransformCreationMode.IgnoreIdentityMatrix) == TransformCreationMode.IgnoreIdentityMatrix)
                {
                    requestedTransform = new TRequestedTransform();
                    element.RenderTransform = requestedTransform;
                    return requestedTransform;
                }

                return null;
            }

            // Transform is actually a group -- check for the requested type
            transformGroup = originalTransform as TransformGroup;
            if (transformGroup != null)
            {
                foreach (Transform child in transformGroup.Children)
                {
                    // Child is the right type -- return it
                    if (child is TRequestedTransform)
                        return child as TRequestedTransform;
                }

                // Right type was not found, but we are OK to add it
                if ((mode & TransformCreationMode.AddToGroup) == TransformCreationMode.AddToGroup)
                {
                    requestedTransform = new TRequestedTransform();
                    transformGroup.Children.Add(requestedTransform);
                    return requestedTransform;
                }

                return null;
            }

            // Current ransform is not a group and is not what we want;
            // create a new group containing the existing transform and the new one
            if ((mode & TransformCreationMode.CombineIntoGroup) == TransformCreationMode.CombineIntoGroup)
            {
                transformGroup = new TransformGroup();
                transformGroup.Children.Add(originalTransform);
                transformGroup.Children.Add(requestedTransform);
                element.RenderTransform = transformGroup;
                return requestedTransform;
            }

            Debug.Assert(false, "Shouldn't get here");
            return null;
        }

        /// <summary>
        /// Returns a string representation of a property path needed to update a Storyboard
        /// </summary>
        /// <param name="element">The element to get the path for</param>
        /// <param name="subProperty">The property of the transform</param>
        /// <typeparam name="TRequestedType">The type of transform to look fo</typeparam>
        /// <returns>A property path</returns>
        public static string GetTransformPropertyPath<TRequestedType>(this FrameworkElement element, string subProperty) where TRequestedType : Transform
        {
            Transform t = element.RenderTransform;
            if (t is TRequestedType)
            {
                return String.Format("(RenderTransform).({0}.{1})", typeof(TRequestedType).Name, subProperty);
            }

            if (t is TransformGroup)
            {
                TransformGroup g = t as TransformGroup;
                for (int i = 0; i < g.Children.Count; i++)
                {
                    if (g.Children[i] is TRequestedType)
                    {
                        return String.Format(
                            "(RenderTransform).(TransformGroup.Children)[" + i + "].({0}.{1})",
                            typeof(TRequestedType).Name,
                            subProperty);
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns a plane projection, creating it if necessary
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="create">Whether or not to create the projection if it doesn't already exist</param>
        /// <returns>The plane project, or null if not found / created</returns>
        public static PlaneProjection GetPlaneProjection(this UIElement element, bool create)
        {
            Projection originalProjection = element.Projection;
            PlaneProjection projection = null;

            // Projection is already a plane projection; return it
            if (originalProjection is PlaneProjection)
            {
                return originalProjection as PlaneProjection;
            }

            // Projection is null; create it if necessary
            if (originalProjection == null)
            {
                if (create)
                {
                    projection = new PlaneProjection();
                    element.Projection = projection;
                }
            }

            // Note that if the project is a Matrix projection, it will not be
            // changed and null will be returned.
            return projection;
        }

        /// <summary>
        /// Perform an action when the element's LayoutUpdated event fires.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="action">The action to perform.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="action"/> is null.
        /// </exception>
        public static void InvokeOnLayoutUpdated(this FrameworkElement element, Action action)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            // Create an event handler that unhooks itself before calling the
            // action and then attach it to the LayoutUpdated event.
            EventHandler handler = null;
            handler = (s, e) =>
            {
                //TODO: is this the right thing to do?
                //Deployment.Current.Dispatcher.BeginInvoke(() => { element.LayoutUpdated -= handler; });
                element.LayoutUpdated -= handler;

                action();
            };
            element.LayoutUpdated += handler;
        }

        /// <summary>
        /// Retrieves all the logical children of a framework element using a 
        /// breadth-first search. For performance reasons this method manually 
        /// manages the stack instead of using recursion.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <returns>The logical children of the framework element.</returns>
        internal static IEnumerable<FrameworkElement> GetLogicalChildren(this FrameworkElement parent)
        {
            Debug.Assert(parent != null, "The parent cannot be null.");

            Popup popup = parent as Popup;
            if (popup != null)
            {
                FrameworkElement popupChild = popup.Child as FrameworkElement;
                if (popupChild != null)
                {
                    yield return popupChild;
                }
            }

            // If control is an items control return all children using the 
            // Item container generator.
            ItemsControl itemsControl = parent as ItemsControl;
            if (itemsControl != null)
            {
                foreach (FrameworkElement logicalChild in
                    Enumerable
                        .Range(0, itemsControl.Items.Count)
                        .Select(index => itemsControl.ItemContainerGenerator.ContainerFromIndex(index))
                        .OfType<FrameworkElement>())
                {
                    yield return logicalChild;
                }
            }

            var parentName = parent.Name;
            Queue<FrameworkElement> queue =
                new Queue<FrameworkElement>(parent.GetVisualChildren().OfType<FrameworkElement>());

            while (queue.Count > 0)
            {
                FrameworkElement element = queue.Dequeue();
                if (element.Parent == parent || element is UserControl)
                {
                    yield return element;
                }
                else
                {
                    foreach (FrameworkElement visualChild in element.GetVisualChildren().OfType<FrameworkElement>())
                    {
                        queue.Enqueue(visualChild);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves all the logical descendents of a framework element using a 
        /// breadth-first search. For performance reasons this method manually 
        /// manages the stack instead of using recursion.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <returns>The logical children of the framework element.</returns>
        internal static IEnumerable<FrameworkElement> GetLogicalDescendents(this FrameworkElement parent)
        {
            Debug.Assert(parent != null, "The parent cannot be null.");

            //return
            //    FunctionalProgramming.TraverseBreadthFirst(
            //        parent,
            //        node => node.GetLogicalChildren(),
            //        node => true);

            return null;
        }

        /// <summary>
        /// Gets all the visual children of the element
        /// </summary>
        /// <param name="root">The element to get children of</param>
        /// <returns>An enumerator of the children</returns>
        public static IEnumerable<FrameworkElement> GetVisualChildren(this FrameworkElement root)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
                yield return VisualTreeHelper.GetChild(root, i) as FrameworkElement;
        }

        /// <summary>
        /// Gets the ancestors of the element, up to the root
        /// </summary>
        /// <param name="node">The element to start from</param>
        /// <returns>An enumerator of the ancestors</returns>
        public static IEnumerable<FrameworkElement> GetVisualAncestors(this FrameworkElement node)
        {
            FrameworkElement parent = node.GetVisualParent();
            while (parent != null)
            {
                yield return parent;
                parent = parent.GetVisualParent();
            }
        }

        /// <summary>
        /// Prepends an item to the beginning of an enumeration
        /// </summary>
        /// <typeparam name="T">The type of item in the enumeration</typeparam>
        /// <param name="list">The existing enumeration</param>
        /// <param name="head">The item to return before the enumeration</param>
        /// <returns>An enumerator that returns the head, followed by the rest of the list</returns>
        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> list, T head)
        {
            yield return head;
            foreach (T item in list)
                yield return item;
        }

        /// <summary>
        /// Gets the VisualStateGroup with the given name, looking up the visual tree
        /// </summary>
        /// <param name="root">Element to start from</param>
        /// <param name="groupName">Name of the group to look for</param>
        /// <returns>The group, if found, or null</returns>
        public static VisualStateGroup GetVisualStateGroup(this FrameworkElement root, string groupName)
        {
            IEnumerable<FrameworkElement> selfOrAncestors = root.GetVisualAncestors().PrependWith(root);

            foreach (FrameworkElement element in selfOrAncestors)
            {
                IList groups = VisualStateManager.GetVisualStateGroups(element);
                foreach (object o in groups)
                {
                    VisualStateGroup group = o as VisualStateGroup;
                    if (group != null && group.Name == groupName)
                        return group;
                }
            }

            return null;
        }

        /// <summary>
        /// Tests if the given item is visible or not inside a given viewport
        /// </summary>
        /// <param name="item">The item to check for visibility</param>
        /// <param name="viewport">The viewport to check visibility within</param>
        /// <param name="orientation">The orientation to check visibility with respect to (vertical or horizontal)</param>
        /// <param name="wantVisible">Whether the test is for being visible or invisible</param>
        /// <returns>True if the item's visibility matches the wantVisible parameter</returns>
        public static bool TestVisibility(this FrameworkElement item, FrameworkElement viewport, Orientation orientation, bool wantVisible)
        {
            try
            {
                // Determine the bounding box of the item relative to the viewport
                GeneralTransform transform = item.TransformToVisual(viewport);
                Point topLeft = transform.Transform(new Point(0, 0));
                Point bottomRight = transform.Transform(new Point(item.ActualWidth, item.ActualHeight));

                // Check for overlapping bounding box of the item vs. the viewport, depending on orientation
                double min, max, testMin, testMax;
                if (orientation == Orientation.Vertical)
                {
                    min = topLeft.Y;
                    max = bottomRight.Y;
                    testMin = 0;
                    testMax = Math.Min(viewport.ActualHeight, double.IsNaN(viewport.Height) ? double.PositiveInfinity : viewport.Height);
                }
                else
                {
                    min = topLeft.X;
                    max = bottomRight.X;
                    testMin = 0;
                    testMax = Math.Min(viewport.ActualWidth, double.IsNaN(viewport.Width) ? double.PositiveInfinity : viewport.Width);
                }

                bool result = wantVisible;

                if (min >= testMax || max <= testMin)
                    result = !wantVisible;

#if false
      // Enable this to help with debugging if you are having issues...
      Debug.WriteLine(String.Format("Test visibility of {0}-{1} inside {2}-{3}. wantVisible {4}, result {5}",
        min, max, testMin, testMax, wantVisible, result));
#endif

                return result;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the items that are visible in a given container.
        /// </summary>
        /// <remarks>This function assumes that items are ordered top-to-bottom or left-to-right; if items are in random positions it won't work</remarks>
        /// <typeparam name="T">The type of items being tested</typeparam>
        /// <param name="items">The items being tested; typically the children of a StackPanel</param>
        /// <param name="viewport">The viewport to test visibility against; typically a ScrollViewer</param>
        /// <param name="orientation">Whether to check for vertical or horizontal visibility</param>
        /// <returns>The items that are (at least partially) visible</returns>
        public static IEnumerable<T> GetVisibleItems<T>(this IEnumerable<T> items, FrameworkElement viewport, Orientation orientation) where T : FrameworkElement
        {
            // Skip over the non-visible items, then take the visible items
            var skippedOverBeforeItems = items.SkipWhile((item) => item.TestVisibility(viewport, orientation, false));
            var keepOnlyVisibleItems = skippedOverBeforeItems.TakeWhile((item) => item.TestVisibility(viewport, orientation, true));
            return keepOnlyVisibleItems;
        }

        /// <summary>
        /// Returns the items that are visible in a given container plus the invisible ones before and after.
        /// </summary>
        /// <remarks>This function assumes that items are ordered top-to-bottom or left-to-right; if items are in random positions it won't work</remarks>
        /// <typeparam name="T">The type of items being tested</typeparam>
        /// <param name="items">The items being tested; typically the children of a StackPanel</param>
        /// <param name="viewport">The viewport to test visibility against; typically a ScrollViewer</param>
        /// <param name="orientation">Wether to check for vertical or horizontal visibility</param>
        /// <param name="beforeItems">List to be populated with items that precede the visible items</param>
        /// <param name="visibleItems">List to be populated with the items that are visible</param>
        /// <param name="afterItems">List to be populated with the items that follow the visible items</param>
        public static void GetVisibleItems<T>(this IEnumerable<T> items, FrameworkElement viewport, Orientation orientation, out List<T> beforeItems, out List<T> visibleItems, out List<T> afterItems) where T : FrameworkElement
        {
            beforeItems = new List<T>();
            visibleItems = new List<T>();
            afterItems = new List<T>();

            VisibleSearchMode mode = VisibleSearchMode.Before;

            // Use a state machine to go over the enumertaion and populate the lists
            foreach (var item in items)
            {
                switch (mode)
                {
                    case VisibleSearchMode.Before:
                        if (item.TestVisibility(viewport, orientation, false))
                        {
                            beforeItems.Add(item);
                        }
                        else
                        {
                            visibleItems.Add(item);
                            mode = VisibleSearchMode.During;
                        }
                        break;

                    case VisibleSearchMode.During:
                        if (item.TestVisibility(viewport, orientation, true))
                        {
                            visibleItems.Add(item);
                        }
                        else
                        {
                            afterItems.Add(item);
                            mode = VisibleSearchMode.After;
                        }
                        break;

                    default:
                        afterItems.Add(item);
                        break;
                }
            }
        }

        /// <summary>
        /// Simple enumeration used in the state machine of GetVisibleItems
        /// </summary>
        enum VisibleSearchMode
        {
            Before,
            During,
            After
        }


        /// <summary>
        /// Performs a breadth-first enumeration of all the descendents in the tree
        /// </summary>
        /// <param name="root">The root node</param>
        /// <returns>An enumerator of all the children</returns>
        public static IEnumerable<FrameworkElement> GetVisualDescendents(this FrameworkElement root)
        {
            Queue<IEnumerable<FrameworkElement>> toDo = new Queue<IEnumerable<FrameworkElement>>();

            toDo.Enqueue(root.GetVisualChildren());
            while (toDo.Count > 0)
            {
                IEnumerable<FrameworkElement> children = toDo.Dequeue();
                foreach (FrameworkElement child in children)
                {
                    yield return child;
                    toDo.Enqueue(child.GetVisualChildren());
                }
            }
        }

        /// <summary>
        /// Returns all the descendents of a particular type
        /// </summary>
        /// <typeparam name="T">The type to look for</typeparam>
        /// <param name="root">The root element</param>
        /// <param name="allAtSameLevel">Whether to stop searching the tree after the first set of items are found</param>
        /// <returns>List of the element found</returns>
        /// <remarks>
        /// The allAtSameLevel flag is used to control enumeration through the tree. For many cases (eg, finding ListBoxItems in a
        /// ListBox) you want enumeration to stop as soon as you've found all the items in the ListBox (no need to search further
        /// in the tree). For other cases though (eg, finding all the Buttons on a page) you want to exhaustively search the entire tree
        /// </remarks>
        public static IEnumerable<T> GetVisualDescendents<T>(this FrameworkElement root, bool allAtSameLevel) where T : FrameworkElement
        {
            bool found = false;
            foreach (FrameworkElement e in root.GetVisualDescendents())
            {
                if (e is T)
                {
                    found = true;
                    yield return e as T;
                }
                else
                {
                    if (found == true && allAtSameLevel == true)
                        yield break;
                }
            }
        }

        /// <summary>
        /// Print the entire visual element tree of an item to the debug console
        /// </summary>
        /// <param name="root">The item whose descendents should be printed</param>
        [Conditional("DEBUG")]
        public static void PrintDescendentsTree(this FrameworkElement root)
        {
            List<string> results = new List<string>();
            root.GetChildTree("", "  ", results);
            foreach (string s in results)
                Debug.WriteLine(s);
        }

        /// <summary>
        /// Prints the visual ancestor tree for an item to the debug console
        /// </summary>
        /// <param name="node">The item whost ancestors you want to print</param>
        [Conditional("DEBUG")]
        public static void PrintAncestorTree(this FrameworkElement node)
        {
            List<string> tree = new List<string>();
            node.GetAncestorVisualTree(tree);
            string prefix = "";
            foreach (string s in tree)
            {
                Debug.WriteLine(prefix + s);
                prefix = prefix + "  ";
            }

        }

        /// <summary>
        /// Gets the vertical offset for a ListBox
        /// </summary>
        /// <param name="list">The ListBox to check</param>
        /// <returns>The vertical offset</returns>
        public static double GetVerticalScrollOffset(this ListBox list)
        {
            ScrollViewer viewer = list.FindVisualChild("ScrollViewer") as ScrollViewer;
            return viewer.VerticalOffset;
        }

        /// <summary>
        /// Gets the horizontal offset for a ListBox
        /// </summary>
        /// <param name="list">The ListBox to check</param>
        /// <returns>The horizontal offset</returns>
        public static double GetHorizontalScrollOffset(this ListBox list)
        {
            ScrollViewer viewer = list.FindVisualChild("ScrollViewer") as ScrollViewer;
            return viewer.HorizontalOffset;
        }

        /// <summary>
        /// Sets the vertical offset of a ListBox
        /// </summary>
        /// <param name="list">The ListBox to check</param>
        /// <returns>True if it was set; false otherwise</returns>
        static bool Internal_SetVerticalScrollOffset(this ListBox list, double offset)
        {
            ScrollViewer viewer = list.FindVisualChild("ScrollViewer") as ScrollViewer;
            if (viewer != null)
            {
                viewer.ScrollToVerticalOffset(offset);
                if (list is ISupportOffsetChanges)
                    (list as ISupportOffsetChanges).VerticalOffsetChanged(offset);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the horizontal offset of a ListBox
        /// </summary>
        /// <param name="list">The ListBox to check</param>
        /// <returns>True if it was set; false otherwise</returns>
        static bool Internal_SetHorizontalScrollOffset(this ListBox list, double offset)
        {
            ScrollViewer viewer = list.FindVisualChild("ScrollViewer") as ScrollViewer;
            if (viewer != null)
            {
                viewer.ScrollToHorizontalOffset(offset);
                if (list is ISupportOffsetChanges)
                    (list as ISupportOffsetChanges).HorizontalOffsetChanged(offset);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the vertical offset of a ListBox
        /// </summary>
        /// <param name="list">The ListBox to modify</param>
        /// <param name="offset">The offset to set</param>
        /// <remarks>
        /// This method will automatically try to wait until the listbox is loaded if the
        /// visual tree has not been created yet
        /// </remarks>
        public static void SetVerticalScrollOffset(this ListBox list, double offset)
        {
            if (list.Internal_SetVerticalScrollOffset(offset))
                return;

            // List is probably not loaded yet; defer scroll until loaded has fired
            ScheduleOnNextRender(delegate { list.Internal_SetVerticalScrollOffset(offset); });
        }

        /// <summary>
        /// Sets the horizontal offset of a ListBox
        /// </summary>
        /// <param name="list">The ListBox to modify</param>
        /// <param name="offset">The offset to set</param>
        /// <remarks>
        /// This method will automatically try to wait until the listbox is loaded if the
        /// visual tree has not been created yet
        /// </remarks>
        public static void SetHorizontalScrollOffset(this ListBox list, double offset)
        {
            if (list.Internal_SetHorizontalScrollOffset(offset))
                return;

            // List is probably not loaded yet; defer scroll until loaded has fired
            ScheduleOnNextRender(delegate { list.Internal_SetHorizontalScrollOffset(offset); });
        }

        /// <summary>
        /// List of work to do on the next render (at the end of the current tick)
        /// </summary>
        static List<Action> workItems;

        /// <summary>
        /// Schedules work to happen at the end of this tick, when the <see cref="CompositionTarget.Rendering"/> event is raised
        /// </summary>
        /// <param name="action">The work to do</param>
        /// <remarks>
        /// Typically you can schedule work using Dispatcher.BeginInvoke, but sometimes that will result in a single-frame
        /// glitch of the visual tree. In that case, use this method.
        /// </remarks>
        public static void ScheduleOnNextRender(Action action)
        {
            if (workItems == null)
            {
                workItems = new List<Action>();
                CompositionTarget.Rendering += DoWorkOnRender;
            }

            workItems.Add(action);
        }

        /// <summary>
        /// Actual function that does work on the render event, then immediately unregisters itself
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The args</param>
        static void DoWorkOnRender(object sender, EventArgs args)
        {
            Debug.WriteLine("DoWorkOnRender running... if you see this message a lot then something is wrong!");

            // Remove ourselves from the event and clear the list
            CompositionTarget.Rendering -= DoWorkOnRender;
            List<Action> work = workItems;
            workItems = null;

            foreach (Action action in work)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached)
                        Debugger.Break();

                    Debug.WriteLine("Exception while doing work for " + action.Method.Name + ". " + ex.Message);
                }
            }
        }


    }
}