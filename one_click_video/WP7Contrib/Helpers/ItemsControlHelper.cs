// --------------------------------------------------------------------------------------------------------------------
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
// --------------------------------------------------------------------------------------------------------------------


namespace WP7Contrib.View.Controls.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using WP7Contrib.View.Controls.Extensions;

    /// <summary>
    /// The ItemContainerGenerator provides useful utilities for ItemsControls.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public sealed class ItemsControlHelper
    {
        #region Constants and Fields

        /// <summary>
        /// A Panel that is used as the ItemsHost of the ItemsControl.  This
        /// property will only be valid when the ItemsControl is live in the
        /// tree and has generated containers for some of its items.
        /// </summary>
        private Panel itemsHost;

        /// <summary>
        /// A ScrollViewer that is used to scroll the items in the ItemsHost.
        /// </summary>
        private ScrollViewer scrollHost;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsControlHelper"/> class. 
        /// Initializes a new instance of the ItemContainerGenerator.
        /// </summary>
        /// <param name="control">
        /// The ItemsControl being tracked by the ItemContainerGenerator.
        /// </param>
        public ItemsControlHelper(ItemsControl control)
        {
            Debug.Assert(control != null, "control cannot be null!");
            this.ItemsControl = control;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a Panel that is used as the ItemsHost of the ItemsControl.
        /// This property will only be valid when the ItemsControl is live in
        /// the tree and has generated containers for some of its items.
        /// </summary>
        public Panel ItemsHost
        {
            get
            {
                // Lookup the ItemsHost if we haven't already cached it.
                if (this.itemsHost == null && this.ItemsControl != null &&
                    this.ItemsControl.ItemContainerGenerator != null)
                {
                    // Get any live container
                    DependencyObject container = this.ItemsControl.ItemContainerGenerator.ContainerFromIndex(0);
                    if (container != null)
                    {
                        // Get the parent of the container
                        this.itemsHost = VisualTreeHelper.GetParent(container) as Panel;
                    }
                    else
                    {
                        this.itemsHost =
                            this.ItemsControl.GetVisualDescendants().OfType<VirtualizingStackPanel>().FirstOrDefault();
                    }
                }

                return this.itemsHost;
            }
        }

        /// <summary>
        /// Gets a ScrollViewer that is used to scroll the items in the
        /// ItemsHost.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Code is linked into multiple projects.")]
        public ScrollViewer ScrollHost
        {
            get
            {
                if (this.scrollHost == null)
                {
                    var panel = this.ItemsHost;
                    if (panel != null)
                    {
                        for (DependencyObject obj = panel;
                             obj != this.ItemsControl && obj != null;
                             obj = VisualTreeHelper.GetParent(obj))
                        {
                            var viewer = obj as ScrollViewer;
                            if (viewer == null)
                            {
                                continue;
                            }

                            this.scrollHost = viewer;
                            break;
                        }
                    }
                }

                return this.scrollHost;
            }
        }

        /// <summary>
        /// Gets or sets the ItemsControl being tracked by the
        /// ItemContainerGenerator.
        /// </summary>
        private ItemsControl ItemsControl { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Prepares the specified container to display the specified item.
        /// </summary>
        /// <param name="element">
        /// Container element used to display the specified item.
        /// </param>
        /// <param name="parentItemContainerStyle">
        /// The ItemContainerStyle for the parent ItemsControl.
        /// </param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", 
            Justification = "Code is linked into multiple projects.")]
        public static void PrepareContainerForItemOverride(DependencyObject element, Style parentItemContainerStyle)
        {
            // Apply the ItemContainerStyle to the item
            var control = element as Control;
            if (parentItemContainerStyle != null && control != null && control.Style == null)
            {
                control.SetValue(FrameworkElement.StyleProperty, parentItemContainerStyle);
            }

            // Note: WPF also does preparation for ContentPresenter,
            // ContentControl, HeaderedContentControl, and ItemsControl.  Since
            // we don't have any other ItemsControls using this
            // ItemContainerGenerator, we've removed that code for now.  It
            // should be added back later when necessary.
        }

        /// <summary>
        /// Apply a control template to the ItemsControl.
        /// </summary>
        public void OnApplyTemplate()
        {
            // Clear the cached ItemsHost, ScrollHost
            this.itemsHost = null;
            this.scrollHost = null;
        }

        /// <summary>
        /// Scroll the desired element into the ScrollHost's viewport.
        /// </summary>
        /// <param name="element">
        /// Element to scroll into view.
        /// </param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", 
            Justification = "File is linked across multiple projects and this method is used in some but not others.")]
        public void ScrollIntoView(FrameworkElement element)
        {
            // Get the ScrollHost
            var host = this.ScrollHost;
            if (host == null)
            {
                return;
            }

            // Get the position of the element relative to the ScrollHost
            GeneralTransform transform;
            try
            {
                transform = element.TransformToVisual(host);
            }
            catch (ArgumentException)
            {
                // Ignore failures when not in the visual tree
                return;
            }

            var itemRect = new Rect(
                transform.Transform(new Point()), 
                transform.Transform(new Point(element.ActualWidth, element.ActualHeight)));

            // Scroll vertically
            double verticalOffset = host.VerticalOffset;
            double verticalDelta = 0;
            double hostBottom = host.ViewportHeight;
            double itemBottom = itemRect.Bottom;
            if (hostBottom < itemBottom)
            {
                verticalDelta = itemBottom - hostBottom;
                verticalOffset += verticalDelta;
            }

            double itemTop = itemRect.Top;
            if (itemTop - verticalDelta < 0)
            {
                verticalOffset -= verticalDelta - itemTop;
            }

            host.ScrollToVerticalOffset(verticalOffset);

            // Scroll horizontally
            double horizontalOffset = host.HorizontalOffset;
            double horizontalDelta = 0;
            double hostRight = host.ViewportWidth;
            double itemRight = itemRect.Right;
            if (hostRight < itemRight)
            {
                horizontalDelta = itemRight - hostRight;
                horizontalOffset += horizontalDelta;
            }

            double itemLeft = itemRect.Left;
            if (itemLeft - horizontalDelta < 0)
            {
                horizontalOffset -= horizontalDelta - itemLeft;
            }

            host.ScrollToHorizontalOffset(horizontalOffset);
        }

        /// <summary>
        /// Update the style of any generated items when the ItemContainerStyle
        /// has been changed.
        /// </summary>
        /// <param name="itemContainerStyle">
        /// The ItemContainerStyle.
        /// </param>
        /// <remarks>
        /// Silverlight does not support setting a Style multiple times, so we
        /// only attempt to set styles on elements whose style hasn't already
        /// been set.
        /// </remarks>
        public void UpdateItemContainerStyle(Style itemContainerStyle)
        {
            if (itemContainerStyle == null)
            {
                return;
            }

            Panel panel = this.ItemsHost;
            if (panel == null || panel.Children == null)
            {
                return;
            }

            foreach (var obj in panel.Children.OfType<FrameworkElement>().Where(obj => obj.Style == null))
            {
                obj.Style = itemContainerStyle;
            }
        }

        #endregion
    }
}