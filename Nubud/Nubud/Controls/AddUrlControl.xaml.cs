using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.ComponentModel;

namespace Nubud.Controls
{
    public partial class AddUrlOverlay : UserControl
    {
        // PhoneApplicationFrame needed for detecting back presses
        private PhoneApplicationFrame _rootFrame = null;

        // Use this for detecting visibility change on code
        public event EventHandler VisibilityChanged = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AddUrlOverlay()
        {
            InitializeComponent();
            Loaded += FeedbackOverlay_Loaded;
            hideContent.Completed += hideContent_Completed;
        }

        /// <summary>
        /// Reset review and feedback funtionality. Makes notifications active
        /// again, for example, after a major application update.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FeedbackOverlay_Loaded(object sender, RoutedEventArgs e)
        {
            // This class needs to be aware of Back key presses.
            AttachBackKeyPressed();
        }

        /// <summary>
        /// Detect back key presses.
        /// </summary>
        private void AttachBackKeyPressed()
        {
            if (_rootFrame == null)
            {
                _rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                _rootFrame.BackKeyPress += FeedbackOverlay_BackKeyPress;
            }
        }

        /// <summary>
        /// Handle back key presses.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FeedbackOverlay_BackKeyPress(object sender, CancelEventArgs e)
        {
            if (this.Visibility == System.Windows.Visibility.Visible)
            {
                this.Tag = "";
                hideContent.Begin();
                e.Cancel = true;
            }
        }


        /// <summary>
        /// Called when no button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            this.Tag = "";
            hideContent.Begin();
        }


        /// <summary>
        /// Called when notification gets hidden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hideContent_Completed(object sender, EventArgs e)
        {
            SetVisibility(false);
        }

        public void Show()
        {
            if (this.Visibility == System.Windows.Visibility.Collapsed)
            {
                SetVisibility(true);

                LayoutRoot.Opacity = 0;
                xProjection.RotationX = 90;

                showContent.Begin();
            }
        }

        /// <summary>
        /// Called when visibility changes.
        /// </summary>
        private void OnVisibilityChanged()
        {
            if (VisibilityChanged != null)
            {
                VisibilityChanged(this, new EventArgs());
            }
        }


        /// <summary>
        /// Called when yes button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Tag = "add";
            hideContent.Begin();
        }

        /// <summary>
        /// Set review/feedback notification visibility.
        /// </summary>
        /// <param name="visible">True to set visible, otherwise False.</param>
        private void SetVisibility(bool visible)
        {
            if (visible)
            {
                PreparePanoramaPivot(false);
                Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PreparePanoramaPivot(true);
                Visibility = System.Windows.Visibility.Collapsed;
            }

            OnVisibilityChanged();
        }

        /// <summary>
        /// Prepare underlaying Pivot and Panorama controls if such exist.
        /// 
        /// Pivot and Panorama capture touch gestures and remain active even
        /// when overlaid with a UserControl. When FeedbackOverlay is shown,
        /// touch events for pivot and panorama controls are disabled, and
        /// they are enabled again after FeedbackOverlay is hidden.
        /// </summary>
        /// <param name="hitTestVisible">True to set visible, otherwise False.</param>
        private void PreparePanoramaPivot(bool hitTestVisible)
        {
            DependencyObject o = VisualTreeHelper.GetParent(this);

            int children = VisualTreeHelper.GetChildrenCount(o);
            for (int i = 0; i < children; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(o, i);
                Type t = child.GetType();
                if (t.FullName == "Microsoft.Phone.Controls.Panorama" ||
                    t.FullName == "Microsoft.Phone.Controls.Pivot")
                {
                    UIElement elem = (UIElement)child;
                    elem.IsHitTestVisible = hitTestVisible;
                }
            }
        }

        private void Url_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.Tag = "add";
                hideContent.Begin();
            }
        }

    }
}
