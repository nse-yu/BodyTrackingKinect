using Microsoft.Azure.Kinect.Sensor;
using Device = Microsoft.Azure.Kinect.Sensor.Device;
using KinectImage = Microsoft.Azure.Kinect.Sensor.Image;
using KinectFrame = Microsoft.Azure.Kinect.BodyTracking.Frame;
using KinectDemo2.Custom.Exceptions;
using Microsoft.Azure.Kinect.BodyTracking;
using SkiaSharp;
using System.Runtime.InteropServices;
using SkiaSharp.Views.Maui.Controls;
using System.Numerics;
using KinectDemo2.Custom.Model.kinect;

namespace KinectDemo2.Custom.Service
{
    public interface IKinectService
    {
        void StartCamera();
        void StopCamera();
        int LoadDevices();
        void DisposeAll();
        int GetCountOfDevices();
        HardwareVersion GetVersion();
        KinectDevice GetDefaultDevice();
        (ImageSource, ImageSource) GetSourceFromKinect();
        Calibration GetCalibration();
        KinectFrame GetBodyFrame();
        unsafe MemoryStream ConvertImageToStream(KinectImage image, SKColorType colorType, SKAlphaType alphaType = SKAlphaType.Premul);
        unsafe MemoryStream ConvertImageToStream(SKBitmap image, SKColorType colorType, SKEncodedImageFormat imageFormat);
        unsafe void ConvertImageToStream(SKBitmap image, SKColorType colorType, SKEncodedImageFormat imageFormat, out MemoryStream ms);
        ImageSource ConvertImageToSource(KinectImage image, SKColorType colorType);
        ImageSource ConvertImageToSource(SKBitmap image);
        SKBitmap DrawLandmarksOnImage(KinectImage image, List<KinectJoint> joints);
        SKBitmap DrawDynamicLandmarksOnImage(KinectImage image, List<KinectJoint> joints);
    }

    public class KinectService : IKinectService
    {
        public static readonly Dictionary<JointId, string> JOINTS = new()
        {
            { JointId.Pelvis, "骨盤" },
            { JointId.SpineNavel, "腹部脊椎" },
            { JointId.SpineChest, "胸部脊椎" },
            { JointId.Neck, "首" },
            { JointId.ClavicleLeft, "左鎖骨" },
            { JointId.ShoulderLeft, "左肩" },
            { JointId.ElbowLeft, "左肘" },
            { JointId.WristLeft, "左手首" },
            { JointId.HandLeft, "左掌" },
            { JointId.HandTipLeft, "左指先端" },
            { JointId.ThumbLeft, "左親指" },
            { JointId.ClavicleRight, "右鎖骨" },
            { JointId.ShoulderRight, "右肩" },
            { JointId.ElbowRight, "右肘" },
            { JointId.WristRight, "右手首" },
            { JointId.HandRight, "右掌" },
            { JointId.HandTipRight, "右指先端" },
            { JointId.ThumbRight, "右親指" },
            { JointId.HipLeft, "左尻" },
            { JointId.KneeLeft, "左膝" },
            { JointId.AnkleLeft, "左くるぶし" },
            { JointId.FootLeft, "左足底" },
            { JointId.HipRight, "右尻" },
            { JointId.KneeRight, "右膝" },
            { JointId.AnkleRight, "右くるぶし" },
            { JointId.FootRight, "右足底" },
            { JointId.Head, "顎" },
            { JointId.Nose, "鼻" },
            { JointId.EyeLeft, "左目" },
            { JointId.EarLeft, "左耳" },
            { JointId.EyeRight, "右目" },
            { JointId.EarRight, "右耳" },
        };
        public static readonly string ALERT_DEVICE_NOT_FOUND = "デバイスが見つかりませんでした。";
        public static readonly string ALERT_DEVICE_FOUND = "デバイスが見つかりました。";
        public static readonly string ALERT_CAMERA_NOT_STARTED = "カメラを起動してください。";
        
        private const SKColorType DEFAULT_SKCOLOR_TYPE = SKColorType.Bgra8888;

        private static readonly int sk_image_quality = 50;
        private static readonly List<(uint, uint)> JOINT_CONNECTION_PAIR = new()
        {
            (0, 1),
            (1, 2),
            (2, 3),
            (2, 4),
            (4, 5),
            (5, 6),
            (6, 7),
            (7, 8),
            (8, 9),
            (7, 10),
            (2, 11),
            (11, 12),
            (12, 13),
            (13, 14),
            (14, 15),
            (15, 16),
            (14, 17),
            (0, 18),
            (18, 19),
            (19, 20),
            (20, 21),
            (0, 22),
            (22, 23),
            (23, 24),
            (24, 25),
            (3, 26),
            (26, 27),
            (26, 28),
            (26, 29),
            (26, 30),
            (26, 31),
        };

        private readonly KinectManager _manager;
        private readonly SKCanvasView _canvasView;


        public KinectService()
        {
            _manager ??= new();
            _canvasView ??= new();
            _canvasView.PaintSurface += OnPaintSurface;
        }

        private void OnPaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            throw new NotImplementedException();
        }

        public (ImageSource, ImageSource) GetSourceFromKinect()
        {
            ImageSource srcRgb, srcDep;
            // TODO Kinectからのキャプチャ画像を外から渡すことも検討中(Dispose & 保守性対策)
            using var capture = GetDefaultDevice().Device.GetCapture();

            unsafe
            {
                srcRgb = ConvertRGBImageToSource(capture.Color);
                srcDep = ConvertDepthImageToSource(capture.Depth);
            }

            return (srcRgb, srcDep);
        }
        public KinectFrame GetBodyFrame()
        {
            var device = GetDefaultDevice();
            var calibration = GetCalibration();
            var tracker = CreateTracker(calibration);
            KinectFrame frame = null;
            try
            {
                tracker.EnqueueCapture(device.Device.GetCapture());
                frame = tracker.PopResult();
            }
            catch (Exception e)
            {
                throw new NotHandledKinectException(Exception5WQuestionsFactory.Init()
                    .What(e.Message)
                    .When(DateTime.Now)
                    .Where(e.TargetSite.Name)
                    .Get(e.GetType()));
            }
            finally
            {
                tracker.Shutdown();
            }
            return frame;
        }
        public int LoadDevices()
        {
            try
            {
                if(GetCountOfDevices() > 0) _manager.DisposeAndClearAll();
                _manager.Devices = CollectInstalledKinect();
            }
            catch (AzureKinectOpenDeviceException e)
            {
                return 0;
            }
            return GetCountOfDevices();
        }


        private ImageSource ConvertRGBImageToSource(KinectImage kinectImage)
        {
            var imageStream = ConvertImageToStream(kinectImage, DEFAULT_SKCOLOR_TYPE);
            return ImageSource.FromStream(() => new MemoryStream(imageStream.ToArray()));
        }
        private ImageSource ConvertDepthImageToSource(KinectImage kinectImage)
        {
            var imageStream = ConvertImageToStream(kinectImage, SKColorType.Argb4444);
            return ImageSource.FromStream(() => new MemoryStream(imageStream.ToArray()));
        }
        private SKBitmap CastKinectImageToSKBitmap(KinectImage kinectImage)
        {
            var skBmp = new SKBitmap(kinectImage.WidthPixels, kinectImage.HeightPixels, DEFAULT_SKCOLOR_TYPE, SKAlphaType.Premul);
            byte[] imageData = kinectImage.Memory.ToArray();
            Marshal.Copy(imageData, 0, skBmp.GetPixels(), imageData.Length);
            return skBmp;
        }
        public unsafe MemoryStream ConvertImageToStream(KinectImage image, SKColorType colorType, SKAlphaType alphaType = SKAlphaType.Premul)
        {
            var skBmp = new SKBitmap(image.WidthPixels, image.HeightPixels, colorType, SKAlphaType.Premul);
            byte[] imageData = image.Memory.ToArray();
            Marshal.Copy(imageData, 0, skBmp.GetPixels(), imageData.Length);

            return ConvertImageToStream(skBmp, colorType, SKEncodedImageFormat.Png);
        }
        public unsafe MemoryStream ConvertImageToStream(SKBitmap image, SKColorType colorType, SKEncodedImageFormat imageFormat)
        {
            if (image == null) throw new InvalidOperationException("Bitmap is not available.");

            var ms = new MemoryStream();
            image?.Encode(ms, imageFormat, quality: sk_image_quality);
            return ms;
        }
        public void ConvertImageToStream(SKBitmap image, SKColorType colorType, SKEncodedImageFormat imageFormat, out MemoryStream ms)
        {
            if (image == null) throw new InvalidOperationException("Bitmap is not available.");
            ms = new();
            image?.Encode(ms, imageFormat, quality: sk_image_quality);
        }
        public ImageSource ConvertImageToSource(KinectImage image, SKColorType colorType = DEFAULT_SKCOLOR_TYPE)
        {
            try
            {
                using var stream = ConvertImageToStream(image, colorType);
                return ImageSource.FromStream(() => new MemoryStream(stream.ToArray()));
            }
            catch (Exception e)
            {
                throw new NotHandledKinectException(e.Message);
            }
        }
        public ImageSource ConvertImageToSource(SKBitmap image)
        {
            var ms = new MemoryStream();
            try
            {
                ConvertImageToStream(image, image.ColorType, SKEncodedImageFormat.Jpeg, out ms);
                return ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
            }
            catch (Exception e)
            {
                throw new NotHandledKinectException(e.Message);
            }
        }


        public SKBitmap DrawLandmarksOnImage(KinectImage image, List<KinectJoint> joints)
        {
            using var bitmapSource = CastKinectImageToSKBitmap(image);
            var vectolizedJoints = ConvertJointsToVectors(joints);
            var normalizedVectors = NormalizeVectors(vectolizedJoints);
            
            return DrawCircleOnImage(bitmapSource, normalizedVectors);
        }
        public SKBitmap DrawDynamicLandmarksOnImage(KinectImage image, List<KinectJoint> joints)
        {
            using var bitmapSource = CastKinectImageToSKBitmap(image);
            var vectolizedJoints = ConvertJointsToVectors(joints);
            var normalizedVectors = NormalizeVectors(vectolizedJoints);
            var skCircledImage = DrawCircleOnImage(bitmapSource, normalizedVectors);

            return DrawHorizontalLevelFrameOnImage(skCircledImage, normalizedVectors);
        }
        private SKBitmap DrawCircleOnImage(SKBitmap originalBitmap, Vector2?[] vectolizedJoints)
        {
            var modifiedBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);
            using var canvas = new SKCanvas(modifiedBitmap);

            canvas.DrawBitmap(originalBitmap, new SKRect(0, 0, originalBitmap.Width, originalBitmap.Height));

            using var paint = new SKPaint()
            {
                Color = SKColors.Red,
                Style = SKPaintStyle.Fill
            };

            float radius = 10;

            vectolizedJoints.ToList().ForEach(vec =>
            {
                if(vec.HasValue) canvas.DrawCircle(vec.Value.X, vec.Value.Y, radius, paint);
            });
            JOINT_CONNECTION_PAIR.ToList().ForEach(pair =>
            {
                var startP = vectolizedJoints[pair.Item1];
                var endP = vectolizedJoints[pair.Item2];
                if(startP.HasValue && endP.HasValue) canvas.DrawLine(startP.Value.X, startP.Value.Y, endP.Value.X, endP.Value.Y, paint);
            });
            return modifiedBitmap;
        }
        private SKBitmap DrawHorizontalLevelFrameOnImage(SKBitmap originalBitmap, Vector2?[] vectolizedJoints)
        {
            var modifiedBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);
            using var canvas = new SKCanvas(modifiedBitmap);

            canvas.DrawBitmap(originalBitmap, new SKRect(0, 0, originalBitmap.Width, originalBitmap.Height));

            using var paint = new SKPaint()
            {
                Color = SKColors.Violet,
                Style = SKPaintStyle.Fill
            };

            //TODO: 水準器描画/ [評価時:両肩の差異][評価時:]
            canvas.DrawLine(100, 100, 600, 100, paint);
            //throw new NotImplementedException();
            return modifiedBitmap;
        }
        private Vector3[] ConvertJointsToVectors(List<KinectJoint> joints) => 
            joints.Select(joint => new Vector3((float)joint.PositionX, (float)joint.PositionY, (float)joint.PositionZ)).ToArray();
        private Vector2?[] NormalizeVectors(Vector3[] vectors) => 
            vectors.Select(vec => GetDefaultDevice().TransformColorToDepth(vec, CalibrationDeviceType.Depth, CalibrationDeviceType.Color)).ToArray();
        
        
        private Tracker CreateTracker(Calibration calibration) => Tracker.Create(calibration, new TrackerConfiguration()
        {
            SensorOrientation = SensorOrientation.Default,
            ProcessingMode = TrackerProcessingMode.Cpu,
            GpuDeviceId = 0
        });
        private List<KinectDevice> CollectInstalledKinect() => Enumerable.Range(0, Device.GetInstalledCount())
            .Select(i => new KinectDevice() { Device = Device.Open(i) }).ToList();


        public void StartCamera() => GetDefaultDevice().Start();
        public void StopCamera() => GetDefaultDevice().Stop();
        public void DisposeAll()
        {
            _manager.StopCamerasAll();
            _manager.DisposeAndClearAll();
        }


        public KinectDevice GetDefaultDevice() => _manager.Devices.FirstOrDefault();
        public HardwareVersion GetVersion() => GetDefaultDevice().Device.Version;
        public Calibration GetCalibration() => GetDefaultDevice().Device.GetCalibration();
        public int GetCountOfDevices() => _manager.Count;
    }
}
