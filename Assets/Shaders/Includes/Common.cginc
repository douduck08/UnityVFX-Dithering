#ifndef COMMON_INCLUDE
#define COMMON_INCLUDE

inline float getLuma(float3 rgb) {
    // const float3 lum = float3(0.2126, 0.7152, 0.0722);
    const float3 lum = float3(0.299, 0.587, 0.114);
    return dot(rgb, lum);
}

#endif // COMMON_INCLUDE