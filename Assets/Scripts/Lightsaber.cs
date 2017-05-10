using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightsaber : MonoBehaviour
{
    [SerializeField]
    private GameObject _light;

    [SerializeField]
    private ParticleSystem _particleSystem;

    [SerializeField]
    private float _openDuration;

    [SerializeField]
    private float _closeDuration;

    [SerializeField]
    private float _resetDuration;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioSource _audioSourceSecondary;

    [SerializeField]
    private AudioClip[] _audioClips;


    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Vector3 _originalScale;

    private bool _free = true;
    private Vector3 _randomPosition;
    private float _randomRotation;

    private bool _closed = true;
    private bool _toggle;
    private float _targetScale;
    private float _speed;


    private void Awake()
    {
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        _originalScale = transform.localScale;
    }

    private void Start()
    {
        _randomPosition = transform.position;
        _randomRotation = transform.rotation.eulerAngles.y;
    }

    private void Update()
    {
        if (_free)
        {
            if(_randomPosition == transform.position)
            {
                _randomPosition = Random.insideUnitSphere * 0.01f;
            }

            transform.position = Vector3.MoveTowards(transform.position, _randomPosition, Time.deltaTime * 0.01f);


            if (Mathf.Approximately(_randomRotation, transform.rotation.eulerAngles.y))
            {
                _randomRotation = Random.Range(45, 145);
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, _randomRotation, transform.rotation.eulerAngles.z)), Time.deltaTime * 10f);
        }

        if(_toggle)
        {
            float scale = Mathf.MoveTowards(_light.transform.localScale.z, _targetScale, _speed * Time.deltaTime);

            _light.transform.localScale = new Vector3(_light.transform.localScale.x, _light.transform.localScale.y, scale);

            if(scale == _targetScale)
            {
                _closed = _targetScale == 0;

                if(!_closed)
                {
                    _audioSource.clip = _audioClips[2];
                    _audioSource.loop = true;
                    _audioSource.volume = 0.75f;
                    _audioSource.Play();
                }

                _toggle = false;
            }
        }
    }


    public void Open()
    {
        if(_closed)
        {
            _audioSource.clip = _audioClips[0];
            _audioSource.loop = false;
            _audioSource.volume = 1.0f;
            _audioSource.Play();

            _targetScale = 1;
            _speed = Mathf.Abs(_light.transform.localScale.z - _targetScale) / _openDuration;
            _toggle = true;

            _particleSystem.Play();
        }
    }

    public void Swing()
    {
        _audioSourceSecondary.Play();
    }

    public void Close()
    {
        if(!_closed)
        {
            _audioSource.clip = _audioClips[1];
            _audioSource.loop = false;
            _audioSource.volume = 1.0f;
            _audioSource.Play();

            _targetScale = 0;
            _speed = Mathf.Abs(_light.transform.localScale.z - _targetScale) / _closeDuration;
            _toggle = true;

            _particleSystem.Stop();
        }
    }

    public void Grab()
    {
        _free = false;
    }

    public void Release()
    {
        StartCoroutine(ReleaseCoroutine());
    }

    private IEnumerator ReleaseCoroutine()
    {
        transform.SetParent(null);

        float deltaPosition = Vector3.Distance(transform.position, _originalPosition) / _resetDuration;
        float deltaRotation = Quaternion.Angle(transform.rotation, _originalRotation) / _resetDuration;

        while (transform.position != _originalPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, _originalPosition, deltaPosition * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _originalRotation, deltaRotation * Time.deltaTime);

            yield return 0;
        }

        Close();

        _free = true;
    }
}