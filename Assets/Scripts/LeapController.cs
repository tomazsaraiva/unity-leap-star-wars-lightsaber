using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeapController : MonoBehaviour
{
    private float MIN_DISABLE_TIME = 2.0f;

    [SerializeField]
    private GameObject _root;

    [SerializeField]
    private Lightsaber _lightsaber;

    [SerializeField]
    private GameObject _grabPoint;

    [SerializeField]
    private float _grabDuration;


    private LeapProvider _leapProvider;


    private bool _grab;
    private float _deltaPosition;
    private float _deltaRotation;

    private bool _hasLightSaber;
    private bool _isEnabled;

    private bool _disable;
    private float _disableTime;


    #region MONOBEHAVIOUR
    
    void Start()
    {
        _leapProvider = FindObjectOfType<LeapProvider>();
    }

    private void Update()
    {
        if(_grab)
        {
            _root.transform.localPosition = Vector3.MoveTowards(_root.transform.localPosition, Vector3.zero, _deltaPosition * Time.deltaTime);
            _root.transform.localRotation = Quaternion.RotateTowards(_root.transform.localRotation, Quaternion.identity, _deltaRotation * Time.deltaTime);

            if(_root.transform.localPosition == Vector3.zero)
            {
                _hasLightSaber = true;
                _grab = false;
            }
        }

        if(_hasLightSaber && _isEnabled)
        {
            if(_leapProvider.CurrentFrame != null && 
                _leapProvider.CurrentFrame.Hands != null &&
                _leapProvider.CurrentFrame.Hands.Count > 0 &&
                _leapProvider.CurrentFrame.Hands[0].PalmVelocity.Magnitude > 0.1f)
            {
                _lightsaber.Swing();
            }
        }

        if(_disable && Time.time - _disableTime > MIN_DISABLE_TIME)
        {
            _disable = false;

            _lightsaber.Close();
        }
    }

    #endregion


    public void Grab()
    {
        if(!_hasLightSaber)
        {
            _root.transform.SetParent(_grabPoint.transform);

            _deltaPosition = Vector3.Distance(_root.transform.localPosition, Vector3.zero) / _grabDuration;
            _deltaRotation = Quaternion.Angle(_root.transform.localRotation, Quaternion.identity) / _grabDuration;

            _lightsaber.Grab();

            _grab = true;
        }
    }
    

    public void Enable(bool enabled)
    {
        if(_hasLightSaber && enabled != _isEnabled)
        {
            if(enabled)
            {
                _lightsaber.Open();
            }
            else
            {
                _disableTime = Time.time;
                _disable = true;
            }

            _isEnabled = enabled;
        }

        if(_disable && enabled)
        {
            _disable = false;
        }
    }

    public void Release()
    {
        if(_hasLightSaber)
        {
            _hasLightSaber = false;

            _lightsaber.Release();
        }
    }
}