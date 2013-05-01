// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueExpiry.cs" company="XamlNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// <summary>
// </summary>
// <credits>
// Kevin Marshall http://blogs.claritycon.com/blog/2010/10/13/wp7-page-transitions-sample/
// </credits>
// --------------------------------------------------------------------------------------------------------------------

namespace WP7Contrib.View.Transitions.Animation
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    using WP7Contrib.View.Controls.Extensions;
    using WP7Contrib.View.Controls.Helpers;

    /// <summary>
    /// The turnstile feather animator.
    /// </summary>
    public class TurnstileFeatherAnimator : AnimatorHelperBase
    {
        #region Constants and Fields

        /// <summary>
        /// The _is vertical orientation.
        /// </summary>
        private bool? isVerticalOrientation;

        /// <summary>
        /// The _visual.
        /// </summary>
        private FrameworkElement visual;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TurnstileFeatherAnimator"/> class.
        /// </summary>
        public TurnstileFeatherAnimator()
        {
            this.InitialDelay = 0;
        }

        #endregion

        #region Enums

        /// <summary>
        /// The directions.
        /// </summary>
        protected enum Directions
        {
            /// <summary>
            /// The in.
            /// </summary>
            In, 

            /// <summary>
            /// The out.
            /// </summary>
            Out
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets ListBox.
        /// </summary>
        public ListBox ListBox { get; set; }

        /// <summary>
        /// Gets or sets Angle.
        /// </summary>
        protected int Angle { get; set; }

        /// <summary>
        /// Gets or sets Direction.
        /// </summary>
        protected Directions Direction { get; set; }

        /// <summary>
        /// Gets or sets Duration.
        /// </summary>
        protected int Duration { get; set; }

        /// <summary>
        /// Gets or sets FeatherDelay.
        /// </summary>
        protected int FeatherDelay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HoldSelectedItem.
        /// </summary>
        protected bool HoldSelectedItem { get; set; }

        /// <summary>
        /// Gets or sets InitialDelay.
        /// </summary>
        protected int InitialDelay { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The begin.
        /// </summary>
        /// <param name="completionAction">
        /// The completion action.
        /// </param>
        public override void Begin(Action completionAction)
        {
            Storyboard = new Storyboard();

            double liCounter = 0;
            var listBoxItems = ListBox.GetVisualDescendants().OfType<ListBoxItem>().Where(lbi => IsOnCurrentPage(lbi) && lbi.IsEnabled).ToList();

            if (HoldSelectedItem && Direction == Directions.Out && ListBox.SelectedItem != null)
            {
                //move selected container to end
                var selectedContainer = ListBox.ItemContainerGenerator.ContainerFromItem(ListBox.SelectedItem);
                listBoxItems.Remove(selectedContainer as ListBoxItem);
                listBoxItems.Add(selectedContainer as ListBoxItem);
            }

            foreach (ListBoxItem li in listBoxItems)
            {
                GeneralTransform gt = li.TransformToVisual(RootElement);
                Point globalCoords = gt.Transform(new Point(0, 0));
                double heightAdjustment = li.Content is FrameworkElement ? ((li.Content as FrameworkElement).ActualHeight / 2) : (li.ActualHeight / 2);
                //double yCoord = globalCoords.Y + ((((System.Windows.FrameworkElement)(((System.Windows.Controls.ContentControl)(li)).Content)).ActualHeight) / 2);
                double yCoord = globalCoords.Y + heightAdjustment;

                double offsetAmount = (RootElement.ActualHeight / 2) - yCoord;

                PlaneProjection pp = new PlaneProjection();
                pp.GlobalOffsetY = offsetAmount * -1;
                pp.CenterOfRotationX = 0;
                li.Projection = pp;

                CompositeTransform ct = new CompositeTransform();
                ct.TranslateY = offsetAmount;
                li.RenderTransform = ct;

                var beginTime = TimeSpan.FromMilliseconds((FeatherDelay * liCounter) + InitialDelay);

                if (Direction == Directions.In)
                {
                    li.Opacity = 0;

                    DoubleAnimationUsingKeyFrames daukf = new DoubleAnimationUsingKeyFrames();

                    EasingDoubleKeyFrame edkf1 = new EasingDoubleKeyFrame();
                    edkf1.KeyTime = beginTime;
                    edkf1.Value = Angle;
                    daukf.KeyFrames.Add(edkf1);

                    EasingDoubleKeyFrame edkf2 = new EasingDoubleKeyFrame();
                    edkf2.KeyTime = TimeSpan.FromMilliseconds(Duration).Add(beginTime);
                    edkf2.Value = 0;

                    ExponentialEase ee = new ExponentialEase();
                    ee.EasingMode = EasingMode.EaseOut;
                    ee.Exponent = 6;

                    edkf2.EasingFunction = ee;
                    daukf.KeyFrames.Add(edkf2);

                    Storyboard.SetTarget(daukf, li);
                    Storyboard.SetTargetProperty(daukf, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)"));
                    Storyboard.Children.Add(daukf);

                    DoubleAnimation da = new DoubleAnimation();
                    da.Duration = TimeSpan.FromMilliseconds(0);
                    da.BeginTime = beginTime;
                    da.To = 1;

                    Storyboard.SetTarget(da, li);
                    Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
                    Storyboard.Children.Add(da);
                }
                else
                {
                    li.Opacity = 1;

                    DoubleAnimation da = new DoubleAnimation();
                    da.BeginTime = beginTime;
                    da.Duration = TimeSpan.FromMilliseconds(Duration);
                    da.To = Angle;

                    ExponentialEase ee = new ExponentialEase();
                    ee.EasingMode = EasingMode.EaseIn;
                    ee.Exponent = 6;

                    da.EasingFunction = ee;

                    Storyboard.SetTarget(da, li);
                    Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)"));
                    Storyboard.Children.Add(da);

                    da = new DoubleAnimation();
                    da.Duration = TimeSpan.FromMilliseconds(10);
                    da.To = 0;
                    da.BeginTime = TimeSpan.FromMilliseconds(Duration).Add(beginTime);

                    Storyboard.SetTarget(da, li);
                    Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
                    Storyboard.Children.Add(da);
                }

                liCounter++;
            }

            base.Begin(completionAction);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The is on current page.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The is on current page.
        /// </returns>
        internal bool IsOnCurrentPage(ListBoxItem item)
        {
            Rect itemsHostRect = Rect.Empty;
            Rect listBoxItemRect = Rect.Empty;

            if (this.visual == null)
            {
                var ich = new ItemsControlHelper(this.ListBox);
                ScrollContentPresenter scp = ich.ScrollHost == null
                                                 ? null
                                                 : ich.ScrollHost.GetVisualDescendants().OfType<ScrollContentPresenter>(
                                                     ).FirstOrDefault();

                this.visual = (ich.ScrollHost == null)
                                  ? null
                                  : ((scp == null) ? ich.ScrollHost : ((FrameworkElement)scp));
            }

            if (this.visual == null)
            {
                return true;
            }

            itemsHostRect = new Rect(0.0, 0.0, this.visual.ActualWidth, this.visual.ActualHeight);

            // ListBoxItem item = ListBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
            if (item == null)
            {
                listBoxItemRect = Rect.Empty;
                return false;
            }

            GeneralTransform transform = item.TransformToVisual(this.visual);
            listBoxItemRect = new Rect(
                transform.Transform(new Point()), transform.Transform(new Point(item.ActualWidth, item.ActualHeight)));

            if (!this.IsVerticalOrientation())
            {
                return (itemsHostRect.Left <= listBoxItemRect.Left) && (listBoxItemRect.Right <= itemsHostRect.Right);
            }

            return (listBoxItemRect.Bottom + 100 >= itemsHostRect.Top) &&
                   (listBoxItemRect.Top - 100 <= itemsHostRect.Bottom);

            // return ((itemsHostRect.Top <= listBoxItemRect.Bottom) && (listBoxItemRect.Top <= itemsHostRect.Bottom));
        }

        /// <summary>
        /// The is vertical orientation.
        /// </summary>
        /// <returns>
        /// The is vertical orientation.
        /// </returns>
        internal bool IsVerticalOrientation()
        {
            if (this.isVerticalOrientation.HasValue)
            {
                return this.isVerticalOrientation.Value;
            }

            var ich = new ItemsControlHelper(this.ListBox);
            var itemsHost = ich.ItemsHost as StackPanel;
            if (itemsHost != null)
            {
                this.isVerticalOrientation = itemsHost.Orientation == Orientation.Vertical;
                return this.isVerticalOrientation.Value;
            }

            var panel2 = ich.ItemsHost as VirtualizingStackPanel;
            this.isVerticalOrientation = (panel2 == null) || (panel2.Orientation == Orientation.Vertical);
            return this.isVerticalOrientation.Value;
        }

        #endregion
    }
}