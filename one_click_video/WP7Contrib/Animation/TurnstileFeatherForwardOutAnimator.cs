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
    /// The turnstile feather forward out animator.
    /// </summary>
    public class TurnstileFeatherForwardOutAnimator : TurnstileFeatherAnimator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TurnstileFeatherForwardOutAnimator"/> class.
        /// </summary>
        public TurnstileFeatherForwardOutAnimator()
        {
            this.Duration = 250;
            this.Angle = 50;
            this.FeatherDelay = 50;
            this.Direction = Directions.Out;
            this.HoldSelectedItem = true;
        }

        #endregion
    }
}