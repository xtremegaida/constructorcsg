using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Windows.Controls.Primitives;
using ConstructorEngine;

namespace Constructor.Views
{
   public partial class ConstructorView3D : UserControl
   {
      public static readonly DependencyProperty IsOrthographicProperty =
         DependencyProperty.Register("IsOrthographic", typeof(bool), typeof(ConstructorView3D), new PropertyMetadata(PropertyChanged));
      public static readonly DependencyProperty SelectedItemProperty =
         DependencyProperty.Register("SelectedItem", typeof(CsgNodeWrapper), typeof(ConstructorView3D), new PropertyMetadata(PropertyChanged));

      public bool IsOrthographic
      {
         get { return (bool)GetValue(IsOrthographicProperty); }
         set { SetValue(IsOrthographicProperty, value); }
      }

      public CsgNodeWrapper SelectedItem
      {
         get { return (CsgNodeWrapper)GetValue(SelectedItemProperty); }
         set { SetValue(SelectedItemProperty, value); }
      }

      protected readonly Dictionary<Mesh3, GeometryModel3D> meshGeometry = new Dictionary<Mesh3, GeometryModel3D>();

      protected readonly Matrix3 defaultMatrix = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);
      protected Vector3 position = new Vector3(0, 0, 0);
      protected Matrix3 direction;
      protected Point3D modelMin, modelMax;
      protected CsgNodeWrapper lastModel;

      protected DirectionalLight light;
      protected Material defaultMaterial = new DiffuseMaterial() { Brush = Brushes.White };

      private bool movingCameraAngle;
      private bool movingCameraPosition;
      private Point lastMoveMousePosition;
      private Cursor defaultMouseCursor;

      public ConstructorView3D()
      {
         DataContext = this;
         InitializeComponent();
         defaultMouseCursor = viewportContainer.Cursor;
         viewport.Children.Add(new ModelVisual3D()
         {
            Content = light = new DirectionalLight(Colors.White,
               new Vector3D(direction.Forward.X, direction.Forward.Y, direction.Forward.Z))
         });
         UpdateCamera();
      }

      public void ResetView()
      {
         direction = defaultMatrix.RotateRelY(Math.PI * -0.15).RotateRelX(Math.PI * -0.15);
         ResetViewingPosition();
      }

      private void ResetViewingPosition()
      {
         Vector3 center = new Vector3((modelMin.X + modelMax.X) * 0.5,
            (modelMin.Y + modelMax.Y) * 0.5, (modelMin.Z + modelMax.Z) * 0.5);
         double z = (modelMax.Z - modelMin.Z) * 0.5 + Math.Max(modelMax.X - modelMin.X, modelMax.Y - modelMin.Y) + 1;
         position = direction.Transpose().Transform(new Vector3(0, 0, -z)) + center;
         UpdateCamera();
      }

      protected virtual Mesh3[] GetMeshes()
      {
         return lastModel.Node.GetMeshes();
      }

      protected virtual void UpdateModel()
      {
         while (viewport.Children.Count > 1) { viewport.Children.RemoveAt(1); }
         CsgNodeWrapper wrapper = null;
         bool resetView = lastModel == null;
         if (LockToggle.IsChecked.GetValueOrDefault()) { wrapper = lastModel; lastModel.IsViewing = true; }
         else if (lastModel != null) { lastModel.IsViewing = false; }
         if (wrapper == null) { wrapper = SelectedItem; }
         if (lastModel != null) { lastModel.MeshChanged -= ModelMeshChanged; lastModel = null; }
         if (wrapper == null) { return; }
         CsgNode root = wrapper.Node;
         lastModel = wrapper;
         lastModel.MeshChanged += ModelMeshChanged;
         modelMin = new Point3D(double.MaxValue, double.MaxValue, double.MaxValue);
         modelMax = new Point3D(double.MinValue, double.MinValue, double.MinValue);
         meshGeometry.Clear();
         foreach (Mesh3 mesh in GetMeshes())
         {
            mesh.CalculateNormals();
            viewport.Children.Add(new ModelVisual3D()
            {
               Content = meshGeometry[mesh] = new GeometryModel3D()
               {
                  Material = defaultMaterial,
                  Geometry = new MeshGeometry3D()
                  {
                     TriangleIndices = new Int32Collection(mesh.TriangleIndices),
                     Normals = new Vector3DCollection(mesh.VertexNormals.Select(x => new Vector3D(x.X, x.Y, x.Z)).ToArray()),
                     Positions = new Point3DCollection(mesh.Vertices.Select(x =>
                     {
                        Vector3 transformed = mesh.Transform.Transform(x) + mesh.Position;
                        if (transformed.X < modelMin.X) { modelMin.X = transformed.X; }
                        if (transformed.X > modelMax.X) { modelMax.X = transformed.X; }
                        if (transformed.Y < modelMin.Y) { modelMin.Y = transformed.Y; }
                        if (transformed.Y > modelMax.Y) { modelMax.Y = transformed.Y; }
                        if (transformed.Z < modelMin.Z) { modelMin.Z = transformed.Z; }
                        if (transformed.Z > modelMax.Z) { modelMax.Z = transformed.Z; }
                        return new Point3D(x.X, x.Y, x.Z);
                     }).ToArray())
                  },
                  Transform = new MatrixTransform3D(mesh.Transform.ToMatrix3D(mesh.Position))
               }
            });
         }
         if (resetView) { ResetView(); }
      }

      private void ModelMeshChanged(object sender, EventArgs e)
      {
         UpdateModel();
      }

      protected void UpdateCamera()
      {
         double x, y, f; Vector3 v;
         if (IsOrthographic)
         {
            OrthographicCamera camera = viewport.Camera as OrthographicCamera;
            if (camera == null || camera.IsFrozen) { camera = new OrthographicCamera(); }
            camera.Position = new Point3D(position.X, position.Y, position.Z);
            camera.LookDirection = new Vector3D(direction.Forward.X, direction.Forward.Y, direction.Forward.Z);
            camera.UpDirection = new Vector3D(direction.Up.X, direction.Up.Y, direction.Up.Z);
            camera.NearPlaneDistance = 1e-5;
            camera.FarPlaneDistance = 1e5;
            camera.Width = direction.Transform(position).Z * -2;
            viewport.Camera = camera;
            v = direction.Right; v.Z = -100.0 / 3.0;
            axisX.X2 = axisX.X1 + (x = v.X * v.Z);
            axisX.Y2 = axisX.Y1 + (y = v.Y * v.Z);
            f = x * x + y * y; if (f > 1e-5) { f = 10.0 / Math.Sqrt(f); }
            Canvas.SetLeft(axisXName, axisX.X2 + x * f - 4);
            Canvas.SetTop(axisXName, axisX.Y2 + y * f - 6);
            v = direction.Up; v.Z = -100.0 / 3.0;
            axisY.X2 = axisY.X1 + (x = v.X * v.Z);
            axisY.Y2 = axisY.Y1 + (y = v.Y * v.Z);
            f = x * x + y * y; if (f > 1e-5) { f = 10.0 / Math.Sqrt(f); }
            Canvas.SetLeft(axisYName, axisY.X2 + x * f - 4);
            Canvas.SetTop(axisYName, axisY.Y2 + y * f - 6);
            v = direction.Forward; v.Z = 100.0 / 3.0;
            axisZ.X2 = axisZ.X1 + (x = v.X * v.Z);
            axisZ.Y2 = axisZ.Y1 + (y = v.Y * v.Z);
            f = x * x + y * y; if (f > 1e-5) { f = 10.0 / Math.Sqrt(f); }
            Canvas.SetLeft(axisZName, axisZ.X2 + x * f - 4);
            Canvas.SetTop(axisZName, axisZ.Y2 + y * f - 6);
         }
         else
         {
            PerspectiveCamera camera = viewport.Camera as PerspectiveCamera;
            if (camera == null || camera.IsFrozen) { camera = new PerspectiveCamera(); }
            camera.Position = new Point3D(position.X, position.Y, position.Z);
            camera.LookDirection = new Vector3D(direction.Forward.X, direction.Forward.Y, direction.Forward.Z);
            camera.UpDirection = new Vector3D(direction.Up.X, direction.Up.Y, direction.Up.Z);
            camera.NearPlaneDistance = 1e-5;
            camera.FarPlaneDistance = 1e5;
            camera.FieldOfView = 90;
            viewport.Camera = camera;
            v = direction.Right; v.Z = -100.0 / (v.Z + 3);
            axisX.X2 = axisX.X1 + (x = v.X * v.Z);
            axisX.Y2 = axisX.Y1 + (y = v.Y * v.Z);
            f = x * x + y * y; if (f > 1e-5) { f = 10.0 / Math.Sqrt(f); }
            Canvas.SetLeft(axisXName, axisX.X2 + x * f - 4);
            Canvas.SetTop(axisXName, axisX.Y2 + y * f - 6);
            v = direction.Up; v.Z = -100.0 / (v.Z + 3);
            axisY.X2 = axisY.X1 + (x = v.X * v.Z);
            axisY.Y2 = axisY.Y1 + (y = v.Y * v.Z);
            f = x * x + y * y; if (f > 1e-5) { f = 10.0 / Math.Sqrt(f); }
            Canvas.SetLeft(axisYName, axisY.X2 + x * f - 4);
            Canvas.SetTop(axisYName, axisY.Y2 + y * f - 6);
            v = direction.Forward; v.Z = 100.0 / (v.Z + 3);
            axisZ.X2 = axisZ.X1 + (x = v.X * v.Z);
            axisZ.Y2 = axisZ.Y1 + (y = v.Y * v.Z);
            f = x * x + y * y; if (f > 1e-5) { f = 10.0 / Math.Sqrt(f); }
            Canvas.SetLeft(axisZName, axisZ.X2 + x * f - 4);
            Canvas.SetTop(axisZName, axisZ.Y2 + y * f - 6);
         }
         light.Direction = new Vector3D(direction.Forward.X, direction.Forward.Y, direction.Forward.Z);
      }

      protected override void OnMouseWheel(MouseWheelEventArgs e)
      {
         position += direction.Forward * e.Delta * 0.005;
         UpdateCamera();
         e.Handled = true;
      }

      protected override void OnMouseDown(MouseButtonEventArgs e)
      {
         if (e.MiddleButton == MouseButtonState.Pressed)
         {
            movingCameraPosition = true;
            lastMoveMousePosition = e.GetPosition(this);
            CaptureMouse();
            viewportContainer.Cursor = Cursors.None;
            Mouse.SetCursor(Cursors.None);
            e.Handled = true;
            return;
         }
         if (e.RightButton == MouseButtonState.Pressed)
         {
            movingCameraAngle = true;
            lastMoveMousePosition = e.GetPosition(this);
            CaptureMouse();
            viewportContainer.Cursor = Cursors.None;
            Mouse.SetCursor(Cursors.None);
            e.Handled = true;
            return;
         }
         base.OnMouseDown(e);
      }

      protected override void OnMouseUp(MouseButtonEventArgs e)
      {
         if (movingCameraPosition || movingCameraAngle)
         {
            movingCameraPosition = false;
            movingCameraAngle = false;
            ReleaseMouseCapture();
            viewportContainer.Cursor = defaultMouseCursor;
         }
         base.OnMouseDown(e);
      }

      protected override void OnMouseMove(MouseEventArgs e)
      {
         if (movingCameraAngle)
         {
            if (e.RightButton != MouseButtonState.Pressed)
            {
               movingCameraAngle = false;
               ReleaseMouseCapture();
               viewportContainer.Cursor = defaultMouseCursor;
            }
            else
            {
               Point position = e.GetPosition(this);
               Vector deltaPosition = position - lastMoveMousePosition;
               if (deltaPosition.Y != 0) { direction = direction.RotateRelX(deltaPosition.Y * -0.004); }
               if (deltaPosition.X != 0) { direction = direction.RotateRelY(deltaPosition.X * 0.004); }
               direction = direction.Orthonormalise();
               UpdateCamera();
               lastMoveMousePosition = position;
               e.Handled = true;
               return;
            }
         }
         else if (movingCameraPosition)
         {
            if (e.MiddleButton != MouseButtonState.Pressed)
            {
               movingCameraPosition = false;
               ReleaseMouseCapture();
               viewportContainer.Cursor = defaultMouseCursor;
            }
            else
            {
               Point position = e.GetPosition(this);
               Vector deltaPosition = position - lastMoveMousePosition;
               this.position += direction.Up * deltaPosition.Y * -0.01;
               this.position += direction.Right * deltaPosition.X * -0.01;
               UpdateCamera();
               lastMoveMousePosition = position;
               e.Handled = true;
               return;
            }
         }
         base.OnMouseMove(e);
      }

      private void ResetClick(object sender, RoutedEventArgs e)
      {
         ResetView();
      }
      
      private void LockClick(object sender, RoutedEventArgs e)
      {
         UpdateModel();
      }

      private void ViewFrontClick(object sender, RoutedEventArgs e)
      {
         direction = defaultMatrix;
         ResetViewingPosition();
      }

      private void ViewBackClick(object sender, RoutedEventArgs e)
      {
         direction = defaultMatrix.RotateRelY(Math.PI);
         ResetViewingPosition();
      }

      private void ViewLeftClick(object sender, RoutedEventArgs e)
      {
         direction = defaultMatrix.RotateRelY(Math.PI * -0.5);
         ResetViewingPosition();
      }

      private void ViewRightClick(object sender, RoutedEventArgs e)
      {
         direction = defaultMatrix.RotateRelY(Math.PI * 0.5);
         ResetViewingPosition();
      }

      private void ViewTopClick(object sender, RoutedEventArgs e)
      {
         direction = defaultMatrix.RotateRelX(Math.PI * -0.5);
         ResetViewingPosition();
      }

      private void ViewBottomClick(object sender, RoutedEventArgs e)
      {
         direction = defaultMatrix.RotateRelX(Math.PI * 0.5);
         ResetViewingPosition();
      }

      private static void PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
      {
         if (args.Property == SelectedItemProperty)
         {
            ((ConstructorView3D)sender).UpdateModel();
         }
         else if (args.Property == IsOrthographicProperty)
         {
            ((ConstructorView3D)sender).UpdateCamera();
         }
      }
   }

   public static class MathExtensions
   {
      public static Matrix3D ToMatrix3D(this Matrix3 matrix)
      {
         return new Matrix3D(
            matrix.Right.X, matrix.Up.X, matrix.Forward.X, 0,
            matrix.Right.Y, matrix.Up.Y, matrix.Forward.Y, 0,
            matrix.Right.Z, matrix.Up.Z, matrix.Forward.Z, 0,
            0, 0, 0, 1);
      }

      public static Matrix3D ToMatrix3D(this Matrix3 matrix, Vector3 position)
      {
         Matrix3D newMatrix = matrix.ToMatrix3D();
         newMatrix.Translate(new Vector3D(position.X, position.Y, position.Z));
         return newMatrix;
      }
   }
}
