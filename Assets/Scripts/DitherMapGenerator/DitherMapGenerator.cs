using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    [System.Serializable]
    public class DitherMapGenerator {

        int[] dither2, dither4, dither8, dither16;
        List<int[]> ditherMatrixs;

        [SerializeField, Range (0, 3)]
        int _iteration = 0;
        public int iteration {
            get {
                return _iteration;
            }
            set {
                _iteration = value;
                _dimensions = 2 << _iteration;
                _matrixSize = _dimensions * _dimensions;
            }
        }

        int _dimensions = 2;
        public int dimensions { get { return _dimensions; } }

        int _matrixSize = 4;
        public int matrixSize { get { return _matrixSize; } }

        public bool testThreshold = false;
        [Range (0, 255)]
        public int threshold = 0;

        public DitherMapGenerator () {
            dither2 = new int[] { 0, 2, 3, 1 };

            int n = 2;
            int n2 = n * 2;
            dither4 = new int[16];
            for (int x = 0; x < n; x++) {
                for (int y = 0; y < n; y++) {
                    dither4[x + y * n2] = 4 * dither2[x + y * n];
                    dither4[x + n + y * n2] = 4 * dither2[x + y * n] + 2;
                    dither4[x + (y + n) * n2] = 4 * dither2[x + y * n] + 3;
                    dither4[x + n + (y + n) * n2] = 4 * dither2[x + y * n] + 1;
                }
            }

            n = 4;
            n2 = n * 2;
            dither8 = new int[64];
            for (int x = 0; x < n; x++) {
                for (int y = 0; y < n; y++) {
                    dither8[x + y * n2] = 4 * dither4[x + y * n];
                    dither8[x + n + y * n2] = 4 * dither4[x + y * n] + 2;
                    dither8[x + (y + n) * n2] = 4 * dither4[x + y * n] + 3;
                    dither8[x + n + (y + n) * n2] = 4 * dither4[x + y * n] + 1;
                }
            }

            n = 8;
            n2 = n * 2;
            dither16 = new int[256];
            for (int x = 0; x < n; x++) {
                for (int y = 0; y < n; y++) {
                    dither16[x + y * n2] = 4 * dither8[x + y * n];
                    dither16[x + n + y * n2] = 4 * dither8[x + y * n] + 2;
                    dither16[x + (y + n) * n2] = 4 * dither8[x + y * n] + 3;
                    dither16[x + n + (y + n) * n2] = 4 * dither8[x + y * n] + 1;
                }
            }

            ditherMatrixs = new List<int[]> () { dither2, dither4, dither8, dither16 };
        }

        public Texture2D CreateTexture (int resolution) {
            Texture2D texture = new Texture2D (resolution, resolution, TextureFormat.RGB24, false);
            texture.name = "Procedural Texture";
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;
            Apply (texture);
            return texture;
        }

        public void Apply (Texture2D texture) {
            int resolution = texture.width;
            int scale = resolution / dimensions;
            int[] ditherMatrix = ditherMatrixs[iteration];
            for (int y = 0; y < resolution; y++) {
                for (int x = 0; x < resolution; x++) {
                    int index = (x / scale) + (y / scale) * dimensions;
                    float value = (float) ditherMatrix[index] / matrixSize + 0.5f / matrixSize;
                    if (testThreshold) {
                        if (threshold * 1f / 255f > value) {
                            value = 1;
                        } else {
                            value = 0;
                        }
                    }
                    texture.SetPixel (x, y, new Color (value, value, value));
                }
            }
            texture.Apply ();
        }
    }
}