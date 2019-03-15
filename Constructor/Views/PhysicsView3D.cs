using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Media3D;
using ConstructorEngine;

namespace Constructor.Views
{
   public class PhysicsView3D : ConstructorView3D
   {
      protected readonly PhysicsSimulation physics = new PhysicsSimulation();
      protected DispatcherTimer timer = new DispatcherTimer();

      public PhysicsView3D()
      {
         LockToggle.Visibility = Visibility.Collapsed;
         timer.Interval = new TimeSpan(10 * 1000 * 20);
         physics.DeltaTime = (float)timer.Interval.TotalSeconds;
         timer.Tick += TimerTick;
      }

      private void TimerTick(object sender, EventArgs e)
      {
         physics.Run();
         foreach (var pair in meshGeometry)
         {
            pair.Value.Transform = new MatrixTransform3D(pair.Key.Transform.ToMatrix3D(pair.Key.Position));
         }
      }

      public void Stop()
      {
         timer.Stop();
      }

      protected override void UpdateModel()
      {
         CsgNodeWrapper node = SelectedItem;
         if (node == null) { timer.IsEnabled = false; }
         else { physics.SetRootNode(node.Node); timer.IsEnabled = true; }
         base.UpdateModel();
      }

      protected override Mesh3[] GetMeshes()
      {
         return physics.GetMeshes();
      }
   }
}
