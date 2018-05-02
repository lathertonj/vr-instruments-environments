#pragma strict

@script RequireComponent(LineRenderer)

var myVelocity : float = 30.0;
var numberOfPoints : int = 10;
var gravity : float = 9.81;
var myAngle : float;
var sinAngle : float;
private var lineRenderer : LineRenderer;
var cube : Transform;
var points : Vector3[];
var shortLength : int;
var projectile : Rigidbody;
var materials : Material[];
static var width : float = 0.1;
var totalDistance : float = 0.0;
static var animateTexture : boolean = true;
static var animationSpeed : float = 2;
static var textureColor : Color;
static var currentTexture : int;
private var currentAnimationOffset : float;

function Start () {
lineRenderer = GetComponent(LineRenderer);
lineRenderer.SetWidth(width,width);
}

function Update () {


//if(Input.GetKey(KeyCode.C))
//Camera.main.transform.RotateAround(transform.position,-Vector3.up,Time.deltaTime * 20);

myAngle = Mathf.Cos((Vector3.Angle(transform.forward,cube.forward)/100));
sinAngle = Mathf.Sin((Vector3.Angle(transform.forward,cube.forward)/100));
points = new Vector3[numberOfPoints + 1];
lineRenderer.SetVertexCount(points.Length + 1);
lineRenderer.SetPosition(0, transform.position);
var length = numberOfPoints;

if(shortLength > 0)
length = shortLength;

totalDistance = Vector3.Distance(transform.position,points[0]);

for(var t = 0; t < length;t++)
{
points[t] = transform.position; 
var tempX = myVelocity * t*.5 * myAngle;
points[t] = points[t] + cube.forward * tempX;
var tempY = myVelocity * t*.5 * sinAngle - .5*gravity*(Mathf.Pow(t*.5, 2));
points[t] = points[t] + cube.up * tempY;
lineRenderer.SetPosition(t+1, points[t]);
var hit : RaycastHit;
if(t > 0){
if(Physics.Linecast(points[t],points[t-1],hit)){
if(hit.transform.tag == "solid"){
lineRenderer.SetVertexCount(t+2);
shortLength = t+2;
lineRenderer.SetPosition(t+1, hit.point);
points[t] = hit.point;
break;
}
}
else{
lineRenderer.SetVertexCount(numberOfPoints+1);
shortLength = 0;
}
if(t != length -1)
totalDistance+= Vector3.Distance(points[t],points[t-1]);
//Debug.Log("Point "+ (t-1) + " to Point " +t + ": " + Vector3.Distance(points[t],points[t-1]));
//Debug.Log("TOTAL DISTANCE: " + totalDistance); 
}
}
if(Input.GetKeyDown(KeyCode.Space)){
if(Physics.gravity != Vector3(0,-gravity,0))
Physics.gravity = Vector3(0,-gravity,0);
var clone = Instantiate(projectile,points[0],transform.rotation);
clone.AddForce((points[1]-points[0]) *myVelocity* 3.5);
}
if(Input.GetKeyDown(KeyCode.X)){
clone = Instantiate(projectile,points[0],transform.rotation);
//clone.isKinematic = true;
clone.useGravity=false;
clone.GetComponent(projectileArc).FollowPoints(points,length);
}

currentAnimationOffset -= animationSpeed * Time.deltaTime;

lineRenderer.material = materials[currentTexture];
if(currentTexture == 0)
lineRenderer.material.mainTextureScale = Vector2 ((totalDistance/(width*2)),1);
else
lineRenderer.material.mainTextureScale = Vector2 (1,1);
if(animateTexture)
lineRenderer.material.mainTextureOffset.x = currentAnimationOffset;
if(lineRenderer.material.HasProperty("_Color")){
//lineRenderer.material.color.r = textureColor.r;
//lineRenderer.material.color.g = textureColor.g;
//lineRenderer.material.color.b = textureColor.b;
    }
    lineRenderer.SetWidth(width,width);
}
