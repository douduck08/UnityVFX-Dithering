using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Camera)), ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class GrayScale : MonoBehaviour {

    Material material;

    void Start () {
        material = new Material (Shader.Find ("Hidden/GrayScale"));
    }

    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        Graphics.Blit (src, dest, material, 0);
    }
}