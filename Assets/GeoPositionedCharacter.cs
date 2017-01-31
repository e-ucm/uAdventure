using UnityEngine;
using System.Collections;
using MapzenGo.Models;
using UnityStandardAssets.Characters.ThirdPerson;
using MapzenGo.Helpers;
using uAdventure.Geo;

public class GeoPositionedCharacter : MonoBehaviour
{

    public TileManager tileManager;
    public ThirdPersonCharacter thirdPersonCharacter;
    public float minDistanceToWalk = 5; // 2 meters

    public void MoveTo(Vector2d latLon)
    {
        moving = true;
        destination = latLon;
    }

    public void InstantMoveTo(Vector2d latLon)
    {
        LatLon = latLon;
    }

    private Vector2d latLon;
    public Vector2d LatLon {
        get
        {
            return latLon;
        }
        set
        {
            latLon = value;
            destination = value;
            var tileManagerRelative = GM.LatLonToMeters(tileManager.Latitude, tileManager.Longitude);
            var positionRelative = (GM.LatLonToMeters(latLon.x, latLon.y) - tileManagerRelative).ToVector2();
            transform.localPosition = new Vector3(positionRelative.x, transform.localPosition.y, positionRelative.y);
        }
    }

    private bool moving = false;
    private Vector2d destination;

    void Start()
    {
        Input.compass.enabled = true;
    }

    private Vector3 lastPos = Vector3.zero;

    void Update()
    {
        var tileManagerRelative = GM.LatLonToMeters(tileManager.Latitude, tileManager.Longitude);
        var latLonMeters = GM.LatLonToMeters(LatLon.x, LatLon.y) - tileManagerRelative;
        var destinationMeters = GM.LatLonToMeters(destination.x, destination.y) - tileManagerRelative;
        destinationMeters -= latLonMeters;

        if (moving && destinationMeters.sqrMagnitude >= minDistanceToWalk * minDistanceToWalk)
            thirdPersonCharacter.Move(Vector3.ClampMagnitude(new Vector3((float)destinationMeters.x, 0, (float)destinationMeters.y), minDistanceToWalk*3) / (minDistanceToWalk*3), false, false);
        else if(lastPos ==  transform.position)
        {
            moving = false;
            destination = latLon;
            thirdPersonCharacter.Move(new Vector3(0, 0, 0), false, false);
            if(Input.compass.enabled)
                transform.localRotation = Quaternion.Euler(0, Input.compass.trueHeading, 0);
        }

        if (!moving && !GPSController.Instance.IsStarted())
        {
            thirdPersonCharacter.Move(new Vector3(Input.GetAxis("Horizontal")*10, 0, Input.GetAxis("Vertical")*10), false, false);
        }

        lastPos = transform.position;

        this.latLon = GM.MetersToLatLon(transform.localPosition.ToVector2xz().ToVector2d() + tileManagerRelative);
        this.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y >= 0 ? transform.localPosition.y : 0, transform.localPosition.z);
        //Debug.Log("Character at: " + this.LatLon +" moving to " + destination);
    }

    public bool IsLookingTo(Vector2d point)
    {
        return IsLookingTowards((point - LatLon).ToVector3xz().normalized);
    }

    public bool IsLookingTowards(Vector2d direction)
    {
        var headingTo = Input.compass.enabled ? Quaternion.Euler(0, Input.compass.trueHeading, 0) * Vector3.forward : transform.rotation * Vector3.forward;
        var directionNo = direction.ToVector3().normalized;
        var angle = Vector3.Angle(directionNo, headingTo);

        return angle < 15; // 15 degree tolerance
    }
}
    