using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Camera)), ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Dithering : MonoBehaviour {

    [Range (0f, 1f)]
    public float alpha = 1;

    Material material;

    void Start () {
        material = new Material (Shader.Find ("Hidden/Dithering"));
    }

    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        material.SetFloat ("_Alpha", alpha);
        Graphics.Blit (src, dest, material, 0);
    }
}