using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made By JK3WN, original code from GGSS
public class CameraZoom : MonoBehaviour
{
    CinemachineVirtualCamera _virtualCamera;
    Coroutine _coroutine;
    float _originalSize;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _originalSize = _virtualCamera.m_Lens.OrthographicSize;
    }

    IEnumerator DoZoom(float zoomSizeFactor, float startZoomDuration, float zoomStopDuration, float endZoomDuration)
    {
        yield return ZoomRoutine(_originalSize, _originalSize * zoomSizeFactor, startZoomDuration);
        yield return new WaitForSecondsRealtime(zoomStopDuration);
        yield return ZoomRoutine(_originalSize * zoomSizeFactor, _originalSize, endZoomDuration);
        _coroutine = null;
    }

    IEnumerator ZoomRoutine(float startZoomSize, float endZoomSize, float duration)
    {
        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / duration;
            _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startZoomSize, endZoomSize, t);
            yield return null;
        }
        _virtualCamera.m_Lens.OrthographicSize = endZoomSize;
    }

    public void ZoomCamera(float zoomSizeFactor = -1.5f, float startZoomDuration = 0.1f, float zoomStopDuration = 0.5f, float endZoomDuration = 0.5f)
    {
        if(_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(DoZoom(zoomSizeFactor, startZoomDuration, endZoomDuration, zoomStopDuration));
    }
}
