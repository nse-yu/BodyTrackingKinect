using KinectDemo2.Custom.Exceptions;
using KinectDemo2.Custom.Helper;
using KinectDemo2.Custom.Model.kinect;
using KinectDemo2.Custom.Service.Python;
using KinectDemo2.Custom.Service.Python.ML;
using Python.Runtime;
using SkiaSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static KinectDemo2.Custom.Model.kinect.KeyPointsWithScores;

namespace KinectDemo2.Custom.Service
{
    namespace Python
    {
        public enum PYLIB
        {
            NUMPY,
            MATPLOTLIB_PYPLOT,
            MATPLOTLIB_JAP,
            TENSORFLOW,
            TENSORFLOW_HUB,
            CTYPES,
            MY_MODULE,
            SYS
        }

        namespace ML
        {
            public enum MLModel
            {
                MOVENET_LIGHTNING,
                MOVENET_THUNDER,
                MEDIAPIPE
            }
        }
    }
    public interface IPythonService
    {
        void Init();
        void ImportLibrary(bool ML,params PYLIB[] libs);
        void PlotJoints(KinectJoint[] joints);
        void Shutdown();
        void DisposeModel();
        bool HasAll(params PYLIB[] libs);
        List<PoseLandmark> MediaPipe(SKBitmap input_image, out SKBitmap pointed_image, Action<ProgressCallbackArguments> progressCallback);
        List<PoseLandmark> MoveNet(MLModel modelName, SKBitmap input_image, out SKBitmap output_image, bool IsLocalModel, Action<ProgressCallbackArguments> progressCallback);
    }
    public class PythonService : IPythonService
    {
        private const string PY_LIB_MATPLOTLIB_JAP = "japanize_matplotlib";
        private const string PY_LIB_NUMPY = "numpy";
        private const string PY_LIB_MATPLOTLIB_PYPLOT = "matplotlib.pyplot";
        private const string PY_LIB_TENSORFLOW_HUB = "tensorflow_hub";
        private const string PY_LIB_TENSORFLOW = "tensorflow";
        private const string PY_LIB_CTYPES = "ctypes";
        private const string PY_LIB_MY_MODULE = "module";
        private const string PY_LIB_SYS = "sys";

        private const string URL_MOVENET_LIGHTNING = "https://tfhub.dev/google/movenet/singlepose/lightning/4";
        private const string URL_MOVENET_THUNDER = "https://tfhub.dev/google/movenet/singlepose/thunder/4";

        private const string URL_LOCAL_MOVENET_LIGHTNING = $"{THIS_PROJECT}\\Model\\movenet_singlepose_lightning_4";
        private const string URL_LOCAL_MOVENET_THUNDER = $"{THIS_PROJECT}\\Model\\movenet_singlepose_thunder_4";
        private const string URL_LOCAL_MEDIAPIPE = $"{THIS_PROJECT}\\Model\\pose_landmarker_lite\\pose_landmarker_lite.task";
        
        private const string THIS_PROJECT = @"C:\Users\boban\Documents\programming\maui\KinectDemo2\KinectDemo2";
        private const string PYTHON_DLL_NAME = "python311.dll";
        private const string PYTHON_MODULE = $"{THIS_PROJECT}\\PyModule\\module";
        private const string pathToVirtualEnv = $"{THIS_PROJECT}\\venv";

        private dynamic _np;
        private dynamic _plt;
        private dynamic _tf;
        private dynamic _hub;
        private dynamic _ctypes;
        private dynamic _module;
        private dynamic _sys;
        private dynamic model;

        public bool IsInitialized = false;

        ~PythonService()
        {
            Shutdown();
        }
        

        public void PlotJoints(KinectJoint[] joints)
        {
            dynamic fig = _plt.figure(facecolor: "white", figsize: new int[] { 12, 9 });
            dynamic ax = fig.add_subplot(111, projection: "3d");

            var xs = joints.Select(j => j.PositionX).ToArray();
            var ys = joints.Select(j => j.PositionY).ToArray();
            var zs = joints.Select(j => j.PositionZ).ToArray();

            Enumerable.Range(0, xs.Length).ToList().ForEach(i =>
            {
                ax.text(xs[i], ys[i], zs[i], KinectService.JOINTS[joints[i].Id], color: "red", fontsize: 6);
            });

            ax.view_init(elev: 250, azim: 135 + 150);
            ax.scatter(xs, ys, zs);
            ax.set_xlabel("positionX");
            ax.set_ylabel("positionY");
            ax.set_zlabel("positionZ");

            _plt.show();
        }
        public List<PoseLandmark> MediaPipe(SKBitmap input_image, out SKBitmap pointed_image, Action<ProgressCallbackArguments> progressCallback)
        {
            ThrowIfNoModule();

            List<PoseLandmark> cs_pose_landmarks = null;
            SKBitmap rgb_input_image = new(input_image.Width, input_image.Height);
            dynamic mpImageData;
            dynamic mpPoseLandmarks;
            dynamic ml_module = null;
            pointed_image = null;

            try
            {
                var img_ptr = input_image.GetPixels().ToInt64();

                CallbackOnMainThread(progressCallback, new ProgressCallbackArguments() { progress = .5, message = $"Loading a MediaPipe model..." });

                using (Py.GIL())
                {
                    // Load the module class
                    ml_module = _module.MLmodule();

                    CallbackOnMainThread(progressCallback, new ProgressCallbackArguments() { progress = .8, message = $"Predicting..." });

                    dynamic result = ml_module.MediaPipe(img_ptr, input_image.Width, input_image.Height, 4, input_image.RowBytes, URL_LOCAL_MEDIAPIPE);

                    mpImageData = result[0];
                    mpPoseLandmarks = result[1];

                    // retrieve the joint position result of the posture detection
                    cs_pose_landmarks = Enum
                        .GetValues(typeof(PoseLandmarkKey))
                        .Cast<PoseLandmarkKey>()
                        .Select((v, i) => new PoseLandmark()
                        {
                            X = mpPoseLandmarks[i][0],
                            Y = mpPoseLandmarks[i][1],
                            Z = mpPoseLandmarks[i][2],
                            Visibility = mpPoseLandmarks[i][3],
                            Presence = mpPoseLandmarks[i][4],
                            Key = v
                        }).ToList();

                    var height = (int)mpImageData[1];
                    var width = (int)mpImageData[2];
                    var stride = (int)mpImageData[3];
                    var image = mpImageData[0];

                    // Initialize the output image
                    pointed_image = new(width, height, input_image.ColorType, input_image.AlphaType); ;

                    //=============== [6.0s] Flatten image to byte array (no nest of loops) ===============//
                    // Resized: 50~60% -> 3.0s
                    byte[] imageData = new byte[pointed_image.ByteCount];
                    for (int i = 0; i < imageData.Length; i++)
                    {
                        imageData[i] = (byte)image[i];
                    }
                    pointed_image = ByteArrayToSKBitmap(imageData, width, height, input_image.ColorType, input_image.AlphaType);
                }
            }
            catch (Exception e)
            {
                if (e is PythonException pyex)
                {
                    CallbackOnMainThread(progressCallback, new ProgressCallbackArguments() { progress = 1.0, message = $"Failed..." });
                    return null;
                }
                throw new NotHandledPythonException(Exception5WQuestionsFactory
                    .Init().What("Error has detected while handling the image prediction with MediaPipe.").When(DateTime.Now).Where(e.Source).Get(e.GetType()));
            }

            CallbackOnMainThread(progressCallback, new ProgressCallbackArguments() { progress = 1.0, message = $"Complete" });
            return cs_pose_landmarks;
        }
        public List<PoseLandmark> MoveNet(MLModel modelName, SKBitmap input_image, out SKBitmap output_image, bool IsLocalModel, Action<ProgressCallbackArguments> progressCallback)
        {
            ThrowIfNoModule();

            List<PoseLandmark> cs_points_and_scores = null;
            dynamic keypoints_and_scores;
            dynamic keypoints_and_image;
            dynamic ml_module = null;

            try
            {
                var img_ptr = input_image.GetPixels().ToInt64();

                string[] locations = modelName == MLModel.MOVENET_LIGHTNING ?
                    new[] { URL_LOCAL_MOVENET_LIGHTNING, URL_MOVENET_LIGHTNING } :
                    new[] { URL_LOCAL_MOVENET_THUNDER, URL_MOVENET_THUNDER };
                var location = IsLocalModel ? locations[0] : locations[1];
                var input_size = modelName == MLModel.MOVENET_LIGHTNING ? 192 : 256;

                CallbackOnMainThread(progressCallback, new ProgressCallbackArguments() { progress = .5, message = $"Loading a{(IsLocalModel ? " local" : "")} model..." });

                using (Py.GIL())
                {
                    // Load the module class
                    ml_module = _module.MLmodule();

                    // run models and retrieve outputs
                    model ??= LoadModel(location);

                    CallbackOnMainThread(progressCallback, new ProgressCallbackArguments() { progress = .8, message = $"Predicting..." });

                    // Output shape is (1, 17, 3)
                    // Ex. idx0 (nose) -> [y, x, point]
                    dynamic result = ml_module.MoveNet(img_ptr, input_image.Width, input_image.Height, 4, input_image.RowBytes, input_size, model);

                    keypoints_and_scores = result[0];
                    keypoints_and_image = result[1];

                    cs_points_and_scores = Enum
                        .GetValues(typeof(MoveNetJointKey))
                        .Cast<MoveNetJointKey>()
                        .Select((v, i) => new PoseLandmark()
                        {
                            X = keypoints_and_scores[i][1],
                            Y = keypoints_and_scores[i][0],
                            Z = 0f,
                            Visibility = keypoints_and_scores[i][2],
                            Presence = keypoints_and_scores[i][2],
                            Key = PoseLandmark.MOVENET_TO_POSE[v]
                        }).ToList();

                    var height = (int)keypoints_and_image[1];
                    var width = (int)keypoints_and_image[2];
                    var stride = (int)keypoints_and_image[3];
                    var image = keypoints_and_image[0];

                    output_image = new(width, height, input_image.ColorType, input_image.AlphaType);

                    //=============== [6.0s] Flatten image to byte array (no nest of loops) ===============//
                    // Resized: 50~60% -> 3.0s
                    byte[] imageData = new byte[output_image.ByteCount];
                    for (int i = 0; i < imageData.Length; i++)
                    {
                        imageData[i] = (byte)image[i];
                    }
                    output_image = ByteArrayToSKBitmap(imageData, width, height, input_image.ColorType, input_image.AlphaType);
                }
            }
            catch (Exception e)
            {
                throw new NotHandledPythonException("Probabry due to the image conversion for MauiImage into Tensor:\r\n" + e.Message);
            }
            finally
            {
                CallbackOnMainThread(progressCallback, new ProgressCallbackArguments() { progress = 1.0, message = $"Complete" });
            }
            return cs_points_and_scores;
        }


        private static void CallbackOnMainThread(Action<ProgressCallbackArguments> action, ProgressCallbackArguments args)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                action(args);
            });
        }
        private static Bitmap ByteArrayToBitmap(byte[] imageData, int width, int height)
        {
            Bitmap bitmap;
            using (MemoryStream stream = new(imageData))
            {
                // MemoryStreamからBitmapを生成
                bitmap = new(width, height, PixelFormat.Format24bppRgb);
                BitmapData bmpData = bitmap.LockBits
                    (
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format24bppRgb
                    );

                // Bitmapにbyte配列のデータをコピー
                Marshal.Copy(imageData, 0, bmpData.Scan0, imageData.Length);

                bitmap.UnlockBits(bmpData);
            }
            return bitmap;
        }
        private static SKBitmap ByteArrayToSKBitmap(byte[] imageData, int width, int height, SKColorType colorType, SKAlphaType alphaType)
        {
            using MemoryStream stream = new(imageData);
            SKBitmap bitmap = new(width, height, colorType, alphaType);
            unsafe
            {
                Marshal.Copy(imageData, 0, bitmap.GetPixels(), imageData.Length);
            }
           return bitmap;
        }


        public void Init()
        {
            if (IsInitialized) return;

            Runtime.PythonDLL = $"{pathToVirtualEnv}\\{PYTHON_DLL_NAME}";

            var path = ConcatEnvPath(GetEnvPath(), pathToVirtualEnv);
            var str = GetEnvPythonPath();
            Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONHOME", pathToVirtualEnv, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONPATH", $"{str};{THIS_PROJECT}\\PyModule;{THIS_PROJECT}\\PyModule\\module", EnvironmentVariableTarget.Process);

            if (!PythonEngine.IsInitialized) PythonEngine.Initialize();
            PythonEngine.PythonPath = Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);
            PythonEngine.PythonHome = pathToVirtualEnv;
            PythonEngine.BeginAllowThreads();

            IsInitialized = true;
        }
        private dynamic LoadModel(string location)
        {
            dynamic modelFromHub = _hub.load(location);
            return modelFromHub.signatures["serving_default"];
        }
        public void ImportLibrary(bool ML, params PYLIB[] libs)
        {
            using (Py.GIL())
            {
                foreach (var lib in libs)
                {
                    try
                    {
                        if (lib == PYLIB.SYS)
                        {
                            _sys = Py.Import(PY_LIB_SYS);
                            continue;
                        }
                        if (ML)
                        {
                            if (lib == PYLIB.TENSORFLOW) _tf ??= Py.Import(PY_LIB_TENSORFLOW);
                            else if (lib == PYLIB.TENSORFLOW_HUB) _hub ??= Py.Import(PY_LIB_TENSORFLOW_HUB);
                            else if (lib == PYLIB.NUMPY) _np ??= Py.Import(PY_LIB_NUMPY);
                            else if (lib == PYLIB.CTYPES) _ctypes ??= Py.Import(PY_LIB_CTYPES);
                            else if (lib == PYLIB.MY_MODULE) _module ??= Py.Import(PY_LIB_MY_MODULE);
                            continue;
                        }
                        if (lib == PYLIB.MATPLOTLIB_PYPLOT)
                        {
                            _plt ??= Py.Import(PY_LIB_MATPLOTLIB_PYPLOT);
                            Py.Import(PY_LIB_MATPLOTLIB_JAP);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new NotHandledPythonException($"[{lib}] {_sys.path}: {e.Message}");
                    }
                }
            }
        }
        public bool HasAll(params PYLIB[] libs)
        {
            return libs.All(lib =>
            {
                return lib switch
                {
                    PYLIB.NUMPY => _np != null,
                    PYLIB.MATPLOTLIB_PYPLOT => _plt != null,
                    PYLIB.TENSORFLOW => _tf != null,
                    PYLIB.TENSORFLOW_HUB => _hub != null,
                    PYLIB.CTYPES => _ctypes != null,
                    PYLIB.MY_MODULE => _module != null,
                    PYLIB.SYS => _sys != null,
                    _ => true,
                };
            });
        }
        private void ThrowIfNoModule()
        {
            if (_module == null) throw new NotHandledPythonException("Module is not initialized, please call LoadModel(). ");
        }


        public void DisposeModel() => model = null;
        public void Shutdown() => PythonEngine.Shutdown();


        private string GetEnvPath() => Environment.GetEnvironmentVariable("PATH").TrimEnd(';');
        private string GetEnvPythonPath() => Environment.GetEnvironmentVariable("PYTHONPATH");
        private string ConcatEnvPath(string path1, string path2) => string.IsNullOrEmpty(path1) ? path2 : path1 + ";" + path2;
    }
}