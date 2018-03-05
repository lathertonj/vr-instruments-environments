using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // laser!
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private Collider hitCollider;
    private bool clearHitColliderOnNextFrame;

    // meta: is it calculated on press of touchpad or only on touch of touchpad
    public bool laserOnTouchpadButtonPress = true;

    public Collider GetFoundCollider()
    {
        return hitCollider;
    }

    public Vector3 GetHitPoint()
    {
        return hitPoint;
    }

	private void ShowLaser( RaycastHit hit )
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint); 
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    private void HideLaser()
    {
        laser.SetActive( false );
        clearHitColliderOnNextFrame = true;
    }

    private void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    private void Update()
    {
        if( clearHitColliderOnNextFrame ) { hitCollider = null; clearHitColliderOnNextFrame = false; }

        if( (laserOnTouchpadButtonPress && Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
            || (!laserOnTouchpadButtonPress && Controller.GetAxis() != Vector2.zero ) )
        {
            RaycastHit hit;

            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 400))
            {
                hitPoint = hit.point;
                ShowLaser(hit);
                hitCollider = hit.collider;
            }
            else
            {
                HideLaser();
            }
        }
        else 
        {
            HideLaser();
        }
    }
}
