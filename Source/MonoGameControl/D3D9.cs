using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using DeviceType = SharpDX.Direct3D9.DeviceType;
using PresentInterval = SharpDX.Direct3D9.PresentInterval;
using Texture = SharpDX.Direct3D9.Texture;

namespace SRL.MonoGameControl
{
    /// <summary>
    /// Represents a Direct3D 9 device required for Direct3D 11 interoperability.
    /// </summary>
    /// <remarks>
    /// It is not possible to set a Direct3D 11 resource in WPF directly because
    /// WPF requires Direct3D 9. The <see cref="D3D9"/> class creates a new Direct3D 9 
    /// device which can be used for sharing resources between Direct3D 11 and Direct3D 9.
    /// Call <see cref="GetSharedTexture"/> to convert a texture from Direct3D 11 to Direct3D 9.
    /// Code source: https://github.com/CartBlanche/MonoGame-Samples/tree/master/WpfInteropSample
    /// </remarks>
    internal class D3D9 : IDisposable
    {
        private bool _disposed;
        private Direct3DEx _direct3D;
        private DeviceEx _device;

        public D3D9()
        {
            _direct3D = new Direct3DEx();

            PresentParameters presentparams = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                PresentationInterval = PresentInterval.Default,

                BackBufferFormat = Format.Unknown,
                BackBufferWidth = 1,
                BackBufferHeight = 1,

                DeviceWindowHandle = GetDesktopWindow()
            };

            _device = new DeviceEx(_direct3D, 0, DeviceType.Hardware, IntPtr.Zero,
                                   CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve,
                                   presentparams);
        }

        ~D3D9()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by an instance of the <see cref="D3D9"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by an instance of the <see cref="D3D9"/> class 
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; 
        /// <see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_device != null)
                    {
                        _device.Dispose();
                        _device = null;
                    }
                    if (_direct3D != null)
                    {
                        _direct3D.Dispose();
                        _direct3D = null;
                    }
                }

                _disposed = true;
            }
        }

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetDesktopWindow();

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        /// <summary>
        /// Creates Direct3D 9 texture from the specified Direct3D 11 texture. 
        /// (The content is shared between the devices.)
        /// </summary>
        /// <param name="renderTarget">The Direct3D 11 texture.</param>
        /// <returns>The Direct3D 9 texture.</returns>
        /// <exception cref="ArgumentException">
        /// The Direct3D 11 texture is not a shared resource, or the texture format is not 
        /// supported.
        /// </exception>
        public Texture GetSharedTexture(Texture2D renderTarget)
        {
            ThrowIfDisposed();

            if (renderTarget == null)
                return null;

            IntPtr handle = renderTarget.GetSharedHandle();
            if (handle == IntPtr.Zero)
                throw new ArgumentException("Unable to access resource. The texture needs to be created as a shared resource.", "renderTarget");

            Format format;
            switch (renderTarget.Format)
            {
                case SurfaceFormat.Bgr32:
                    format = Format.X8R8G8B8;
                    break;
                case SurfaceFormat.Bgra32:
                    format = Format.A8R8G8B8;
                    break;
                default:
                    throw new ArgumentException("Unexpected surface format. Supported formats are: SurfaceFormat.Bgr32, SurfaceFormat.Bgra32.", "renderTarget");
            }

            return new Texture(_device, renderTarget.Width, renderTarget.Height, 1, Usage.RenderTarget, format, Pool.Default, ref handle);
        }
    }
}