using KinectDemo2.Custom.Model.kinect;
using KinectDemo2.Custom.Service;
using KinectDemo2.Custom.Service.Python.ML;
using SkiaSharp;

namespace KinectDemo2.Custom.Helper
{
    internal interface IPosePrediction
    {
        List<PoseLandmark> Predict(
            SKBitmap input_image,
            out SKBitmap output_image
            );
    }

    internal abstract class PosePredictionModel : IPosePrediction
    {
        protected readonly IPythonService _PythonService = App.PythonService;
        public Action<ProgressCallbackArguments> ProgressCallback { protected get; set; }
        public CancellationToken CancellationToken { protected get; set; }
        public static PosePredictionModel GetInstance(MLModel modelName)
        {
            return modelName switch 
            { 
                MLModel.MOVENET_LIGHTNING => new MoveNetLightning(),
                MLModel.MOVENET_THUNDER => new MoveNetThunder(),
                _ => new MediaPipe()
            };
        }
        public PosePredictionModel SetCancellationToken(CancellationToken token)
        {
            CancellationToken = token;
            return this;
        }
        public PosePredictionModel SetCallBack(Action<ProgressCallbackArguments> callback)
        {
            ProgressCallback = callback;
            return this;
        }
        public abstract List<PoseLandmark> Predict(SKBitmap input_image, out SKBitmap output_image);
    }

    internal abstract class MoveNetBase : PosePredictionModel
    {
        public bool UseLocal { get; set; }

        public override abstract List<PoseLandmark> Predict(SKBitmap input_image, out SKBitmap output_image);
    }

    internal class MediaPipe : PosePredictionModel
    {
        public override List<PoseLandmark> Predict(SKBitmap input_image, out SKBitmap output_image)
        {
            return _PythonService.MediaPipe(input_image, out output_image, ProgressCallback);
        }
    }

    internal class MoveNetLightning : MoveNetBase
    {
        public override List<PoseLandmark> Predict(SKBitmap input_image, out SKBitmap output_image)
        {
            return _PythonService.MoveNet(MLModel.MOVENET_LIGHTNING, input_image, out output_image, UseLocal, ProgressCallback);
        }
    }

    internal class MoveNetThunder : MoveNetBase
    {
        public override List<PoseLandmark> Predict(SKBitmap input_image, out SKBitmap output_image)
        {
            return _PythonService.MoveNet(MLModel.MOVENET_THUNDER, input_image, out output_image, UseLocal, ProgressCallback);
        }
    }
}
