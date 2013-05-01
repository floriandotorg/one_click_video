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
    using System.Windows.Markup;
    using System.Windows.Media.Animation;

    using WP7Contrib.View.Controls.Transitions.Animation;

    /// <summary>
    /// The continuum forward out animator.
    /// </summary>
    public class ContinuumForwardOutAnimator : ContinuumAnimator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuumForwardOutAnimator"/> class.
        /// </summary>
        public ContinuumForwardOutAnimator()
        {
            this.Storyboard = XamlReader.Load(Storyboards.ContinuumForwardOutStoryboard) as Storyboard;
        }

        #endregion
    }
}