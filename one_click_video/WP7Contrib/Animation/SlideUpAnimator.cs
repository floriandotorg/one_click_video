﻿// --------------------------------------------------------------------------------------------------------------------
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
    /// The slide up animator.
    /// </summary>
    public class SlideUpAnimator : SlideAnimator
    {
        #region Constants and Fields

        /// <summary>
        /// The storyboard.
        /// </summary>
        private static Storyboard storyboard;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideUpAnimator"/> class.
        /// </summary>
        public SlideUpAnimator()
        {
            if (storyboard == null)
            {
                storyboard = XamlReader.Load(Storyboards.SlideUpFadeInStoryboard) as Storyboard;
            }

            this.Storyboard = storyboard;
        }

        #endregion
    }
}