import tensorflow as tf;
import ctypes;
import matplotlib.pyplot as plt
import numpy as np;
from matplotlib.collections import LineCollection
import cv2
import mediapipe as mp
from mediapipe.framework.formats import landmark_pb2
from mediapipe import solutions
from mediapipe.tasks import python
from mediapipe import ImageFormat
from mediapipe.tasks.python import vision

class MLmodule:
  def __init__(self):
    self.ptr = None
    self.color_channel = 3

  # Maps bones to a matplotlib color name.
  KEYPOINT_EDGE_INDS_TO_COLOR = {
      (0, 1): 'm',
      (0, 2): 'c',
      (1, 3): 'm',
      (2, 4): 'c',
      (0, 5): 'm',
      (0, 6): 'c',
      (5, 7): 'm',
      (7, 9): 'm',
      (6, 8): 'c',
      (8, 10): 'c',
      (5, 6): 'y',
      (5, 11): 'm',
      (6, 12): 'c',
      (11, 12): 'y',
      (11, 13): 'm',
      (13, 15): 'm',
      (12, 14): 'c',
      (14, 16): 'c'
  }



  def Hello():
    return "Hello, good working"

  def Dispose(self):
    self.ptr = None

  """
  [mp.ImageFormat]
  SRGB    : sRGB, interleaved: one byte for R, then one byte for G, then one byte for B for each pixel.
  SRGBA   : sRGBA, interleaved: one byte for R, one byte for G, one byte for B, one byte for alpha or unused.
  SBGRA   : sBGRA, interleaved: one byte for B, one byte for G, one byte for R, one byte for alpha or unused.
  GRAY8   : Grayscale, one byte per pixel.
  GRAY16  : Grayscale, one uint16 per pixel.
  SRGB48  : sRGB, interleaved, each component is a uint16.
  SRGBA64 : sRGBA, interleaved, each component is a uint16.
  VEC32F1 : One float per pixel.
  VEC32F2 : Two floats per pixel.
  """
  def MediaPipe(self, img_ptr, width, height, channel, stride, model_asset_path):
    self.color_channel = channel
    img = np.ctypeslib.as_array((stride * height * ctypes.c_uint8).from_address(img_ptr)).reshape(height, width, self.color_channel)

    # channel: 3 => BGR, 4 => BGRA
    npImage = img.copy()

    # channel: 3 => RGB, 4 => RGBA
    #npImage = MLmodule.FitCScolorByChannel(npImage)

    # STEP 0: Required to convert np images to mp images ]
    # In this line, the color channels of the input image are converted from a python-like format to a general format such as RGB or RGBA.
    # Therefore, there is no need to convert formats any further
    imageFormat = ImageFormat.SRGB if self.color_channel is 3 else ImageFormat.SRGBA
    mpImage = mp.Image(image_format=imageFormat, data=npImage)

    # Represents external model asset used by the Task APIs
    base_options = python.BaseOptions(model_asset_path=model_asset_path)

    # PoseLandmarkerOptions
    options = vision.PoseLandmarkerOptions(
        base_options=base_options,
        output_segmentation_masks=True)
    
    # PoseLandmarker.create_from_options(options) => PoseLandmarker
    detector = vision.PoseLandmarker.create_from_options(options)

    #TODO most apperance of error about image processing
    # STEP 4: Detect pose landmarks from the input image.
    # PoseLandmarker#detect(image, image_processing_options) => PoseLandmarkerResult
    detection_result = detector.detect(mpImage)
    pose_landmarks = np.array(detection_result.pose_landmarks[0])
    
    c_pose_landmarks = [[lm.x, lm.y, lm.z, lm.visibility, lm.presence] for lm in pose_landmarks]

    # Outputs:
    # Pose landmarks in normalized image coordinates
    # Pose landmarks in world coordinates
    # Optional: a segmentation mask for the pose.

    # STEP 5: Process the detection result. In this case, visualize it.
    # TODO Currently, requires to provide rgb image, not argb.
    annotated_image = MLmodule.draw_landmarks_on_image(mpImage.numpy_view(), detection_result)
    #MLmodule.show(annotated_image)
    
    imHeight = annotated_image.shape[0]
    imWidth = annotated_image.shape[1]
    imStride = annotated_image.strides[0]
    self.ptr = MLmodule.image_to_pointer(annotated_image)
    annotated_image = annotated_image.flatten()

    return [[annotated_image, imHeight, imWidth, imStride], c_pose_landmarks]
  
  def MoveNet(self, img_ptr, width, height, channel, stride, input_size, model):
      self.color_channel = channel

      img = np.ctypeslib.as_array((stride * height * ctypes.c_uint8).from_address(img_ptr)).reshape(height, width, channel)
      img = MLmodule.FitCsColorByChannel(img)
      img = MLmodule.rgba_to_rgb(img)
      input_image = img.copy()
      #MLmodule.show(input_image)

      #========Resize and pad the image to keep the aspect ratio and fit the expected size========#
      # 1. Expand tensor dims: 
      # input_image.shape = (height, width, 3) ----> (1, height, width, 3)
      input_image = tf.expand_dims(input_image, axis=0)

      # 2. Convert image size to expected height and width maintaining its aspect ratio:
      # The expected input_size depends on models you use
      input_image = tf.image.resize_with_pad(input_image, input_size, input_size)

      # 3. Cast tensor type:
      # SavedModel format expects tensor type of int32.
      input_image = tf.cast(input_image, dtype=tf.int32)

      # input_image: 
      # A [1, height, width, 3] tensor represents the input image pixels. Note that the height/width should already 
      # be resized and match the expected input resolution of the model before passing into this function.
      outputs = model(input_image)

      # Returns:
      # A [1, 1, 17, 3] double numpy array representing the predicted keypoint coordinates and scores.
      keypoints_with_scores = outputs['output_0'].numpy()

      #====================================================
      result_image = MLmodule.draw_prediction_on_image(img, keypoints_with_scores)
      result_image = MLmodule.rgb_to_rgba(result_image)
      result_image = MLmodule.FitPyColorByChannel(result_image)
      result_image = result_image.astype("uint8")
      result_image = cv2.resize(result_image, dsize=(img.shape[1], img.shape[0]), interpolation=cv2.INTER_CUBIC)

      imHeight = result_image.shape[0]
      imWidth = result_image.shape[1]
      imStride = result_image.strides[0]
      self.ptr = MLmodule.image_to_pointer(result_image)
      result_image = result_image.flatten()
      #====================================================

      return [keypoints_with_scores[0][0], [result_image, imHeight, imWidth, imStride]]

  @staticmethod
  def draw_landmarks_on_image(image, detection_result):

    #if image.shape[2] is not 3:
    #  raise Exception("[nse-yu] You must pass RGB(BGR) images, so please remove the alpha channel from the image.")
    
    pose_landmarks_list = detection_result.pose_landmarks
    annotated_image = np.copy(image)
    annotated_image = cv2.cvtColor(annotated_image, cv2.COLOR_RGBA2RGB) if image.shape[2] is 4 else annotated_image 

    # Loop through the detected poses to visualize.
    for idx in range(len(pose_landmarks_list)):
      pose_landmarks = pose_landmarks_list[idx]

      # Draw the pose landmarks.
      pose_landmarks_proto = landmark_pb2.NormalizedLandmarkList()
      pose_landmarks_proto.landmark.extend([
        landmark_pb2.NormalizedLandmark(x=landmark.x, y=landmark.y, z=landmark.z) for landmark in pose_landmarks
      ])
      
      # must provide rgb images
      solutions.drawing_utils.draw_landmarks(
        annotated_image,
        pose_landmarks_proto,
        solutions.pose.POSE_CONNECTIONS,
        solutions.drawing_styles.get_default_pose_landmarks_style())
      
    annotated_image = cv2.cvtColor(annotated_image, cv2.COLOR_RGB2RGBA) if image.shape[2] is 4 else annotated_image 
    return annotated_image

  @staticmethod
  def _keypoints_and_edges_for_display(keypoints_with_scores,
                                      height,
                                      width,
                                      keypoint_threshold=0.11):
    keypoints_all = []
    keypoint_edges_all = []
    edge_colors = []

    # [1, 1, 17, 3] is expected
    num_instances, _, _, _ = keypoints_with_scores.shape

    # only one loop
    for idx in range(num_instances):
      # 17個の関節のX座標のみの配列: shape = (17,)
      kpts_x = keypoints_with_scores[0, idx, :, 1]

      # 17個の関節のy座標のみの配列
      kpts_y = keypoints_with_scores[0, idx, :, 0]
      
      # 17個の関節のスコアのみの配列
      kpts_scores = keypoints_with_scores[0, idx, :, 2]

      # combine [x0,x1,...] and [y0,y1,...] -> [[x0,y0],[x1,y1],...]
      kpts_absolute_xy = np.stack([width * np.array(kpts_x), height * np.array(kpts_y)], axis=-1)
      
      # scoreが閾値より大きい関節のみフィルタ
      kpts_above_thresh_absolute = kpts_absolute_xy[kpts_scores > keypoint_threshold, :]
      
      # 追加
      keypoints_all.append(kpts_above_thresh_absolute)

      # Ex. edge_pair, color = (1, 3), 'm'
      # edge_pair構造：(線分始点のidx, 線分終点のidx)
      for edge_pair, color in MLmodule.KEYPOINT_EDGE_INDS_TO_COLOR.items():
        # 関節線分描画のための処理 - ペアのスコアを取り出し、閾値以上の信頼度のみ座標登録
        if (kpts_scores[edge_pair[0]] > keypoint_threshold and kpts_scores[edge_pair[1]] > keypoint_threshold):
          
          # 線分始点idxの0(X)と1(Y)
          x_start = kpts_absolute_xy[edge_pair[0], 0]
          y_start = kpts_absolute_xy[edge_pair[0], 1]
          
          # 線分終点idxの0(X)と1(Y)
          x_end = kpts_absolute_xy[edge_pair[1], 0]
          y_end = kpts_absolute_xy[edge_pair[1], 1]
          
          # to array
          line_seg = np.array([[x_start, y_start],[x_end, y_end]])
          
          # Ex. [ [[Xst0,Yst0],[Xen0,Yen0]], [[Xst1,Yst1],[Xen1,Yen1]],...]]
          keypoint_edges_all.append(line_seg)
          edge_colors.append(color)

    if keypoints_all:
      keypoints_xy = np.concatenate(keypoints_all, axis=0)
    else:
      keypoints_xy = np.zeros((0, 17, 2))

    if keypoint_edges_all:
      edges_xy = np.stack(keypoint_edges_all, axis=0)
    else:
      edges_xy = np.zeros((0, 2, 2))

    return keypoints_xy, edges_xy, edge_colors

  @staticmethod
  def draw_prediction_on_image(image, keypoints_with_scores, close_figure=False):
    """Draws the keypoint predictions on image.

    Args:
      image: A numpy array with shape [height, width, channel] representing the
        pixel values of the input image.
      keypoints_with_scores: A numpy array with shape [1, 1, 17, 3] representing
        the keypoint coordinates and scores returned from the MoveNet model.
      crop_region: A dictionary that defines the coordinates of the bounding box
        of the crop region in normalized coordinates (see the init_crop_region
        function below for more detail). If provided, this function will also
        draw the bounding box on the image.
      output_image_height: An integer indicating the height of the output image.
        Note that the image aspect ratio will be the same as the input image.

    Returns:
      A numpy array with shape [out_height, out_width, channel] representing the
      image overlaid with keypoint predictions.
    """
    # 画像の高さ、幅、チャンネル(ex. RGB => 3)を取得
    height, width, channel = image.shape

    # 取得した高さ、幅からアスペクト比を保持
    aspect_ratio = float(width) / height

    # 幅：12*アスペクト比*100, 高さ：12*100 [px] 
    fig, ax = plt.subplots(figsize=(width / 100, height / 100))

    # なんかの白い太い傍線をなくすためらしい
    plt.axis('off')
    #plt.axis('tight')
    fig.tight_layout(pad=0)
    #fig.subplots_adjust(left=0, right=1, bottom=0, top=1)
    ax.margins(0)
    ax.set_yticklabels([])
    ax.set_xticklabels([])

    # Axes上に表示した画像を表す戻り値, Axesは描画領域を表す
    im = ax.imshow(image)

    # from matplotlib.collections import LineCollection
    # 一度に複数の線を描画するための便利な関数
    # 線分を示す座標を与えると、指定した線分スタイルをもとに描画
    line_segments = LineCollection([], linewidths=(4), linestyle='solid')
    ax.add_collection(line_segments)

    # Turn off tick labels
    scat = ax.scatter([], [], s=60, color='#FF1493', zorder=3)

    # All points are converted to absolute positions
    (keypoint_locs, keypoint_edges, edge_colors) = MLmodule._keypoints_and_edges_for_display(keypoints_with_scores, height, width)

    # Keypoint_edges expected shape is (17, 2, 2); 17->all joints, 2->(start, end), 2->(x, y)
    # But, probably not all joints are passed because of score thresholds

    # 計算した線分座標リストをセット
    line_segments.set_segments(keypoint_edges)

    # それに対応する色をセット
    line_segments.set_color(edge_colors)

    # One or more edges are reliable
    if keypoint_edges.shape[0]:
      line_segments.set_segments(keypoint_edges)
      line_segments.set_color(edge_colors)

    # One or more points are reliable
    if keypoint_locs.shape[0]:
      scat.set_offsets(keypoint_locs)

    fig.canvas.draw()
    image_from_plot = np.frombuffer(fig.canvas.tostring_rgb(), dtype=np.uint8)
    image_from_plot = image_from_plot.reshape(fig.canvas.get_width_height()[::-1] + (3,))
    plt.close(fig)
    
    return image_from_plot

  @staticmethod
  def image_to_pointer(image : np.ndarray):
      imHeight = image.shape[0]
      imWidth = image.shape[1]
      #result_image_data = image.ctypes.data_as(ctypes.POINTER(((ctypes.c_uint8 * image.shape[2]) * imWidth) * imHeight)).contents
      data = image.ctypes.data_as(ctypes.POINTER(((ctypes.c_uint8 * image.shape[2]) * imWidth) * imHeight))
      ptr = ctypes.addressof(data)
      #ptr = ctypes.c_void_p(image.ctypes.data)
      return ptr

  @staticmethod
  def rgba_to_rgb(rgba_image):
    rgb_image = cv2.cvtColor(rgba_image, cv2.COLOR_RGBA2RGB)
    return rgb_image

  @staticmethod
  def rgb_to_rgba(rgb_image):
    rgba_image = cv2.cvtColor(rgb_image, cv2.COLOR_RGB2RGBA)
    return rgba_image
  
  @staticmethod
  def FitCsColorByChannel(img : np.ndarray):
    channel = img.shape[2]
    if channel is 3:
      return cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    elif channel is 4:
      return cv2.cvtColor(img, cv2.COLOR_BGRA2RGBA)

  @staticmethod
  def FitPyColorByChannel(img : np.ndarray):
    channel = img.shape[2]
    if channel is 3:
      return cv2.cvtColor(img, cv2.COLOR_RGB2BGR)
    elif channel is 4:
      return cv2.cvtColor(img, cv2.COLOR_RGBA2BGRA)

  @staticmethod
  def show(image):
    plt.figure(figsize=(5,5))
    plt.imshow(image)
    plt.show()

  c_style_landmarks = lambda lm: [lm.x, lm.y, lm.z, lm.visibility, lm.presence]
