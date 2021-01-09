using HelixToolkit.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CubliApp
{
    class ViewModel3d
    {
        private const string MODEL_PATH = "cube.obj";
        private static ModelVisual3D device3D;
        private static HelixViewport3D viewPort3d;
        private static Matrix3D StartPosition;
        public static void CreateModel(ref HelixViewport3D viewPort)
        {
            if (device3D == null)
            {
                if (File.Exists(MODEL_PATH))
                {
                    viewPort3d = viewPort;


                    device3D = new ModelVisual3D();
                    device3D.Content = Display3d();
                    StartPosition = device3D.Transform.Value;

                    viewPort3d.Camera.LookDirection = new Vector3D(14.32, -15.95, -7.48);
                    viewPort3d.Camera.UpDirection = new Vector3D(-0.354, 0.392, 0.834);
                    viewPort3d.Camera.Position = new Point3D(-16.52, 16.62, 9);
                    //viewPort3d.CameraController.CameraTarget = new Point3D(-2.2, 0.6, 1.5);
                    viewPort3d.Children.Add(device3D);
                }
                else
                {
                    MessageBox.Show("Wrong path to model!");
                }
            }
        }
        public static Model3D Display3d()
        {
            Model3D device = null;
            try
            {
                //Adding a gesture here
                viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);

                //Import 3D model file
                ModelImporter import = new ModelImporter();
                Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Gray));
                import.DefaultMaterial = material;
                //Load the 3D model file

                device = import.Load(MODEL_PATH);

            }
            catch (Exception e)
            {
                // Handle exception in case can not file 3D model
                MessageBox.Show("Exception Error : " + e.StackTrace);
            }
            return device;
        }

        public static void Rotate(string axis, char direction, int angle)
        {
            if (device3D != null)
            {
                angle = direction == '+' ? angle : -angle;

                if (axis == "x")
                {
                    var matrix = device3D.Transform.Value;
                    matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), angle));
                    device3D.Transform = new MatrixTransform3D(matrix);
                }
                else if (axis == "y")
                {
                    var matrix = device3D.Transform.Value;
                    matrix.Rotate(new Quaternion(new Vector3D(0, -1, 0), angle));
                    device3D.Transform = new MatrixTransform3D(matrix);
                }
            }
        }
        public static void Reset()
        {
            device3D.Transform = new MatrixTransform3D(StartPosition);
        }
    }
}
