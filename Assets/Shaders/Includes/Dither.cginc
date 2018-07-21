#ifndef DITHER_INCLUDE
#define DITHER_INCLUDE

#include "UnityCG.cginc"

inline fixed getDither (int x, int y) {
#ifdef USE_DITHER_256
    const fixed diter[256] = {0,128,32,160,8,136,40,168,2,130,34,162,10,138,42,170,192,64,224,96,200,72,232,104,194,66,226,98,202,74,234,106,48,176,16,144,56,184,24,152,50,178,18,146,58,186,26,154,240,112,208,80,248,120,216,88,242,114,210,82,250,122,218,90,12,140,44,172,4,132,36,164,14,142,46,174,6,134,38,166,204,76,236,108,196,68,228,100,206,78,238,110,198,70,230,102,60,188,28,156,52,180,20,148,62,190,30,158,54,182,22,150,252,124,220,92,244,116,212,84,254,126,222,94,246,118,214,86,3,131,35,163,11,139,43,171,1,129,33,161,9,137,41,169,195,67,227,99,203,75,235,107,193,65,225,97,201,73,233,105,51,179,19,147,59,187,27,155,49,177,17,145,57,185,25,153,243,115,211,83,251,123,219,91,241,113,209,81,249,121,217,89,15,143,47,175,7,135,39,167,13,141,45,173,5,133,37,165,207,79,239,111,199,71,231,103,205,77,237,109,197,69,229,101,63,191,31,159,55,183,23,151,61,189,29,157,53,181,21,149,255,127,223,95,247,119,215,87,253,125,221,93,245,117,213,85};
    x &= 15;
    y &= 15;
    return (diter[x + y * 16] + 1) / 257;
#else
    const fixed diter[64] = {0,32,8,40,2,34,10,42,48,16,56,24,50,18,58,26,12,44,4,36,14,46,6,38,60,28,52,20,62,30,54,22,3,35,11,43,1,33,9,41,51,19,59,27,49,17,57,25,15,47,7,39,13,45,5,37,63,31,55,23,61,29,53,21};
    x &= 7;
    y &= 7;
    return (diter[x + y * 8] + 1) / 65;
#endif
}

inline void clipBayerDither (float2 screenPos, float alpha) {
    int x = screenPos.x * _ScreenParams.x;
    int y = screenPos.y * _ScreenParams.y;
    fixed value = getDither(x, y);
    clip(alpha - value);
}

inline void clipBayerDither (float2 screenPos, float alpha, float noise) {
    int x = screenPos.x * _ScreenParams.x;
    int y = screenPos.y * _ScreenParams.y;
    fixed value = getDither(x, y) + noise;
    clip(alpha - value * 0.5);
}

inline void clipFSDither (float2 screenPos, float alpha) {
    // Fake Floyd‑Steinberg Dither
    int x = screenPos.x * _ScreenParams.x;
    int y = screenPos.y * _ScreenParams.y;

    const int lookupSize = 64;
    const float errorCarry = 0.3;
    fixed xError = 0.0;
    for(int i = 0; i < lookupSize; i++){
        fixed dither = getDither(x - i, y);
        dither += xError;
        fixed bit = step(dither, 0.5);
        xError = (dither - bit) * errorCarry;
    }
    fixed yError = 0.0;
    for(int i = 0; i < lookupSize; i++){
        fixed dither = getDither(x, y - i);
        dither += yError;
        fixed bit = step(dither, 0.5);
        yError = (dither - bit) * errorCarry;
    }
    
    fixed dither = getDither(x, y);
    dither += xError * 0.5 + yError * 0.5;
    clip (alpha - dither);
}

inline void clipDitherMap (float2 screenPos, float alpha, sampler2D ditherMap, float4 ditherMap_TexelSize) {
    float2 pos = screenPos * ditherMap_TexelSize.xy * _ScreenParams.xy;
    half mask = tex2D(ditherMap, pos).r;
    clip(alpha - mask);
}

#endif // DITHER_INCLUDE