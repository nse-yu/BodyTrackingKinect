import numpy as np
import sys
import cv2

ar = [[[
[0.3652285, 0.47498566, 0.72223186],
[0.3420325, 0.5015627, 0.6947305],
[0.34201902, 0.4488368, 0.86470896],
[0.36389828, 0.5390702, 0.66152555],
[0.36234197, 0.42144275, 0.7903046],
[0.47105074, 0.61311126, 0.8062645],
[0.48165533, 0.35624918, 0.8398951],
[0.63922316, 0.73941827, 0.81816685],   
]]]

np_ar = np.array(ar)
row0 = np_ar[0,0,:,0]
row1 = np_ar[0,0,:,1]
row2 = np_ar[0,0,:,2]

row01 = np.stack([row0, row1], axis=-1)
print(row01)

row01_thresh = row01[row2 > 0.8, :]
print(row01_thresh)

ar2 = [
    [[0, 1],[2, 3]],
    [[4, 5],[6, 7]],
    [[8, 9],[10,11]]
]

np_ar2 = np.array(ar2)
print(np_ar2)
print(np.stack(np_ar2, axis=0))

print(np.concatenate(row01_thresh, axis=0))
print(cv2.__version__)
print(sys.path)