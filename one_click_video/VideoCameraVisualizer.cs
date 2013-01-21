using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

namespace one_click_video
{
    /// <summary>
    /// Wrapper around the Microsoft.Phone.CameraVisualizer object in Microsoft.Phone.Media.Extended.
    /// </summary>
    public class VideoCameraVisualizer : UserControl
    {
        private object _cameraVisualizer;
        private MethodInfo _cameraVisualizerSetSourceMethod;
        private FieldInfo _cameraVisualizerViewfinderBrush;

        public VideoCameraVisualizer()
        {
            // Load the media extended assembly which contains the extended video camera object.
            Assembly mediaExtendedAssembly = Assembly.Load("Microsoft.Phone.Media.Extended, Version=7.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e");

            // Get the camera source type (primary camera).
            Type cameraVisualizerType = mediaExtendedAssembly.GetType("Microsoft.Phone.CameraVisualizer");
            ConstructorInfo cameraVisualizerConstructor = cameraVisualizerType.GetConstructor(Type.EmptyTypes);
            _cameraVisualizer = cameraVisualizerConstructor.Invoke(null);

            // Set the properties and methods.
            _cameraVisualizerSetSourceMethod = cameraVisualizerType.GetMethod("SetSource");

            _cameraVisualizerViewfinderBrush = cameraVisualizerType.GetField("viewfinderBrush", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void SetSource(VideoCamera camera)
        {
            // Invoke the set source method on the camera visualizer object.
            _cameraVisualizerSetSourceMethod.Invoke(_cameraVisualizer, new object[] { camera.InnerCameraObject });
        }

        public object InnerObject()
        {
            return _cameraVisualizer;
        }
    }
}
