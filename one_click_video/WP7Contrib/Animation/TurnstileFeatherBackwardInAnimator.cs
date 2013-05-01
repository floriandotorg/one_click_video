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
    /// <summary>
    /// The turnstile feather backward in animator.
    /// </summary>
    public class TurnstileFeatherBackwardInAnimator : TurnstileFeatherAnimator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TurnstileFeatherBackwardInAnimator"/> class.
        /// </summary>
        public TurnstileFeatherBackwardInAnimator()
        {
            this.Duration = 350;
            this.Angle = 50;
            this.FeatherDelay = 50;
            this.Direction = Directions.In;
        }

        #endregion
    }
}