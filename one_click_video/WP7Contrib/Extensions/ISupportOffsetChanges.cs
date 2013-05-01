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
    /// <summary>
  /// Interface for listboxes that want to know when their offset has changed
  /// </summary>
  /// <remarks>
  /// This is really a hack for supporting re-creation of LazyListBox along with 
  /// saved scroll offsets...
  /// </remarks>
  public interface ISupportOffsetChanges
  {
    /// <summary>
    /// The horizontal offset has been changed
    /// </summary>
    /// <param name="offset">The new offset</param>
    void HorizontalOffsetChanged(double offset);

    /// <summary>
    /// The vertical offset has been changed
    /// </summary>
    /// <param name="offset">The new offset</param>
    void VerticalOffsetChanged(double offset);
  }
}
