/**
* Jigbox
* Copyright(c) 2016 KLab, Inc. All Rights Reserved.
* Proprietary and Confidential - This source code is not for redistribution
*
* Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
* conditions, Licensor grants to you, and you hereby accept nontransferable,
* nonexclusive limited right to access, obtain, use, copy and/or download
* a copy of this product only for requirement purposes. You may not rent,
* lease, loan, time share, sublicense, transfer, make generally available,
* license, disclose, disseminate, distribute or otherwise make accessible or
* available this product to any third party without the prior written approval
* of Licensor. Unauthorized copying of this product, including modifications
* of this product or programs in which this product has been merged or included
* with other software products is expressly forbidden.
*/

#ifndef JIGBOX_MASK_INCLUDE
#define JIGBOX_MASK_INCLUDE

inline float2 ClipRange(float2 position, float2 rect, float2 offset)
{
	return (position * rect + offset) * 0.5 + float2(0.5, 0.5);
}

inline float TextureClipping(sampler2D clipTexture, float2 uv)
{
	return tex2D(clipTexture, uv).a;
}

inline float2 SoftnessClipRange(float2 position, float2 rect, float2 offset)
{
	return position * rect + offset;
}

inline float SoftnessClipping(float2 range, float2 softness)
{
	float2 value = (float2(1.0, 1.0) - abs(range)) * softness;
	return clamp(min(value.x, value.y), 0.0, 1.0);
}

#endif
