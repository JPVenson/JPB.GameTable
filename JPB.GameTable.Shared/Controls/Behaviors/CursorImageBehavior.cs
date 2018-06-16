using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace JPB.GameTable.UI.Resources.Behaviors
{
	public class CursorImageBehavior : Behavior<Image>
	{
		public CursorImageBehavior()
		{
			
		}

		public static readonly DependencyProperty CursorProperty = DependencyProperty.Register(
		"Cursor", typeof(System.Windows.Forms.Cursor), typeof(CursorImageBehavior), new PropertyMetadata(Cursors.Arrow));

		public System.Windows.Forms.Cursor Cursor
		{
			get { return (System.Windows.Forms.Cursor) GetValue(CursorProperty); }
			set { SetValue(CursorProperty, value); }
		}

		/// <inheritdoc />
		protected override void OnAttached()
		{
			AssociatedObject.Source = ConvertFromWindowFormCursor(Cursor);
		}

		ImageSource ConvertFromWindowFormCursor(System.Windows.Forms.Cursor cursor)
		{
			int width = cursor.Size.Width;
			int height = cursor.Size.Height;
			System.Drawing.Bitmap b = new System.Drawing.Bitmap(width, height);
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b);
			cursor.Draw(g, new System.Drawing.Rectangle(0, 0, width, height));
			AssociatedObject.RenderTransform = new TranslateTransform(-cursor.HotSpot.X, 0);
			ImageSource img = Imaging.CreateBitmapSourceFromHIcon(b.GetHicon(), new Int32Rect(0, 0, width, height), BitmapSizeOptions.FromEmptyOptions());
			return img;
		}

		//If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject([In] IntPtr hObject);

		public ImageSource ImageSourceForBitmap(Bitmap bmp)
		{
			var handle = bmp.GetHbitmap();
			try
			{
				return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
			finally { DeleteObject(handle); }
		}
	}
}
