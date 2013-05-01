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
    /// The turnstile feather backward out animator.
    /// </summary>
    public class TurnstileFeatherBackwardOutAnimator : TurnstileFeatherAnimator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TurnstileFeatherBackwardOutAnimator"/> class.
        /// </summary>
        public TurnstileFeatherBackwardOutAnimator()
        {
            this.Duration = 250;
            this.Angle = -80;
            this.FeatherDelay = 50;
            this.Direction = Directions.Out;
        }

        #endregion
    }
}